using Calendar.Scripts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calendar.Scripts
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public List<string> TagFilters { get; } = new List<string> { "All", "Match", "Training" };

        private readonly IEventRepository _repository;

        public ObservableCollection<DayViewModel> Days { get; set; }
        private ObservableCollection<FootballEvent> _allEvents = new ObservableCollection<FootballEvent>();
        public ObservableCollection<FootballEvent> FilteredEvents { get; set; }

        private DateTime _selectedDate = DateTime.Today;

        private DayViewModel _selectedDay;

        public event PropertyChangedEventHandler PropertyChanged;
        private string _selectedTagFilter = "All";

        public ICommand ChangeSelectedDateCommand { get; set; }
        public ICommand AddEventCommand { get; set; }
        public ICommand PreviousMonthCommand { get; set; }
        public ICommand NextMonthCommand { get; set; }
        public ICommand ModifyEventCommand { get; set; }
        public ICommand DeleteEventCommand { get; set; }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string SelectedTagFilter
        {
            get { return _selectedTagFilter; }
            set
            {
                if (_selectedTagFilter != value)
                {
                    _selectedTagFilter = value;
                    OnPropertyChanged(nameof(SelectedTagFilter));
                    GenerateMonthDays();
                }
            }
        }

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
            PreviousMonthCommand = new RelayCommand(() => SelectedDate = SelectedDate.AddMonths(-1));
            NextMonthCommand = new RelayCommand(() => SelectedDate = SelectedDate.AddMonths(1));
            ModifyEventCommand = new RelayCommand(ModifyEvent);
            DeleteEventCommand = new RelayCommand(DeleteEvent);
        }

        private void ModifyEvent(object parameter)
        {
            var eventToModify = parameter as FootballEvent;
            if (eventToModify != null)
            {
                
                var editWindow = new AddEventWindow(eventToModify); 
                var result = editWindow.ShowDialog();

                if (result == true && editWindow.ResultEvent != null)
                {
                    _repository.UpdateEvent(editWindow.ResultEvent);
                    GenerateMonthDays();
                }
            }
        }

        private void DeleteEvent(object parameter)
        {
            var eventToDelete = parameter as FootballEvent;
            if (eventToDelete != null)
            {
                _repository.DeleteEvent(eventToDelete);
                GenerateMonthDays();
            }
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
    var daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);

    for (int day = 1; day <= daysInMonth; day++)
    {
        var date = new DateTime(SelectedDate.Year, SelectedDate.Month, day);
        var dayViewModel = new DayViewModel(date)
        {
            IsCurrentMonth = true
        };

        var dayEvents = _repository.GetEvents(date);
        
        if (SelectedTagFilter != "All")
        {
            dayEvents = dayEvents.Where(e => 
                e.Tag.Equals(SelectedTagFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        foreach (var ev in dayEvents)
        {
            dayViewModel.Events.Add(ev);
        }

        Days.Add(dayViewModel);

        if (date.Date == SelectedDate.Date)
            SelectedDay = dayViewModel;
    }

    foreach (var day in Days)
    {
        day.IsSelected = day.Date.Date == SelectedDate.Date;
    }
        }

        private void AddEvent()
        {
            var window = new AddEventWindow();
            if (window.ShowDialog() == true)
            {
                if (TimeSpan.TryParse(window.TimeText, out TimeSpan parsedTime))
                {
                    var newEvent = new FootballEvent
                    {
                        Date = SelectedDate,
                        Time = parsedTime,
                        Title = window.TitleText,
                        Tag = window.TagText
                    };

                    _repository.AddEvent(newEvent);
                    GenerateMonthDays();
                }
                else
                {
                    MessageBox.Show("Invalid time format. Use HH:mm.");
                }
            }
        }
    }
}
