using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    public class NextMonthCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public NextMonthCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. This parameter is not used.</param>
        /// <returns>true if this command can be executed.</returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Executes the command.
        /// Advances the calendar by one month.
        /// </summary>
        /// <param name="parameter">This parameter is not used.</param>
        public void Execute(object parameter)
        {
            _viewModel.SelectedDate = _viewModel.SelectedDate.AddMonths(1);
        }

        public event EventHandler CanExecuteChanged;
    }
}