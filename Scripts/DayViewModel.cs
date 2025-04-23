using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts
{
    public class DayViewModel
    {
        public DateTime Date { get; }
        public ObservableCollection<FootballEvent> Events { get; }
        public bool IsCurrentMonth { get; set; }
        public bool IsSelected { get; set; }

        public DayViewModel(DateTime date)
        {
            Date = date;
            Events = new ObservableCollection<FootballEvent>();
        }
    }
}
