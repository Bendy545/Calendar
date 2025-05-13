using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    public class PreviousMonthCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public PreviousMonthCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _viewModel.SelectedDate = _viewModel.SelectedDate.AddMonths(-1);
        }

        public event EventHandler CanExecuteChanged;
    }
}