using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private static double _verticalOffset = 0;
        private const double NotificationHeight = 120;
        private const double MarginBetween = 10;

        public string NotificationMessage
        {
            get { return (string)GetValue(NotificationMessageProperty); }
            set { SetValue(NotificationMessageProperty, value); }
        }

        public static readonly DependencyProperty NotificationMessageProperty =
           DependencyProperty.Register("NotificationMessage", typeof(string), typeof(NotificationWindow));

        /// <summary>
        /// Initializes a new instance of the NotificationWindow class.
        /// </summary>
        /// <param name="message">The message to be displayed in the notification.</param>
        public NotificationWindow(string message)
        {
            InitializeComponent();
            NotificationMessage = message;
            DataContext = this;

            Left = SystemParameters.WorkArea.Width - Width - 10;
            Top = SystemParameters.WorkArea.Height - Height - 10 - _verticalOffset;

            _verticalOffset += NotificationHeight + MarginBetween;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(15)
            };
            timer.Tick += (s, e) => { timer.Stop(); CloseNotification(); };
            timer.Start();
        }

        /// <summary>
        /// Handles the Click event of the Close button.
        /// Stops the auto-close timer and closes the notification.
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseNotification();
        }

        /// <summary>
        /// Closes the notification window.
        /// This method ensures any specific cleanup related to closing can be centralized if needed.
        /// </summary>
        private void CloseNotification()
        {
            _verticalOffset -= NotificationHeight + MarginBetween;
            Close();
        }
    }
}
