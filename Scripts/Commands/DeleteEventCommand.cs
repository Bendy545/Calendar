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

        public bool CanExecute(object parameter) => parameter is FootballEvent;

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