using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    [JsonObject]
    public class EspnBroadcast
    {
        [JsonProperty("names")]
        public List<string> Names { get; set; } = new List<string>();
    }
}
