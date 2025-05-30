
using Calendar.Commands;
using Calendar.Scripts;
using Calendar.Scripts.API;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Calendar.Scripts
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _notificationsEnabled = true;
        private readonly IEventRepository _repository;

        public IEventRepository Repository => _repository;

        private readonly MLBApiService _mlbApiService;
        private readonly EventNotificationService _eventNotifier;
        private readonly ThemeService _themeService = new ThemeService();

        private DateTime _selectedDate = DateTime.Today;
        private string _selectedTagFilter = "All";
        private string _selectedTeamTheme;
        private DayViewModel _selectedDay;

        private readonly DispatcherTimer _notificationTimer;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DayViewModel> Days { get; private set; }
        //public ObservableCollection<FootballEvent> FilteredEvents { get; private set; }
        private ObservableCollection<MLBGame> _mlbGames;

        public List<string> TagFilters { get; } = new List<string> { "All", "Match", "Training" };
        public ObservableCollection<MLBGame> SelectedMLBGames { get; private set; } = new ObservableCollection<MLBGame>();

        public ObservableCollection<string> AvailableThemes { get; } = new ObservableCollection<string>
        {
            "Default",
            "RedSox",
            "Yankees",
            "Padres",
            "Rockies"
        };

        public string SelectedTeamTheme
        {
            get => _selectedTeamTheme;
            set
            {
                if (SetProperty(ref _selectedTeamTheme, value))
                {
                    _themeService.ApplyTheme(value);
                    Properties.Settings.Default.SelectedTeamTheme = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (SetProperty(ref _notificationsEnabled, value))
                {
                    Properties.Settings.Default.NotificationsEnabled = value;
                    Properties.Settings.Default.Save();

                    if (value)
                    {
                        _eventNotifier.ResetNotifiedEvents(); 
                        _eventNotifier.CheckUpcomingEvents(); 
                    }
                }
            }
        }

        public ObservableCollection<MLBGame> MLBGames
        {
            get => _mlbGames;
            set => SetProperty(ref _mlbGames, value);
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

        public MainViewModel()
        {
            _repository = EventRepository.Instance;
            _mlbApiService = new MLBApiService();
            _eventNotifier = new EventNotificationService(_repository);

            Days = new ObservableCollection<DayViewModel>();
            SelectedMLBGames = new ObservableCollection<MLBGame>();

            SetupCommands();
            GenerateMonthDays();
            LoadMLBGamesForSelectedDate();

            _eventNotifier.OnNotification += NotificationManager.ShowNotification;

            _notificationTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _notificationTimer.Tick += (s, e) => { if (NotificationsEnabled) _eventNotifier.CheckUpcomingEvents(); };
            _notificationTimer.Start();

            _eventNotifier.CheckUpcomingEvents();
            

            SelectedTeamTheme = Properties.Settings.Default.SelectedTeamTheme ?? "Default";
            NotificationsEnabled = Properties.Settings.Default.NotificationsEnabled;
            _themeService.ApplyTheme(SelectedTeamTheme);
        }

        /// <summary>
        /// Initializes all ICommand properties.
        /// </summary>
        private void SetupCommands()
        {
            AddEventCommand = new AddEventCommand(this);
            ChangeSelectedDateCommand = new ChangeSelectedDateCommand(this);
            PreviousMonthCommand = new PreviousMonthCommand(this);
            NextMonthCommand = new NextMonthCommand(this);
            ModifyEventCommand = new ModifyEventCommand(this);
            DeleteEventCommand = new DeleteEventCommand(this);
        }

        /// <summary>
        /// Asynchronously loads MLB games for the currently SelectedDate.
        /// and populates the SelectedMLBGames collection.
        /// </summary>
        public async void LoadMLBGamesForSelectedDate()
        {
            SelectedMLBGames.Clear();
            var mlbGames = await _mlbApiService.GetUpcomingGamesAsync(SelectedDate, SelectedDate);
            foreach (var game in mlbGames)
                SelectedMLBGames.Add(game);
        }

        /// <summary>
        /// Generates the DayViewModelobjects for the month of the SelectedDate.
        /// Populates the Days collection and applies any active tag filters.
        /// Sets the SelectedDay based on SelectedDate.
        /// </summary>
        public void GenerateMonthDays()
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

        public void CleanUp() => _notificationTimer.Stop();

        /// <summary>
        /// Helper method for setting a property value and raising the PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="storage">Reference to the backing field of the property.</param>
        /// <param name="value">The new value for the property.</param>
        /// <param name="propertyName">The name of the property. Automatically determined by the compiler.</param>
        /// <returns>True if the value was changed.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises the PropertyChnaged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed. Automatically determined if called from a property setter.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}