namespace FS_BMK_ui.ViewModels
{
    using System;
    using FS_BMK_ui.Models;
    using System.Diagnostics;
    using System.Windows.Input;
    using System.Collections.Generic;
    using FS_BMK_ui.Commands;
    using System.Windows;

    internal class OptimizationSuspensionViewModel
    {

        // private members
        private OptimizationSuspension optimizationSuspension;


        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class
        /// </summary>
        public OptimizationSuspensionViewModel()
        {
            optimizationSuspension = new OptimizationSuspension();
        }


        /// <summary>
        /// Gets the customer instance
        /// </summary>
        public OptimizationSuspension OptimizationSuspension
        {
            get
            {
                return optimizationSuspension;
            }
        }

        private ICommand _OptimiseCommand;
        public ICommand OptimiseCommand
        {
            get
            {
                if (_OptimiseCommand == null)
                {
                    _OptimiseCommand = new RelayCommand(OptimiseExecute, CanOptimiseExecute, false);
                }
                return _OptimiseCommand;
            }
        }


        private void OptimiseExecute(object parameter)
        {
            MessageBox.Show($"executed optimise button xval high {OptimizationSuspension.Hardpoints[0].XValHigh} " +
                $"xval low {OptimizationSuspension.Hardpoints[0].XValLow}");
        }

        private bool CanOptimiseExecute(object parameter)
        {
            if (OptimizationSuspension.CompareLowHighValues())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
