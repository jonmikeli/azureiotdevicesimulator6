using IoT.Simulator.Models;

using System;

namespace IoT.Simulator.Services
{
    public interface ITimeSeriesSimulatorService
    {
        public TimeSeriesNumericDataItem CreateTimeSeriesNumericData(TimeSeriesNumericDataItem from, DateTime to, int min, int max);
    }
}
