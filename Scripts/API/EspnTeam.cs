using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    [JsonObject]
    public class EspnTeam
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
