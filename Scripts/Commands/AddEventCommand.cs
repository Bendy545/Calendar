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

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command</param>
        /// <returns>true if the command can be executed.</returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Executes the command.
        /// Opens the AddEventWindow to gather event details.
        /// </summary>
        /// <param name="parameter">This parameter is not used</param>
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