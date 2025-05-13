using System;
using System.Windows;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    public class AddEventCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public AddEventCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var window = new AddEventWindow();
            if (window.ShowDialog() == true)
            {
                if (TimeSpan.TryParse(window.TimeText, out TimeSpan parsedTime))
                {
                    var newEvent = new FootballEvent
                    {
                        Date = _viewModel.SelectedDate,
                        Time = parsedTime,
                        Title = window.TitleText,
                        Tag = window.TagText
                    };

                    _viewModel.Repository.AddEvent(newEvent);
                    _viewModel.GenerateMonthDays();
                }
                else
                {
                    MessageBox.Show("Invalid time format. Use HH:mm.");
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}