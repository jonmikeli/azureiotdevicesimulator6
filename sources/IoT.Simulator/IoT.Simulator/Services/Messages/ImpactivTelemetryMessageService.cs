using IoT.Simulator.Extensions;
using IoT.Simulator.Models;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    public class ImpactivTelemetryMessageService : ITelemetryMessageService
    {
        private ILogger _logger;
        private string _logPrefix;
        private ITimeSeriesSimulatorService _timeseriesSimulationService;
        private ImpactivMessage _lastCreatedItem;

        public ImpactivTelemetryMessageService(ITimeSeriesSimulatorService timeseriesSimulationService, ILoggerFactory loggerFactory)
        {
            if (timeseriesSimulationService == null)
                throw new ArgumentNullException(nameof(timeseriesSimulationService));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _timeseriesSimulationService = timeseriesSimulationService;

            _logger = loggerFactory.CreateLogger<SimpleTelemetryMessageService>();
            _logPrefix = "ImpactivTelemetryMessageService".BuildLogPrefix();
        }

        public async Task<string> GetMessageAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetMessageAsync(string deviceId, string moduleId)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetRandomizedMessageAsync(string deviceId, string moduleId)
        {
            string artifactId = string.IsNullOrEmpty(moduleId) ? deviceId : moduleId;
            _logger.LogTrace($"{_logPrefix}::GetRandomizedMessageAsync::{artifactId}.");

            ImpactivMessage data = MessageFactory.CreateMessage<ImpactivMessage>(artifactId);

            //Randomize data
            if (_lastCreatedItem == null)
                _lastCreatedItem = MessageFactory.RandomizeMessage<ImpactivMessage>(data, DateTime.UtcNow.AddMonths(-5), DateTime.UtcNow, 0, 20, 0, 5);
            else
            {

                TimeSeriesNumericDataItem newDataItemIn = _timeseriesSimulationService.CreateTimeSeriesNumericData(
                        new TimeSeriesNumericDataItem
                        {
                            Date = _lastCreatedItem.ISOTime,
                            Value = _lastCreatedItem.CounterPeopleIn
                        },
                        _lastCreatedItem.ISOTime.AddHours(1),
                            0,
                            10
                            );

                TimeSeriesNumericDataItem newDataItemOut = _timeseriesSimulationService.CreateTimeSeriesNumericData(
                    new TimeSeriesNumericDataItem
                    {
                        Date = _lastCreatedItem.ISOTime,
                        Value = _lastCreatedItem.CounterPeopleOut
                    },
                    _lastCreatedItem.ISOTime.AddHours(1),
                        0,
                        _lastCreatedItem.CounterPeopleIn
                        );

                _lastCreatedItem.CounterPeopleIn = newDataItemIn.Value;
                _lastCreatedItem.CounterPeopleOut = newDataItemOut.Value;
                _lastCreatedItem.ISOTime = newDataItemOut.Date.ToUniversalTime();
            }

            _logger.LogTrace($"{_logPrefix}::{artifactId}::Randomized data to update template's values before sending the message.");

            if (_lastCreatedItem != null)
                return JsonConvert.SerializeObject(_lastCreatedItem, Formatting.Indented);
            else
                return null;
        }

        public async Task<string> GetRandomizedMessageAsync(string deviceId, string moduleId, Direction direction)
        {
            string artifactId = string.IsNullOrEmpty(moduleId) ? deviceId : moduleId;
            _logger.LogTrace($"{_logPrefix}::GetRandomizedMessageAsync::{artifactId}.");

            ImpactivMessage data = MessageFactory.CreateMessage<ImpactivMessage>(artifactId);

            //Randomize data
            if (_lastCreatedItem == null)
                _lastCreatedItem = MessageFactory.RandomizeMessage<ImpactivMessage>(data, DateTime.UtcNow.AddMonths(-5), DateTime.UtcNow, 0, 20, 0, 0);

            if (direction == Direction.In)
            {
                TimeSeriesNumericDataItem newDataItemIn = _timeseriesSimulationService.CreateTimeSeriesNumericData(
                        new TimeSeriesNumericDataItem
                        {
                            Date = _lastCreatedItem.ISOTime,
                            Value = _lastCreatedItem.CounterPeopleIn
                        },
                        _lastCreatedItem.ISOTime.AddHours(1),
                            1,
                            10
                            );

                _lastCreatedItem.CounterPeopleIn = newDataItemIn.Value;
                _lastCreatedItem.CounterPeopleOut = 0;
                _lastCreatedItem.ISOTime = newDataItemIn.Date.ToUniversalTime();
            }
            else if (direction == Direction.Out)
            {
                TimeSeriesNumericDataItem newDataItemOut = _timeseriesSimulationService.CreateTimeSeriesNumericData(
                    new TimeSeriesNumericDataItem
                    {
                        Date = _lastCreatedItem.ISOTime,
                        Value = _lastCreatedItem.CounterPeopleOut
                    },
                    _lastCreatedItem.ISOTime.AddHours(1),
                        1,
                        2
                        );

                _lastCreatedItem.CounterPeopleIn = 0;
                _lastCreatedItem.CounterPeopleOut = newDataItemOut.Value;
                _lastCreatedItem.ISOTime = newDataItemOut.Date.ToUniversalTime();
            }

            _logger.LogTrace($"{_logPrefix}::{artifactId}::Randomized data to update template's values before sending the message.");

            if (_lastCreatedItem != null)
                return JsonConvert.SerializeObject(_lastCreatedItem, Formatting.Indented);
            else
                return null;
        }
    }

    public enum Direction
    {
        In,
        Out
    }

    internal static class MessageFactory
    {
        static internal T CreateMessage<T>(string deviceId)
            where T : ImpactivMessage, new()
        {
            return CreateMessage<T>(deviceId, DateTime.UtcNow);
        }

        static internal T CreateMessage<T>(string deviceId, DateTime date, int peopleIn = 0, int peopleOut = 0)
            where T : ImpactivMessage, new()
        {
            T result = new T();

            result.CounterPeopleIn = peopleIn;
            result.CounterPeopleOut = peopleOut;
            result.ISOTime = date;
            result.Time = date;
            result.DeviceId = deviceId;

            return result;
        }

        static internal T RandomizeMessage<T>(T data, DateTime dateStart, DateTime dateEnd, int inMin, int inMax, int outMin, int outMax)
            where T : ImpactivMessage
        {
            if (data == default(T))
                throw new ArgumentNullException(nameof(data));

            Random r = new Random(DateTime.Now.Millisecond);

            data.Time = RandomDate(r, dateStart, dateEnd);
            data.ISOTime = data.Time;
            data.CounterPeopleIn = r.Next(inMin, inMax);
            data.CounterPeopleOut = r.Next(outMin, outMax);

            return data;
        }

        static private DateTime RandomDate(Random r, DateTime dateStart, DateTime dateEnd)
        {
            long timestampStart = dateStart.TimeStamp();
            long timestampEnd = dateStart.TimeStamp();
            long newDate = r.NextInt64(timestampStart, timestampEnd);

            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(newDate).ToUniversalTime();
        }
    }
}
