using SingleTableDynamo.Metrics;
using System;
using System.Threading.Tasks;

namespace SingleTableDynamo.Tests.Components.Utilities
{
    public class DoNothingMetricWriter : IMetricWriter
    {
        private readonly IDisposable _doNothingDisposable;

        public DoNothingMetricWriter()
        {
            _doNothingDisposable = new DoNothingDisposable();
        }

        public IDisposable BeginApiResponseTimeScope(string endpoint)
        {
            return _doNothingDisposable;
        }

        public IDisposable BeginDynamoDBResponseTimeScope(string operationName)
        {
            return _doNothingDisposable;
        }

        public IDisposable BeginLambdaFunctionHandlerTimeScope(string handlerName)
        {
            return _doNothingDisposable;
        }

        public IDisposable BeginProcessTimeScope(string dimensionName, string dimensionValue)
        {
            return _doNothingDisposable;
        }

        public void Dispose()
        {
            _doNothingDisposable.Dispose();
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public void TrackApiRequest(string endpoint)
        {
        }

        public void TrackCount(string dimensionName, string dimensionValue)
        {
        }

        public void TrackException(string endpoint)
        {
        }

        public void TrackLambdaFunctionHandler(string handlerName)
        {
        }

        public void TrackProcessTime(string dimensionName, string dimensionValue, double metricValue)
        {
        }
    }
}