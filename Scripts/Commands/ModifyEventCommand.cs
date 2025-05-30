using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    public class ModifyEventCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public ModifyEventCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.</param>
        /// <returns>true if parameter is a FootballEvent.</returns>
        public bool CanExecute(object parameter) => parameter is FootballEvent;

        /// <summary>
        /// Executes the command.
        /// Opens the AddEventWindow pre-filled with the event's details for modification.
        /// Updates the event in the repository if changes are confirmed.
        /// </summary>
        /// <param name="parameter">The FootballEvent to modify.</param>
        public void Execute(object parameter)
        {
            if (parameter is FootballEvent eventToModify)
            {
                var editWindow = new AddEventWindow(eventToModify);
                if (editWindow.ShowDialog() == true && editWindow.ResultEvent != null)
                {
                    _viewModel.Repository.UpdateEvent(editWindow.ResultEvent);
                    _viewModel.GenerateMonthDays();
                }
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}