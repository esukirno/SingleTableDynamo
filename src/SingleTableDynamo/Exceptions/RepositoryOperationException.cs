using System;

namespace SingleTableDynamo.Exceptions
{
    public class RepositoryOperationException : Exception
    {
        public RepositoryOperationException(string message) : base(message)
        {
        }
    }
}