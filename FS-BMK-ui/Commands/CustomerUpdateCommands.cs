namespace FS_BMK_ui.Commands
{
    using System;
    using System.Windows.Input;
    using FS_BMK_ui.ViewModels;

    internal class CustomerUpdateCommand : ICommand
    {

        public CustomerUpdateCommand(OptimizationSuspensionViewModel viewModel)
        {
            _ViewModel = viewModel;
        }

        private OptimizationSuspensionViewModel _ViewModel;

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _ViewModel.CanUpdate;
        }

        public void Execute(object parameter)
        {
            _ViewModel.SaveChanges();
        }
        #endregion
    }
}
