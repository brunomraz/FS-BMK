namespace FS_BMK_ui.ViewModels
{
    using System;
    using FS_BMK_ui.Models;
    using System.Diagnostics;
    using System.Windows.Input;
    using FS_BMK_ui.Commands;

    internal class CurrentSuspensionViewModel
    {
        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class
        /// </summary>
        public CurrentSuspensionViewModel()
        {
            _Customer = new Customer("David");
            UpdateCommand = new CustomerUpdateCommand(this);
        }

        /// <summary>
        /// gets or sets a System.Boolean value indicating whether the Customer can be updated
        /// </summary>
        public bool CanUpdate
        {
            get
            {
                if (Customer == null)
                {
                    return false;
                }
                return !string.IsNullOrWhiteSpace(Customer.Name);
            }
        }

        private Customer _Customer;
        /// <summary>
        /// Gets the customer instance
        /// </summary>
        public Customer Customer
        {
            get
            {
                return _Customer;
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
            Debug.Assert(false, String.Format($"{Customer.Name} was updated."));
        }
    }
}
