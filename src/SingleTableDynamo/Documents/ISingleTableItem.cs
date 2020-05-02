namespace SingleTableDynamo.Documents
{
    public interface ISingleTableItem
    {
        string HashKey { get; set; }

        string SortKey { get; set; }
    }
}