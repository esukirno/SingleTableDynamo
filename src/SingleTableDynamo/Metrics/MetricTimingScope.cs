using System;
using System.Diagnostics;

namespace SingleTableDynamo.Metrics
{
    public class MetricTimingScope : IDisposable
    {
        private readonly IMetricWriter _metricWriter;
        private readonly string _dimensionName;
        private readonly string _dimensionValue;
        private readonly Stopwatch _stopWatch;

        public MetricTimingScope(IMetricWriter metricWriter, string dimensionName, string dimensionValue)
        {
            _metricWriter = metricWriter;
            _dimensionValue = dimensionValue;
            _dimensionName = dimensionName;
            _stopWatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _metricWriter.TrackProcessTime(_dimensionName, _dimensionValue, _stopWatch.ElapsedMilliseconds);
            _stopWatch.Stop();
        }
    }
}