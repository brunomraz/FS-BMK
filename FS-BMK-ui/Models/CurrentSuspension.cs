namespace FS_BMK_ui.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class CurrentSuspension : INotifyPropertyChanged
    {
        private string _Name;
        private string _hardpointName;
        public List<Hardpoint> Hardpoints = new List<Hardpoint> { 
            new Hardpoint("LCA1", 100,120,-600,-550,100,120),
            new Hardpoint("LCA2", 100,120,-600,-550,100,120),
            new Hardpoint("LCA3", 100,120,-600,-550,100,120),
            new Hardpoint("UCA1", 100,120,-600,-550,100,120),
            new Hardpoint("UCA2", 100,120,-600,-550,100,120),
            new Hardpoint("UCA3", 100,120,-600,-550,100,120),
            new Hardpoint("TR1", 100,120,-600,-550,100,120),
            new Hardpoint("TR2", 100,120,-600,-550,100,120),
            new Hardpoint("WCN", 100,120,-600,-550,100,120),
            new Hardpoint("SPN", 100,120,-600,-550,100,120),

        };

        public class Hardpoint
        {
            private string _hardpointName;
            private int _xValLow;
            private int _yValLow;
            private int _zValLow;
            private int _xValHigh;
            private int _yValHigh;
            private int _zValHigh;



            public Hardpoint(string hardpointName, 
                int xValLow, int xValHigh, 
                int yValLow, int yValHigh, 
                int zValLow, int zValHigh)
            {
                HardpointNameClass = hardpointName;
                XValLow = xValLow;
                YValLow = yValLow;
                ZValLow = zValLow;
                XValHigh = xValHigh;
                YValHigh = yValHigh;
                ZValHigh = zValHigh;
            }


            public string HardpointNameClass
            {
                get { return _hardpointName; }
                set { _hardpointName = value; }
            }

            public int XValLow
            { 
                get { return _xValLow; }
                set { _xValLow = value; }
            }
            public int YValLow
            {
                get { return _yValLow; }
                set { _yValLow = value; }
            }
            public int ZValLow
            {
                get { return _zValLow; }
                set { _zValLow = value; }
            }
            public int XValHigh
            {
                get { return _xValHigh; }
                set { _xValHigh = value; }
            }
            public int YValHigh
            {
                get { return _yValHigh; }
                set { _yValHigh = value; }
            }
            public int ZValHigh
            {
                get { return _zValHigh; }
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
