using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar.Scripts
{
    public static class NotificationManager
    {
        private static List<NotificationWindow> _activeNotifications = new List<NotificationWindow>();
        private static double _verticalOffset = 0;
        private const double NotificationHeight = 120;
        private const double MarginBetween = 10;

        public static void ShowNotification(string message)
        {

            if (!Properties.Settings.Default.NotificationsEnabled)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = new NotificationWindow(message);
                notification.Closed += (s, e) =>
                {
                    _activeNotifications.Remove(notification);
                    UpdatePositions();
                };

                _activeNotifications.Add(notification);
                notification.Show();
                UpdatePositions();
            });
        }

        private static void UpdatePositions()
        {
            double currentOffset = 0;
            foreach (var notification in _activeNotifications)
            {
                notification.Top = SystemParameters.WorkArea.Height - NotificationHeight - 10 - currentOffset;
                currentOffset += NotificationHeight + MarginBetween;
            }
            _verticalOffset = currentOffset;
        }
    }
}
