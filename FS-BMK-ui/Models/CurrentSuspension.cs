namespace FS_BMK_ui.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class CurrentSuspension : INotifyPropertyChanged
    {
        private string _Name;
        private string _hardpointName;

        public class Hardpoints
        {
            private string _hardpointName;
            private float _xValLow;
            private float _yValLow;
            private float _zValLow;
            private float _xValHigh;
            private float _yValHigh;
            private float _zValHigh;



            public Hardpoints(string hardpointName, 
                float xValLow, float xValHigh, 
                float yValLow, float yValHigh, 
                float zValLow, float zValHigh)
            {
                HardpointName = hardpointName;
                XValLow = xValLow;
                YValLow = yValLow;
                ZValLow = zValLow;
                XValHigh = xValHigh;
                YValHigh = yValHigh;
                ZValHigh = zValHigh;
            }


            public string HardpointName
            {
                get { return _hardpointName; }
                set { _hardpointName = value; }
            }

            public float XValLow
            { 
                get { return XValLow; }
                set { _xValLow = value; }
            }
            public float YValLow
            {
                get { return YValLow; }
                set { _yValLow = value; }
            }
            public float ZValLow
            {
                get { return ZValLow; }
                set { _zValLow = value; }
            }
            public float XValHigh
            {
                get { return XValHigh; }
                set { _xValHigh = value; }
            }
            public float YValHigh
            {
                get { return YValHigh; }
                set { _yValHigh = value; }
            }
            public float ZValHigh
            {
                get { return ZValHigh; }
                set { _zValHigh = value; }
            }

        }

        /// <summary>
        /// Initializes a new instance of the CurrentSuspension class;
        /// </summary>
        public CurrentSuspension(string hardpointName, string customerName)
        {
            HardpointName = hardpointName;
            Name = customerName;
        }

        public string HardpointName
        {
            get { return _hardpointName; }
            set 
            { 
                _hardpointName = value; }
            }

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
