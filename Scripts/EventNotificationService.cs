using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Scripts
{

    /// <summary>
    /// Service responsible for checking upcoming events and triggering notifications.
    /// </summary>
    public class EventNotificationService
    {
        private readonly IEventRepository _repository;
        private readonly Dictionary<int, bool> _notifiedEvents = new Dictionary<int, bool>();

        public event Action<string> OnNotification;

        public EventNotificationService(IEventRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Checks for upcoming events within a 30-minute threshold and triggers notifications.
        /// It checks events for the current day and the start of the next day.
        /// </summary>
        public void CheckUpcomingEvents()
        {
            var now = DateTime.Now;
            var threshold = now.AddMinutes(30);
            var events = _repository.GetEvents(now.Date)
                .Where(e => ShouldNotify(e, now, threshold)).ToList();

            foreach (var ev in events)
            {
                if (!_notifiedEvents.ContainsKey(ev.Id))
                {
                    var minutes = GetTimeUntilEvent(ev, now);
                    string message = $"{ev.Title} ({ev.Tag})\nStarts in {minutes:0} minutes at {ev.Time:hh\\:mm}";
                    OnNotification?.Invoke(message);
                    _notifiedEvents[ev.Id] = true;
                }
            }

            var expired = _notifiedEvents.Keys.Where(id => !events.Any(e => e.Id == id)).ToList();
            foreach (var id in expired) _notifiedEvents.Remove(id);
        }

        private bool ShouldNotify(FootballEvent ev, DateTime now, DateTime threshold) =>
            (ev.Date.Date == now.Date && ev.Time >= now.TimeOfDay && ev.Time <= threshold.TimeOfDay) ||
            (ev.Date.Date == now.AddDays(1).Date && ev.Time <= threshold.TimeOfDay);

        private double GetTimeUntilEvent(FootballEvent ev, DateTime now)
        {
            return ev.Date.Date == now.Date
                ? (ev.Time - now.TimeOfDay).TotalMinutes
                : (TimeSpan.FromHours(24) - now.TimeOfDay + ev.Time).TotalMinutes;
        }

        public void ResetNotifiedEvents()
        {
            _notifiedEvents.Clear();
        }
    }
}
