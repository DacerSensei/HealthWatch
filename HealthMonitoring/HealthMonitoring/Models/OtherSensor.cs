using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthMonitoring.Models
{
    public class OtherSensor
    {
        [JsonProperty("BloodOxygen")]
        public string BloodOxygen { get; set; }

        [JsonProperty("BloodPressure")]
        public string BloodPressure { get; set; }

        [JsonProperty("BodyComposition")]
        public string BodyComposition { get; set; }
    }
}
