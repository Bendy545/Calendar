using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Calendar.Scripts
{

    /// <summary>
    /// Manages the display and positioning of multiple NotificationWindow instances.
    /// Ensures notifications are stacked neatly in the work area.
    /// </summary>
    public static class NotificationManager
    {
        private static List<NotificationWindow> _activeNotifications = new List<NotificationWindow>();
        private static double _verticalOffset = 0;
        private const double NotificationHeight = 120;
        private const double MarginBetween = 10;

        /// <summary>
        /// Shows a notification window with the given message, if notifications are enabled.
        /// Manages the position and lifecycle of the notification window.
        /// </summary>
        /// <param name="message">The message to display in the notification.</param>
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

        /// <summary>
        /// Updates the screen positions of all active notifications.
        /// Notifications are stacked vertically from the bottom-right of the primary work area.
        /// </summary>
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
