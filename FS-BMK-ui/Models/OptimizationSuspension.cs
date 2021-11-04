namespace FS_BMK_ui.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    public class OptimizationSuspension : INotifyPropertyChanged
    {

        private List<Hardpoint> _hardpoints = new List<Hardpoint> {
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

        private List<float> _suspensionFeatureLimits = new List<float>
        {
            -2.7f, -2.6f, -1f, -0.9f, // camber angle down low high lim, camber up low high lim
            -0.08f, 0f, 0f, 0.05f,    // toe angle down pos low high lim, toe angle up pos low high lim
            4f, 15f,                  // caster angle low high lim
            50f, 65f,                 // roll centre height low lim high lim
            10f, 25f,                 // caster trail low high lim
            -15f, 8f,                 // scrub radius low high lim
            3f, 8f,                   // kingpin angle low high lim
            10f, 18f,                 // anti drive low high lim
            0f, 20f,                  // anti brake low high lim
            -10f, 0f,                 // half track change down pos low high lim
            -1.5f, 1.5f,              // wheelbase change down pos low high lim
            0f, 3f,                   // half track change up pos low high lim
            -1.5f, 1.5f,              // wheelbase change up pos low high lim
            60f, 100f,                // inside wheel free radius LCA3 low high lim
            60f, 100f,                // inside wheel free radius UCA3 low high lim
            60f, 100f,                // inside wheel free radius TR2 low high lim
            -100f, -20f,              // distance from LCA3 to plane defined by WCN and line WCN-SPN
            -100f, -20f,              // distance from UCA3 to plane defined by WCN and line WCN-SPN
            -100f, -20f,              // distance from TR2 to plane defined by WCN and line WCN-SPN

        };

        private float _wheelRadius = 210f;
        private float _wheelbase = 1530f;
        private float _cogHeight = 300f;
        private float _frontDriveBias = 0f;
        private float _frontBrakeBias = 0.6f;
        private float _rearDriveBias;
        private float _rearBrakeBias;
        private int _suspensionPos = 1;  // front or rear suspension 0 for front, 1 for rear
        private int _drivePos = 1;  // outboard or inboard drive 0 for outboard, 1 for inboard
        private int _brakesPos = 1;  // outboard or inboard brakes 0 for outboard, 1 for inboard
        private float _verticalMovement = 30f;
        private int _coreNum = 1;
        private float _optimisationDuration = 20f;

        public int CoreNum { 
            get { return _coreNum; }
            set { _coreNum = value; }
        }

        public float OptimisationDuration 
        {
            get { return _optimisationDuration; }
            set { _optimisationDuration = value; }
        }

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
        public List<Hardpoint> Hardpoints
        {
            get { return _hardpoints; }

        }
        public List<float> SuspensionFeatureLimits
        {
            get { return _suspensionFeatureLimits; }

        }


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
        /// Initializes a new instance of the OptimizationSuspension class;
        /// </summary>
        public OptimizationSuspension()
        {
            _rearDriveBias = 1 - _frontDriveBias;
            _rearBrakeBias = 1 - _frontBrakeBias;
        }


        public bool CompareLowHighValues()
        {
            for (int i = 0; i < Hardpoints.Count; i++)
            {
                if (Hardpoints[i].XValHigh < Hardpoints[i].XValLow || Hardpoints[i].YValHigh < Hardpoints[i].YValLow || Hardpoints[i].ZValHigh < Hardpoints[i].ZValLow)
                    return false;
            }
            for (int i = 0; i < SuspensionFeatureLimits.Count / 2; i++)
            {
                if (SuspensionFeatureLimits[2 * i] > SuspensionFeatureLimits[2 * i + 1])
                    return false;
            }
            return true;
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
