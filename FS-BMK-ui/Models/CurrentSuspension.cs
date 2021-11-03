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
            private float _xVal;
            private float _yVal;
            private float _zVal;




            public Hardpoint(string hardpointName,
                float xVal, float yVal,
                float zVal)
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

            public float XVal
            {
                get { return _xVal; }
                set { _xVal = value; }
            }
            public float YVal
            {
                get { return _yVal; }
                set { _yVal = value; }
            }
            public float ZVal
            {
                get { return _zVal; }
                set { _zVal = value; }
            }


        }


        private float _wheelRadius = 210f;
        private float _wheelWidth = 200f;
        private float _wheelInsideRadius = 100f;
        private float _wheelbase = 1530f;
        private float _cogHeight = 300f;
        private float _frontDriveBias = 0f;
        private float _frontBrakeBias = 0.6f;
        private float _rearDriveBias;
        private float _rearBrakeBias;
        private float _verticalMovement = 0f;
        private float[] _hardpointsMoved = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private int _suspensionPos = 1; // front or rear suspension 0 for front, 1 for rear
        private int _drivePos = 1;  // outboard or inboard drive 0 for outboard, 1 for inboard
        private int _brakesPos = 1; // outboard or inboard brakes 0 for outboard, 1 for inboard
        private List<Hardpoint> _hardpoints = new List<Hardpoint> {
            new Hardpoint("LCA1", -2038.666f, -411.709f, -132.316f),  // 0
            new Hardpoint("LCA2", -2241.147f, -408.195f, -126.205f),  // 1 
            new Hardpoint("LCA3", -2135f, -600f, -140f),              // 2
            new Hardpoint("UCA1", -2040.563f, -416.249f, -275.203f),  // 3
            new Hardpoint("UCA2", -2241.481f, -417.314f, -270.739f),  // 4
            new Hardpoint("UCA3", -2153f, -578f, -315f),              // 5 
            new Hardpoint("TR1",  -2234.8f, -411.45f, -194.6f),       // 6
            new Hardpoint("TR2",  -2225f, -582f, -220f),              // 7
            new Hardpoint("WCN",  -2143.6f, -620.5f, -220.07f),       // 8
            new Hardpoint("SPN",  -2143.6f, -595.5f, -219.34f)        // 9

        };
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

        public int SuspensionPos
        {
            get { return _suspensionPos; }
            set
            {
                _suspensionPos = value;
                OnPropertyChanged("SuspensionPos");
            }
        }
        public int DrivePos
        {
            get { return _drivePos; }
            set
            {
                _drivePos = value;
                OnPropertyChanged("DrivePos");
            }
        }
        public int BrakesPos
        {
            get { return _brakesPos; }
            set
            {
                _brakesPos = value;
                OnPropertyChanged("BrakesPos");
            }
        }
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
        public List<WPFFloat> SuspensionCharacteristics
        {
            get
            {
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
        public float WheelRadius
        {
            get { return _wheelRadius; }
            set { _wheelRadius = value; }
        }
        public float WheelInsideRadius { get { return _wheelInsideRadius; } set { _wheelInsideRadius = value; } }
        public float WheelWidth { get { return _wheelWidth; } set { _wheelWidth = value; } }
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
