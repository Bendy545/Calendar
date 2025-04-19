using Calendar.Scripts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calendar.Scripts
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IEventRepository _repository;

        public ObservableCollection<DayViewModel> Days { get; set; }
        public ICommand AddEventCommand { get; set; }

        private DateTime _selectedDate = DateTime.Today;
        public ICommand ChangeSelectedDateCommand { get; set; }

        private DayViewModel _selectedDay;
        public DayViewModel SelectedDay
        {
            get => _selectedDay;
            set
            {
                _selectedDay = value;
                OnPropertyChanged();
            }
        }
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                OnPropertyChanged();
                GenerateMonthDays();
            }
        }

        public MainViewModel()
        {
            _repository = EventRepository.Instance;
            Days = new ObservableCollection<DayViewModel>();
            AddEventCommand = new RelayCommand(AddEvent);
            ChangeSelectedDateCommand = new RelayCommand(param => ChangeSelectedDate((DateTime)param)); 
            GenerateMonthDays();
        }

        private void ChangeSelectedDate(DateTime date)
        {
            SelectedDate = date;
            foreach (var day in Days)
            {
                day.IsSelected = day.Date.Date == date.Date;
            }

            SelectedDay = Days.FirstOrDefault(d => d.Date.Date == date.Date);
        }

        private void GenerateMonthDays()
        {
            Days.Clear();
            var firstDay = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
            var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);

            for (int i = 0; i < 42; i++)
            {
                var date = startDate.AddDays(i);
                var day = new DayViewModel(date)
                {
                    IsCurrentMonth = date.Month == SelectedDate.Month
                };

                foreach (var ev in _repository.GetEvents(date))
                {
                    day.Events.Add(ev);
                }

                Days.Add(day);

                if (date.Date == SelectedDate.Date)
                    SelectedDay = day;
            }
        }

        private void AddEvent()
        {
            var newEvent = new FootballEvent
            {
                Date = SelectedDate,
                Time = new TimeSpan(15, 0, 0),
                Title = "Nový zápas",
                Tag = "zápas"
            };

            _repository.AddEvent(newEvent);
            GenerateMonthDays();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
