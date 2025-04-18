using Calendar.Scripts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calendar.Scripts
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IEventRepository _repository = EventRepository.Instance;

        public ObservableCollection<FootballEvent> Events { get; set; }
        public ICommand AddEventCommand { get; set; }

        private DateTime _selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set { _selectedDate = value; OnPropertyChanged(); LoadEvents(); }
        }

        public MainViewModel()
        {
            Events = new ObservableCollection<FootballEvent>();
            AddEventCommand = new RelayCommand(AddEvent);
            LoadEvents();
        }

        public void LoadEvents()
        {
            Events.Clear();
            foreach (var ev in _repository.GetEvents(SelectedDate))
            {
                Events.Add(ev);
            }
        }

        private void AddEvent()
        {
            var newEvent = new FootballEvent
            {
                Date = SelectedDate,
                Title = "Nový zápas",
                Tag = "zápas"
            };
            _repository.AddEvent(newEvent);
            LoadEvents();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
