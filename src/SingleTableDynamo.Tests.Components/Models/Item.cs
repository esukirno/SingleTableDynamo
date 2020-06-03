using SingleTableDynamo.Documents;

namespace SingleTableDynamo.Tests.Components.Models
{
    public class Item : ISingleTableItem
    {
        public string HashKey => "HashKey";

        public string SortKey => "SortKey";

        public string Value { get; set; }
    }
}