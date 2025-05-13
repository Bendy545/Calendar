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

        public bool CanExecute(object parameter) => parameter is FootballEvent;

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