namespace SingleTableDynamo.Helpers
{
    public static class PredicateOperationHelper
    {
        public static string VariableFor(this string attributeName)
        {
            return $":v_{attributeName}";
        }

        public static string AliasFor(this string attributeName)
        {
            return $"#{attributeName}";
        }
    }
}