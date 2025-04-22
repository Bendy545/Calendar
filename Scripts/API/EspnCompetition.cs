using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    [JsonObject]
    public class EspnCompetition
    {
        [JsonProperty("competitors")]
        public List<EspnCompetitor> Competitors { get; set; } = new List<EspnCompetitor>();

        [JsonProperty("venue")]
        public EspnVenue Venue { get; set; }

        [JsonProperty("broadcasts")]
        public List<EspnBroadcast> Broadcasts { get; set; } = new List<EspnBroadcast>();
    }
}
