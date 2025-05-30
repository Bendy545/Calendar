using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    public class MLBGameDetail
    {

        /// <summary>
        /// Teams involved in the game.
        /// </summary>
        public MLBTeams Teams { get; set; }

        /// <summary>
        /// Official time if the game.
        /// </summary>
        public DateTime GameDate { get; set; }
    }
}
