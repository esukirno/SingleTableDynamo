using System;

namespace SingleTableDynamo.Tests.Components.Utilities
{
    public class DoNothingDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}