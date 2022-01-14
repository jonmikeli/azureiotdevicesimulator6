using IoT.Simulator.Extensions;
using IoT.Simulator.Models;

using System;
using System.Collections.Generic;

namespace IoT.Simulator.Services
{
    internal class TimeSeriesSimulatorService : ITimeSeriesSimulatorService
    {
        public TimeSeriesNumericDataItem CreateTimeSeriesNumericData(TimeSeriesNumericDataItem from, DateTime end, int min, int max)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            if (from.Date > end)
                throw new Exception("Start date is over the end date.");

            TimeSeriesNumericDataItem result = new TimeSeriesNumericDataItem();
            Random r = new Random(DateTime.UtcNow.Millisecond);

            result.Date = RandomDate(r, from.Date, end);
            result.Value = r.Next(min, max);

            return result;
        }

        private DateTime RandomDate(Random r, DateTime dateStart, DateTime dateEnd)
        {
            long timestampStart = dateStart.TimeStamp();
            long timestampEnd = dateStart.TimeStamp();
            long newDate = r.NextInt64(timestampStart, timestampEnd);

            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(newDate).ToUniversalTime();
        }
    }
}
