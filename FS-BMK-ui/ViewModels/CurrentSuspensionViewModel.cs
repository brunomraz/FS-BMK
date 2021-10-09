namespace FS_BMK_ui.ViewModels
{
    using System;
    using FS_BMK_ui.Models;
    using System.Diagnostics;
    using System.Windows.Input;
    using FS_BMK_ui.Commands;
    using System.Collections.Generic;

    internal class CurrentSuspensionViewModel
    {
        private CurrentSuspension currentSuspension;

        private List<CurrentSuspension> _suspensions = new List<CurrentSuspension>
        {
            new CurrentSuspension("lca1", "10"),
            new CurrentSuspension("lca2", "20")
        };

        public List<CurrentSuspension> Suspensions
        {
            get { return _suspensions; }
        }



        public List<CurrentSuspension.Hardpoint> HardpointsMV
        {
            get { return currentSuspension.Hardpoints; }
        }




        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class
        /// </summary>
        public CurrentSuspensionViewModel()
        {
            currentSuspension = new CurrentSuspension("a","David");
            UpdateCommand = new CustomerUpdateCommand(this);
        }

        /// <summary>
        /// gets or sets a System.Boolean value indicating whether the CurrentSuspension can be updated
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
        public CurrentSuspension CurrentSuspension
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
