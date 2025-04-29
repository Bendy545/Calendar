using Calendar.Scripts;
using Calendar.Scripts.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Calendar.Scripts
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _notificationsEnabled = true;

        private readonly IEventRepository _repository;
        private readonly MLBApiService _mlbApiService;

        private ObservableCollection<FootballEvent> _allEvents = new ObservableCollection<FootballEvent>();
        private DateTime _selectedDate = DateTime.Today;
        private string _selectedTagFilter = "All";
        private DayViewModel _selectedDay;

        private DispatcherTimer _notificationTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DayViewModel> Days { get; private set; }
        public ObservableCollection<FootballEvent> FilteredEvents { get; private set; }
        private ObservableCollection<MLBGame> _mlbGames;

        public List<string> TagFilters { get; } = new List<string> { "All", "Match", "Training" };
        public ObservableCollection<MLBGame> SelectedMLBGames { get; private set; } = new ObservableCollection<MLBGame>();
        private Dictionary<int, bool> _notifiedEvents = new Dictionary<int, bool>();

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (SetProperty(ref _notificationsEnabled, value))
                {
                    if (value) CheckForUpcomingEvents(null, EventArgs.Empty);
                }
            }
        }

        public ObservableCollection<MLBGame> MLBGames
        {
            get { return _mlbGames; }
            set { SetProperty(ref _mlbGames, value); }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                    GenerateMonthDays();
                }
            }
        }

        public string SelectedTagFilter
        {
            get => _selectedTagFilter;
            set
            {
                if (_selectedTagFilter != value)
                {
                    _selectedTagFilter = value;
                    OnPropertyChanged();
                    GenerateMonthDays();
                }
            }
        }

        public DayViewModel SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (_selectedDay != value)
                {
                    _selectedDay = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ChangeSelectedDateCommand { get; private set; }
        public ICommand AddEventCommand { get; private set; }
        public ICommand PreviousMonthCommand { get; private set; }
        public ICommand NextMonthCommand { get; private set; }
        public ICommand ModifyEventCommand { get; private set; }
        public ICommand DeleteEventCommand { get; private set; }
        public ICommand CheckNowCommand { get; private set; }

        public MainViewModel()
        {
            
            _repository = EventRepository.Instance;
            _mlbApiService = new MLBApiService();
            Days = new ObservableCollection<DayViewModel>();
            SelectedMLBGames = new ObservableCollection<MLBGame>();

            AddEventCommand = new RelayCommand(AddEvent);
            ChangeSelectedDateCommand = new RelayCommand(param => ChangeSelectedDate((DateTime)param));
            PreviousMonthCommand = new RelayCommand(() => SelectedDate = SelectedDate.AddMonths(-1));
            NextMonthCommand = new RelayCommand(() => SelectedDate = SelectedDate.AddMonths(1));
            ModifyEventCommand = new RelayCommand(ModifyEvent);
            DeleteEventCommand = new RelayCommand(DeleteEvent);
            CheckNowCommand = new RelayCommand(() =>
            {
                CheckForUpcomingEvents(null, EventArgs.Empty);
            });
            GenerateMonthDays();
            LoadMLBGamesForSelectedDate();

            _notificationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1) 
            };
            _notificationTimer.Tick += (sender, e) =>
            {
                if (NotificationsEnabled) CheckForUpcomingEvents(sender, e);
            };
            _notificationTimer.Start();

            CheckForUpcomingEvents(null, EventArgs.Empty);

        }

        

        private async void LoadMLBGamesForSelectedDate()
        {
            SelectedMLBGames.Clear();

            var mlbGames = await _mlbApiService.GetUpcomingGamesAsync(SelectedDate, SelectedDate);

            foreach (var game in mlbGames)
            {
                SelectedMLBGames.Add(game);
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

        private void ModifyEvent(object parameter)
        {
            var eventToModify = parameter as FootballEvent;
            if (eventToModify == null) return;

            var editWindow = new AddEventWindow(eventToModify);
            var result = editWindow.ShowDialog();

            if (result == true && editWindow.ResultEvent != null)
            {
                _repository.UpdateEvent(editWindow.ResultEvent);
                GenerateMonthDays();
            }
        }

        private void DeleteEvent(object parameter)
        {
            var eventToDelete = parameter as FootballEvent;
            if (eventToDelete == null) return;

            _repository.DeleteEvent(eventToDelete);
            GenerateMonthDays();
        }

        private void ChangeSelectedDate(DateTime date)
        {
            SelectedDate = date;

            foreach (var day in Days)
                day.IsSelected = day.Date.Date == date.Date;

            SelectedDay = Days.FirstOrDefault(d => d.Date.Date == date.Date);

            LoadMLBGamesForSelectedDate();
        }

        private void GenerateMonthDays()
        {
            Days.Clear();

            var firstDay = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(SelectedDate.Year, SelectedDate.Month, day);
                var dayViewModel = new DayViewModel(date) { IsCurrentMonth = true };

                var dayEvents = _repository.GetEvents(date);

                if (_selectedTagFilter != "All")
                {
                    dayEvents = dayEvents
                        .Where(e => string.Equals(e.Tag, _selectedTagFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                foreach (var ev in dayEvents)
                    dayViewModel.Events.Add(ev);

                Days.Add(dayViewModel);

                if (date.Date == SelectedDate.Date)
                    SelectedDay = dayViewModel;
            }

            foreach (var day in Days)
                day.IsSelected = day.Date.Date == SelectedDate.Date;
        }

        private void CheckForUpcomingEvents(object sender, EventArgs e)
        {
            if (!NotificationsEnabled) return;

            var now = DateTime.Now;
            var upcomingThreshold = now.AddMinutes(30);

            var upcomingEvents = _repository.GetEvents(now.Date)
                .Where(evt =>
                    (evt.Date.Date == now.Date && evt.Time >= now.TimeOfDay && evt.Time <= upcomingThreshold.TimeOfDay) ||
                    (evt.Date.Date == now.AddDays(1).Date && evt.Time <= upcomingThreshold.TimeOfDay))
                .ToList();

            foreach (var ev in upcomingEvents)
            {
                if (!_notifiedEvents.ContainsKey(ev.Id))
                {
                    var timeUntilEvent = (ev.Date.Date == now.Date ?
                        ev.Time - now.TimeOfDay :
                        TimeSpan.FromHours(24) - now.TimeOfDay + ev.Time);

                    string message = $"{ev.Title} ({ev.Tag})\n" +
                                   $"Starts in {timeUntilEvent.TotalMinutes:0} minutes at {ev.Time:hh\\:mm}";

                    ShowCustomNotification(message);
                    _notifiedEvents[ev.Id] = true;
                }
            }

            var expiredEvents = _notifiedEvents.Keys
                .Where(id => !upcomingEvents.Any(evt => evt.Id == id))
                .ToList();

            foreach (var id in expiredEvents)
            {
                _notifiedEvents.Remove(id);
            }
        }

        private void ShowCustomNotification(string message)
        {
            NotificationManager.ShowNotification(message);
        }

        public void CleanUp()
        {
            _notificationTimer.Stop();
        }


        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}

