using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FS_BMK_ui.Models
{
    class CurrentSuspension : INotifyPropertyChanged
    {
        public class Hardpoint
        {
            private string _hardpointName;
            private int _xVal;
            private int _yVal;
            private int _zVal;




            public Hardpoint(string hardpointName,
                int xVal,int yVal,
                int zVal)
            {
                HardpointNameClass = hardpointName;
                XVal = xVal;
                YVal = yVal;
                ZVal = zVal;

            }

            public string HardpointNameClass
            {
                get { return _hardpointName; }
                set { _hardpointName = value; }
            }

            public int XVal
            {
                get { return _xVal; }
                set { _xVal = value; }
            }
            public int YVal
            {
                get { return _yVal; }
                set { _yVal = value; }
            }
            public int ZVal
            {
                get { return _zVal; }
                set { _zVal = value; }
            }
        }

        private List<Hardpoint> _hardpoints = new List<Hardpoint> {
            new Hardpoint("LCA1", 100,120,-600),
            new Hardpoint("LCA2", 100,120,-600),
            new Hardpoint("LCA3", 100,120,-600),
            new Hardpoint("UCA1", 100,120,-600),
            new Hardpoint("UCA2", 100,120,-600),
            new Hardpoint("UCA3", 100,120,-600),
            new Hardpoint("TR1", 100,120,-600),
            new Hardpoint("TR2", 100,120,-600),
            new Hardpoint("WCN", 100,120,-600),
            new Hardpoint("SPN", 100,120,-600)

        };

        private float[] _hardpointsMoved = new float[30];



        public float[] HardpointsMoved
        {
            get
            {
                return _hardpointsMoved;
            }
            set
            {
                _hardpointsMoved = value;

            }
        }




        private List<WPFFloat> _suspensionCharacteristics = new List<WPFFloat>
        {
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat(),
            new WPFFloat()
        };


        public List<WPFFloat> SuspensionCharacteristics { 
            get { 
                return _suspensionCharacteristics;
            } 
            set
            {
                _suspensionCharacteristics = value;

            }
        }

        public List<Hardpoint> Hardpoints
        {
            get { return _hardpoints; }
        }

        private float[] testArray = new float[] { 0f, 0f, 0f };

        private float _wheelRadius = 210f;
        private float _wheelbase = 1530f;
        private float _cogHeight = 300f;
        private float _frontDriveBias = 0f;
        private float _frontBrakeBias = 0.6f;
        private float _rearDriveBias;
        private float _rearBrakeBias;

        private float _verticalMovement = 0f;


        public float WheelRadius
        {
            get { return _wheelRadius; }
            set { _wheelRadius = value; }
        }

        public float Wheelbase
        {
            get { return _wheelbase; }
            set { _wheelbase = value; }
        }
        public float CoGHeight
        {
            get { return _cogHeight; }
            set { _cogHeight = value; }
        }
        public float FrontDriveBias
        {
            get { return _frontDriveBias; }
            set
            {
                _frontDriveBias = value;
                OnPropertyChanged("RearDriveBias");
            }
        }
        public float FrontBrakeBias
        {
            get { return _frontBrakeBias; }
            set
            {
                _frontBrakeBias = value;
                OnPropertyChanged("RearBrakeBias");

            }
        }
        public float RearDriveBias
        {
            get { return _rearDriveBias = 1 - _frontDriveBias; }
            set
            {
                _rearDriveBias = value;
            }
        }
        public float RearBrakeBias
        {
            get { return _rearBrakeBias = 1 - _frontBrakeBias; }
            set
            {
                _rearBrakeBias = value;
            }
        }
        public float VerticalMovement { get { return _verticalMovement; } set { _verticalMovement = value; } }


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

    public class WPFFloat : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private float _Value;
        public float Value
        {
            get { return _Value; }
            set { _Value = value; OnPropertyChanged("Value"); }
        }

        void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
