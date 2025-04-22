using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    [JsonObject]
    public class EspnStatus
    {
        [JsonProperty("type")]
        public EspnStatusType Type { get; set; }
    }
}
