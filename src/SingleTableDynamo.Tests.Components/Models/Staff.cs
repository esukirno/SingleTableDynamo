using SingleTableDynamo.Documents;

namespace SingleTableDynamo.Tests.Components.Models
{
    public class Staff : ISingleTableItem
    {
        public string HashKey => "Staff#" + Id;

        public string SortKey => "DOB#" + DOB;

        public string Id { get; set; }

        public string Name { get; set; }

        public string DOB { get; set; }
    }
}