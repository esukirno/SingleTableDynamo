using SingleTableDynamo.Documents;
using SingleTableDynamo.Predicates;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SingleTableDynamo
{
    public interface IDynamoDBSingleTableRepository<T> where T : ISingleTableItem
    {
        string HashKeyName { get; }

        string SortKeyName { get; }

        Task<List<T>> QueryAsync(HashKeySearchPredicate hashKeySearchPredicate, SortKeySearchPredicate sortKeySearchPredicate, IEnumerable<FilterSearchPredicate> filterSearchPredicates = null, CancellationToken cancellationToken = default);

        Task BatchUpsertAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);

        Task BatchDeleteAsync(IEnumerable<T> items, CancellationToken cancellationToken = default);

        Task UpsertAsync(T item, IEnumerable<FilterSearchPredicate> conditionPredicates = null, CancellationToken cancellationToken = default);

        Task DeleteAsync(T item, IEnumerable<FilterSearchPredicate> conditionPredicates = null, CancellationToken cancellationToken = default);
    }
}