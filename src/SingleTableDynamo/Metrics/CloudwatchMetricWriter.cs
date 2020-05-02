using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SingleTableDynamo.Metrics
{
    public class CloudwatchMetricWriter : IMetricWriter
    {
        private readonly IAmazonCloudWatch _client;
        private readonly List<MetricDatum> _metricData;
        private readonly string _namespace;

        public CloudwatchMetricWriter(IAmazonCloudWatch client, IOptions<MetricWriterSettings> settings)
        {
            _client = client;
            _metricData = new List<MetricDatum>();
            _namespace = settings.Value.CloudWatchNamespace;
        }

        public void TrackLambdaFunctionHandler(string handlerName)
        {
            TrackCount("LambdaFunctionHandler", handlerName);
        }

        public void TrackApiRequest(string endpoint)
        {
            TrackCount("ApiRequest", endpoint);
        }

        public void TrackException(string endpoint)
        {
            TrackCount("Exception", endpoint);
        }

        public IDisposable BeginApiResponseTimeScope(string endpoint)
        {
            return new MetricTimingScope(this, "ApiResponse", endpoint);
        }

        public IDisposable BeginLambdaFunctionHandlerTimeScope(string handlerName)
        {
            return new MetricTimingScope(this, "LambdaFunctionHandler", handlerName);
        }

        public IDisposable BeginDynamoDBResponseTimeScope(string operationName)
        {
            return new MetricTimingScope(this, "DynamoDB", operationName);
        }

        public IDisposable BeginProcessTimeScope(string dimensionName, string dimensionValue)
        {
            return new MetricTimingScope(this, dimensionName, dimensionValue);
        }

        public void TrackCount(string dimensionName, string dimensionValue)
        {
            _metricData.Add(new MetricDatum
            {
                Dimensions = new List<Dimension> { new Dimension { Name = dimensionName, Value = dimensionValue } },
                MetricName = "Count",
                Value = 1.0,
                Unit = StandardUnit.Count
            });
        }

        public void TrackProcessTime(string dimensionName, string dimensionValue, double metricValue)
        {
            _metricData.Add(new MetricDatum
            {
                Dimensions = new List<Dimension> { new Dimension { Name = dimensionName, Value = dimensionValue } },
                MetricName = "Time",
                Value = metricValue,
                Unit = StandardUnit.Milliseconds
            });
        }

        public async Task FlushAsync()
        {
            await _client.PutMetricDataAsync(new PutMetricDataRequest
            {
                MetricData = _metricData.ToList(), //creates a copy so that _metricData can be cleared
                Namespace = _namespace
            });
            _metricData.Clear();
        }

        public async void Dispose()
        {
            await FlushAsync();
        }
    }
}