using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts.API
{
    public class MLBResponse
    {
        /// <summary>
        /// List of dates, each containing games.
        /// </summary>
        public List<MLBDate> Dates { get; set; }
    }
}
