using Calendar.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calendar
{

    public partial class AddEventWindow : Window
    {
        private readonly FootballEvent _editingEvent;

        public string TitleText => TitleBox.Text;
        public string TimeText => TimeBox.Text;

        public string TagText
        {
            get
            {
                var selectedItem = TagComboBox.SelectedItem as ComboBoxItem;
                return selectedItem?.Content.ToString(); 
            }
        }

        public FootballEvent ResultEvent { get; private set; }

        public AddEventWindow(FootballEvent eventToModify = null)
        {
            InitializeComponent();

            if (eventToModify != null)
            {
                _editingEvent = eventToModify;

                TitleBox.Text = _editingEvent.Title;
                TimeBox.Text = _editingEvent.Time.ToString(@"hh\:mm");

                foreach (ComboBoxItem item in TagComboBox.Items)
                {
                    if ((string)item.Content == _editingEvent.Tag)
                    {
                        TagComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TitleText))
            {
                MessageBox.Show("Please enter a title for the event.");
                return;
            }

            if (TagComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tag.");
                return;
            }

            if (!IsValidTime(TimeBox.Text))
            {
                MessageBox.Show("Please enter a valid time in HH:mm format.");
                return;
            }

            var parsedTime = TimeSpan.Parse(TimeBox.Text);

            ResultEvent = new FootballEvent
            {
                Id = _editingEvent?.Id ?? 0, 
                Title = TitleText,
                Time = parsedTime,
                Tag = TagText,
                Date = _editingEvent?.Date ?? DateTime.Today
            };

            DialogResult = true;
            Close();
        }

        private bool IsValidTime(string time)
        {
            string pattern = @"^([0-1][0-9]|2[0-3]):([0-5][0-9])$";
            return Regex.IsMatch(time, pattern);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
