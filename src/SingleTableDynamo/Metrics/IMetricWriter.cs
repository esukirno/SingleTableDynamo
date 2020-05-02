using System;
using System.Threading.Tasks;

namespace SingleTableDynamo.Metrics
{
    public interface IMetricWriter : IDisposable
    {
        void TrackCount(string dimensionName, string dimensionValue);

        void TrackProcessTime(string dimensionName, string dimensionValue, double metricValue);

        void TrackLambdaFunctionHandler(string handlerName);

        void TrackApiRequest(string endpoint);

        void TrackException(string endpoint);

        IDisposable BeginLambdaFunctionHandlerTimeScope(string handlerName);

        IDisposable BeginApiResponseTimeScope(string endpoint);

        IDisposable BeginDynamoDBResponseTimeScope(string operationName);

        IDisposable BeginProcessTimeScope(string dimensionName, string dimensionValue);

        Task FlushAsync();
    }
}