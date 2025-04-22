using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    [JsonObject]
    public class EspnEvent
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("competitions")]
        public List<EspnCompetition> Competitions { get; set; } = new List<EspnCompetition>();

        [JsonProperty("status")]
        public EspnStatus Status { get; set; }
    }
}
