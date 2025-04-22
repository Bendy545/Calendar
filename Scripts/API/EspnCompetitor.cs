using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    [JsonObject]
    public class EspnCompetitor
    {
        [JsonProperty("homeAway")]
        public string HomeAway { get; set; }

        [JsonProperty("score")]
        public string Score { get; set; }

        [JsonProperty("team")]
        public EspnTeam Team { get; set; }
    }
}
