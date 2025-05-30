using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    public class MLBDate
    {
        /// <summary>
        /// List of game details for this date.
        /// </summary>
        public List<MLBGameDetail> Games { get; set; }
    }
}
