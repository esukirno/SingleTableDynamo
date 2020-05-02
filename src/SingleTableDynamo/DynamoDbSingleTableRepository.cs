using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using SingleTableDynamo.Documents;
using SingleTableDynamo.Exceptions;
using SingleTableDynamo.Extensions;
using SingleTableDynamo.Metrics;
using SingleTableDynamo.Predicates;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamoDBDocument = Amazon.DynamoDBv2.DocumentModel.Document;

namespace SingleTableDynamo
{
    public class DynamoDBSingleTableRepository<T> : IDynamoDBSingleTableRepository<T> where T : ISingleTableItem
    {
        private const int MaxDynamoDBBatchGet = 100;
        private const int MaxDynamoDBBatchWrite = 25;
        private const int PollyRetryCount = 3;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly IMetricWriter _metricWriter;
        private readonly string _tableName;
        private readonly string _objectName;

        public string HashKeyName { get; }

        public string SortKeyName { get; }

        public DynamoDBSingleTableRepository(IAmazonDynamoDB dynamoDbClient, IDynamoDBContext dynamoDbContext, IMetricWriter metricWriter, string tableName, string hashKeyName = "HashKey", string sortKeyName = "SortKey")
        {
            _dynamoDbClient = dynamoDbClient;
            _dynamoDbContext = dynamoDbContext;
            _metricWriter = metricWriter;
            _tableName = tableName;
            _objectName = typeof(T).Name;
            HashKeyName = hashKeyName;
            SortKeyName = sortKeyName;
        }

        public async Task<List<T>> QueryAsync(HashKeySearchPredicate hashKeySearchPredicate, SortKeySearchPredicate sortKeySearchPredicate, IEnumerable<FilterSearchPredicate> filterSearchPredicates, CancellationToken cancellationToken = default)
        {
            var keyConditionExpression = hashKeySearchPredicate.ToFilterExpression() + " AND " + sortKeySearchPredicate.ToFilterExpression();
            string filterExpression = null;

            var expressionAttributeValues = new Dictionary<string, AttributeValue>();
            var expressionAttributeNames = new Dictionary<string, string>();

            PopulateAttributeNames(expressionAttributeNames, hashKeySearchPredicate);
            PopulateAttributeValues(expressionAttributeValues, hashKeySearchPredicate);

            PopulateAttributeNames(expressionAttributeNames, sortKeySearchPredicate);
            PopulateAttributeValues(expressionAttributeValues, sortKeySearchPredicate);

            if (filterSearchPredicates != null && filterSearchPredicates.Any())
            {
                filterExpression = string.Join(" AND ", filterSearchPredicates.Select(x => x.ToFilterExpression()));

                foreach (var predicate in filterSearchPredicates)
                {
                    PopulateAttributeNames(expressionAttributeNames, predicate);
                    PopulateAttributeValues(expressionAttributeValues, predicate);
                }
            }

            return await RunQueryAsync(keyConditionExpression, filterExpression, expressionAttributeNames, expressionAttributeValues, cancellationToken);
        }

        public async Task BatchUpsertAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            var writeRequests = items.Select(item => new WriteRequest
            {
                PutRequest = new PutRequest
                {
                    Item = ConvertToAttributes(item)
                }
            });

            var unprocessedRequests = await RunWriteRequestWithRetryAsync(writeRequests, cancellationToken);

            if (unprocessedRequests.Any())
            {
                throw new RepositoryOperationException($"Unable to process {unprocessedRequests.Count} out of {writeRequests.Count()} DynamoDB WriteRequests");
            }
        }

        public async Task BatchDeleteAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            var writeRequests = items.Select(item => new WriteRequest
            {
                DeleteRequest = new DeleteRequest
                {
                    Key = GetKeyAttributeValues(item)
                }
            });

            var objectName = typeof(T).Name;

            var unprocessedRequests = await RunWriteRequestWithRetryAsync(writeRequests, cancellationToken);

            if (unprocessedRequests.Any())
            {
                throw new RepositoryOperationException($"Unable to process {unprocessedRequests.Count} out of {writeRequests.Count()} DynamoDB WriteRequests");
            }
        }

        private Dictionary<string, AttributeValue> ConvertToAttributes(T item)
        {
            var attributes = _dynamoDbContext.ToDocument<T>(item).ToAttributeMap();
            attributes["HashKey"] = new AttributeValue(item.HashKey);
            attributes["SortKey"] = new AttributeValue(item.SortKey);
            return attributes;
        }

        public async Task UpsertAsync(T item, IEnumerable<FilterSearchPredicate> conditionPredicates = null, CancellationToken cancellationToken = default)
        {
            string filterExpression = null;
            var expressionAttributeNames = new Dictionary<string, string>();
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            if (conditionPredicates != null && conditionPredicates.Any())
            {
                filterExpression = string.Join(" AND ", conditionPredicates.Select(x => x.ToFilterExpression(true)));

                foreach (var predicate in conditionPredicates)
                {
                    PopulateAttributeNames(expressionAttributeNames, predicate);
                    PopulateAttributeValues(expressionAttributeValues, predicate);
                }
            }

            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = ConvertToAttributes(item),
                ExpressionAttributeNames = expressionAttributeNames,
                ExpressionAttributeValues = expressionAttributeValues,
                ConditionExpression = filterExpression
            };

            try
            {
                // For full list of DynamoDB exceptions
                // https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Programming.Errors.html
                await Policy
                .Handle<ItemCollectionSizeLimitExceededException>()
                .Or<LimitExceededException>()
                .Or<ProvisionedThroughputExceededException>()
                .Or<RequestLimitExceededException>()
                .Or<InternalServerErrorException>()
                .Or<TransactionConflictException>()
                .WaitAndRetryAsync(PollyRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(async () =>
                {
                    using (_metricWriter.BeginDynamoDBResponseTimeScope("PutItemAsync:" + _objectName))
                    {
                        return await _dynamoDbClient.PutItemAsync(request, cancellationToken);
                    }
                });
            }
            catch (ConditionalCheckFailedException)
            {
                // When ConditionExpression check fails, just ignore it
            }
        }

        public async Task DeleteAsync(T item, IEnumerable<FilterSearchPredicate> conditionPredicates = null, CancellationToken cancellationToken = default)
        {
            string filterExpression = null;
            var expressionAttributeNames = new Dictionary<string, string>();
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            if (conditionPredicates != null && conditionPredicates.Any())
            {
                filterExpression = string.Join(" AND ", conditionPredicates.Select(x => x.ToFilterExpression(true)));

                foreach (var predicate in conditionPredicates)
                {
                    PopulateAttributeNames(expressionAttributeNames, predicate);
                    PopulateAttributeValues(expressionAttributeValues, predicate);
                }
            }

            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = GetKeyAttributeValues(item),
                ExpressionAttributeNames = expressionAttributeNames,
                ExpressionAttributeValues = expressionAttributeValues,
                ConditionExpression = filterExpression
            };

            try
            {
                // For full list of DynamoDB exceptions
                // https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Programming.Errors.html
                await Policy
                    .Handle<ItemCollectionSizeLimitExceededException>()
                    .Or<LimitExceededException>()
                    .Or<ProvisionedThroughputExceededException>()
                    .Or<RequestLimitExceededException>()
                    .Or<InternalServerErrorException>()
                    .Or<TransactionConflictException>()
                    .WaitAndRetryAsync(PollyRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                    .ExecuteAsync(async () =>
                    {
                        using (_metricWriter.BeginDynamoDBResponseTimeScope("DeleteItemAsync:" + _objectName))
                        {
                            return await _dynamoDbClient.DeleteItemAsync(request, cancellationToken);
                        }
                    });
            }
            catch (ConditionalCheckFailedException)
            {
                // When ConditionExpression check fails, just ignore it
            }
        }

        private static void PopulateAttributeNames(Dictionary<string, string> attributeNames, ISearchPredicate searchPredicate)
        {
            foreach (var item in searchPredicate.ToExpressionAttributeNames())
            {
                attributeNames.Add(item.Key, item.Value);
            }
        }

        private static void PopulateAttributeValues(Dictionary<string, AttributeValue> attributeValues, ISearchPredicate searchPredicate)
        {
            foreach (var item in searchPredicate.ToExpressionAttributeValues())
            {
                attributeValues.Add(item.Key, item.Value);
            }
        }

        private static ConcurrentBag<WriteRequest> ToConcurrentBag(IEnumerable<WriteRequest> writeRequests)
        {
            var output = new ConcurrentBag<WriteRequest>();
            foreach (var request in writeRequests)
            {
                output.Add(request);
            }

            return output;
        }

        private Dictionary<string, AttributeValue> GetKeyAttributeValues(T item)
        {
            return new Dictionary<string, AttributeValue>
            {
                { HashKeyName, new AttributeValue { S = item.HashKey } },
                { SortKeyName, new AttributeValue { S = item.SortKey } }
            };
        }

        private async Task<List<T>> RunQueryAsync(string keyConditionExpression, string filterExpression, Dictionary<string, string> expressionAttributeNames, Dictionary<string, AttributeValue> expressionAttributeValues, CancellationToken cancellationToken = default)
        {
            var output = new List<T>();
            Dictionary<string, AttributeValue> lastKeyEvaluated = null;
            do
            {
                var request = new QueryRequest
                {
                    TableName = _tableName,
                    Limit = MaxDynamoDBBatchGet,
                    KeyConditionExpression = keyConditionExpression,
                    FilterExpression = filterExpression,
                    ExpressionAttributeNames = expressionAttributeNames,
                    ExpressionAttributeValues = expressionAttributeValues,
                    ExclusiveStartKey = lastKeyEvaluated
                };

                QueryResponse response;
                using (_metricWriter.BeginDynamoDBResponseTimeScope("QueryAsync:" + _objectName))
                {
                    response = await _dynamoDbClient.QueryAsync(request, cancellationToken);
                }

                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    var outputItem = _dynamoDbContext.FromDocument<T>(DynamoDBDocument.FromAttributeMap(item));
                    output.Add(outputItem);
                }

                lastKeyEvaluated = response.LastEvaluatedKey;
            } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);

            return output;
        }

        private async Task<List<WriteRequest>> RunWriteRequestAsync(IEnumerable<WriteRequest> writeRequests, CancellationToken cancellationToken)
        {
            var responses = await Task.WhenAll(
                writeRequests.Batch(MaxDynamoDBBatchWrite)
                    .Select(batch =>
                    {
                        var batchWriteRequest = new BatchWriteItemRequest
                        {
                            RequestItems = new Dictionary<string, List<WriteRequest>>
                            {
                                { _tableName, batch.ToList() }
                            }
                        };

                        using (_metricWriter.BeginDynamoDBResponseTimeScope("BatchWriteItemAsync:" + _objectName))
                        {
                            return _dynamoDbClient.BatchWriteItemAsync(batchWriteRequest, cancellationToken);
                        }
                    }));

            var unprocessedItems = new List<WriteRequest>();
            foreach (var response in responses)
            {
                if (response.UnprocessedItems.Any() && response.UnprocessedItems.ContainsKey(_tableName))
                {
                    unprocessedItems.AddRange(response.UnprocessedItems[_tableName]);
                }
            }

            return unprocessedItems.ToList();
        }

        private async Task<List<WriteRequest>> RunWriteRequestWithRetryAsync(IEnumerable<WriteRequest> writeRequests, CancellationToken cancellationToken)
        {
            var requestsBag = ToConcurrentBag(writeRequests);

            await Policy
                .HandleResult<ConcurrentBag<WriteRequest>>(unprocessedItems => { return unprocessedItems.Count > 0; })
                .WaitAndRetryAsync(PollyRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(async () =>
                {
                    var unprocessedItems = await RunWriteRequestAsync(requestsBag, cancellationToken);
                    requestsBag.Clear();
                    foreach (var item in unprocessedItems)
                    {
                        requestsBag.Add(item);
                    }

                    return requestsBag;
                });

            return requestsBag.ToList();
        }
    }
}