using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    public class MLBGame
    {
        public DateTime Date { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeam { get; set; }
        public string GameTime { get; set; }
    }
}
