namespace FS_BMK_ui.Models
{
    using System.ComponentModel;

    public class Customer : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the Customer class;
        /// </summary>





        public Customer(string customerName)
        {
            Name = customerName;
        }


        private string _Name;



        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");

            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion



    }
}
