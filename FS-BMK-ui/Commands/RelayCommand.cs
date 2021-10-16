using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FS_BMK_ui.Commands
{
    class RelayCommand : ICommand
    {
        Action<object> _executeAction;
        Func<object, bool> _canExecute;
        bool _canExecuteCache;


        public RelayCommand(Action<object> executeAction, Func<object, bool> canExecute, bool canExecuteCache)
        {
            _canExecute = canExecute;
            _executeAction = executeAction;
            _canExecuteCache = canExecuteCache;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;

            }
            else
            {
                return _canExecute(parameter);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {

                CommandManager.RequerySuggested += value;

            }
            remove
            {

                CommandManager.RequerySuggested -= value;

            }
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }
    }
}
