using IoT.Simulator.Extensions;
using IoT.Simulator.Models;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

namespace IoT.Simulator.Services
{
    //https://dejanstojanovic.net/aspnet/2018/december/registering-multiple-implementations-of-the-same-interface-in-aspnet-core/
    public class ImpactivTelemetryMessageService : ITelemetryMessageService
    {
        private ILogger _logger;
        private string _logPrefix;
        private string fileTemplatePath = @"./Messages/measureddata.json";
        private ITimeSeriesSimulatorService _timeseriesSimulationService;

        public ImpactivTelemetryMessageService(ITimeSeriesSimulatorService timeseriesSimulationService, ILoggerFactory loggerFactory)
        {
            if (timeseriesSimulationService == null)
                throw new ArgumentNullException(nameof(timeseriesSimulationService));

            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

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
            data = MessageFactory.RandomizeMessage<ImpactivMessage>(data, DateTime.UtcNow.AddMonths(-5), DateTime.UtcNow, 0, 20, 0, 5);
            _logger.LogTrace($"{_logPrefix}::{artifactId}::Randomized data to update template's values before sending the message.");

            if (data != null)
                return JsonConvert.SerializeObject(data, Formatting.Indented);
            else
                return null;
        }

        public async Task<string> GetRandomizedMessageAsync(string deviceId, string moduleId, int peoplePresent)
        {
            string artifactId = string.IsNullOrEmpty(moduleId) ? deviceId : moduleId;
            _logger.LogTrace($"{_logPrefix}::GetRandomizedMessageAsync::{artifactId}.");

            ImpactivMessage data = MessageFactory.CreateMessage<ImpactivMessage>(artifactId);

            //Randomize data
            data = MessageFactory.RandomizeMessage<ImpactivMessage>(data, DateTime.UtcNow.AddMonths(-5), DateTime.UtcNow, 0, 10, 0, peoplePresent);
            _logger.LogTrace($"{_logPrefix}::{artifactId}::Randomized data to update template's values before sending the message.");

            if (data != null)
                return JsonConvert.SerializeObject(data, Formatting.Indented);
            else
                return null;
        }
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
