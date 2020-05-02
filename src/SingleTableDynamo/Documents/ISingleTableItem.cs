namespace SingleTableDynamo.Documents
{
    public interface ISingleTableItem
    {
        string HashKey { get; }

        string SortKey { get; }
    }
}