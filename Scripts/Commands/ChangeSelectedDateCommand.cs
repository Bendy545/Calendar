using System;
using System.Linq;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    public class ChangeSelectedDateCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        public ChangeSelectedDateCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter) => parameter is DateTime;

        public void Execute(object parameter)
        {
            if (parameter is DateTime date)
            {
                _viewModel.SelectedDate = date;
                foreach (var day in _viewModel.Days)
                    day.IsSelected = day.Date.Date == date.Date;
                _viewModel.SelectedDay = _viewModel.Days.FirstOrDefault(d => d.Date.Date == date.Date);
                _viewModel.LoadMLBGamesForSelectedDate();
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}