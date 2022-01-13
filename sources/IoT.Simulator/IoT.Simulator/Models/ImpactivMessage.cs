using Newtonsoft.Json;

using System;

namespace IoT.Simulator.Models
{
    public class ImpactivMessage
    {
        [JsonProperty("time")]
        public DateTime Time { get; set; }

        [JsonProperty("iso_time")]
        public DateTime ISOTime { get; set; }

        [JsonProperty("sensor_id")]
        public string DeviceId { get; set; }

        [JsonProperty("nb_in")]
        public int CounterPeopleIn { get; set; }

        [JsonProperty("nb_out")]
        public int CounterPeopleOut { get; set; }
    }
}
