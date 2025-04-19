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
        public DateTime Date { get; set; }
        public ObservableCollection<FootballEvent> Events { get; set; }
        public bool IsCurrentMonth { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
            }
        }

        public DayViewModel(DateTime date)
        {
            Date = date;
            Events = new ObservableCollection<FootballEvent>();
        }
    }
}
