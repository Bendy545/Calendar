using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    public class DeleteEventCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public DeleteEventCommand(MainViewModel viewModel)
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
        /// Deletes the specified FotballEvent from the repository and refreshes the calendar.
        /// </summary>
        /// <param name="parameter">The FotballEvent to delete.</param>
        public void Execute(object parameter)
        {
            if (parameter is FootballEvent eventToDelete)
            {
                _viewModel.Repository.DeleteEvent(eventToDelete);
                _viewModel.GenerateMonthDays();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}