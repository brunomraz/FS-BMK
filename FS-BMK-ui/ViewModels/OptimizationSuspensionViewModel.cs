namespace FS_BMK_ui.ViewModels
{
    using System;
    using FS_BMK_ui.Models;
    using System.Diagnostics;
    using System.Windows.Input;
    using FS_BMK_ui.Commands;
    using System.Collections.Generic;

    internal class OptimizationSuspensionViewModel
    {

        // private members
        private OptimizationSuspension currentSuspension;

        private List<OptimizationSuspension> _suspensions = new List<OptimizationSuspension>
        {
            new OptimizationSuspension("lca1", "10"),
            new OptimizationSuspension("lca2", "20")
        };

        public List<OptimizationSuspension> Suspensions
        {
            get { return _suspensions; }
        }



        public List<OptimizationSuspension.Hardpoint> HardpointsMV
        {
            get { return currentSuspension.Hardpoints; }
        }




        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class
        /// </summary>
        public OptimizationSuspensionViewModel()
        {
            currentSuspension = new OptimizationSuspension("a","David");
            UpdateCommand = new CustomerUpdateCommand(this);
        }

        /// <summary>
        /// gets or sets a System.Boolean value indicating whether the OptimizationSuspension can be updated
        /// </summary>
        public bool CanUpdate
        {
            get
            {
                if (CurrentSuspension == null)
                {
                    return false;
                }
                return !string.IsNullOrWhiteSpace(CurrentSuspension.Name);
            }
        }

        /// <summary>
        /// Gets the customer instance
        /// </summary>
        public OptimizationSuspension CurrentSuspension
        {
            get
            {
                return currentSuspension;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand UpdateCommand
        {
            get;
            private set;
        }

        public void SaveChanges()
        {
            Debug.Assert(false, String.Format($"{CurrentSuspension.Name} was updated."));
        }
    }
}
