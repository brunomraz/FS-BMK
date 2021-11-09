namespace FS_BMK_ui.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    public class OptimizationSuspension : INotifyPropertyChanged
    {

        private Hardpoint[] _hardpoints = new Hardpoint[10] {
            new Hardpoint("LCA1", -2040.1f,0,-420,-400,-140,-130, false, true,true),
            new Hardpoint("LCA2", -2240,0,-420,-400,-140,-120, false, true,true),
            new Hardpoint("LCA3", -2140,-2130,-610,-590,-150,-130,true,true,true),
            new Hardpoint("UCA1", -2040,0,-420,-400,-280,-270,false,true,true),
            new Hardpoint("UCA2", -2240,0,-420,-400,-275,-265, false, true,true),
            new Hardpoint("UCA3", -2160,-2150,-590,-570,-320,-310,true,true,true),
            new Hardpoint("TR1", -2240,-2230,-420,-400,-200,-190,true,true,true),
            new Hardpoint("TR2", -2240,-2210,-600,-570,-230,-210,true,true,true),
            new Hardpoint("WCN", -2143.6f,0,-620.5f,0,-220.07f,0,false,false,false),
            new Hardpoint("SPN", -2143.6f,0,-595.5f,0,-219.34f,0,false,false,false)



            //new Hardpoint("LCA1", 1,0,2,3,4,5, false, true,true),
            //new Hardpoint("LCA2", 6,0,7,8,9,10, false, true,true),
            //new Hardpoint("LCA3", 11,12,13,14,15,16,true,true,true),
            //new Hardpoint("UCA1", 17,0,18,19,20,21,false,true,true),
            //new Hardpoint("UCA2", 22,0,23,24,25,26, false, true,true),
            //new Hardpoint("UCA3", 27,28,29,30,31,32,true,true,true),
            //new Hardpoint("TR1", 33,34,35,36,37,38,true,true,true),
            //new Hardpoint("TR2", 39,40,41,42,43,44,true,true,true),
            //new Hardpoint("WCN", 45,0,46,0,47,0,false,false,false),
            //new Hardpoint("SPN", 48,0,49,0,50,0,false,false,false)

        };

        private float[] _hardpointLimits = new float[50];

        private List<float> _suspensionFeatureLimits = new List<float>
        {
            -2.65f, -1f,               // 0, 1,   camber angle down, camber up
            -0.08f, 0f,               // 2, 3,   toe angle down pos low high lim
            0f, 0.05f,                // 4, 5,   toe angle up pos low high lim
            4f, 7f,                  // 6, 7,   caster angle low high lim
            50f, 65f,                 // 8, 9,   roll centre height low lim high lim
            15f, 25f,                 // 10, 11, caster trail low high lim
            -15f, -7f,                 // 12, 13, scrub radius low high lim
            4f, 8f,                   // 14, 15, kingpin angle low high lim
            10f, 18f,                 // 16, 17, anti drive low high lim
            0f, 10f,                  // 18, 19, anti brake low high lim
            -10f, 0f,                 // 20, 21, half track change down pos low high lim
            0f, 3f,                   // 22, 23, half track change up pos low high lim
            -1.5f, 1.5f,              // 24, 25, wheelbase change down pos low high lim
            -1.5f, 1.5f,              // 26, 27, wheelbase change up pos low high lim
            60f, 100f,                // 28, 29, inside wheel free radius LCA3 low high lim
            60f, 100f,                // 30, 31, inside wheel free radius UCA3 low high lim
            60f, 100f,                // 32, 33, inside wheel free radius TR2 low high lim
            -100f, -20f,              // 34, 35, distance from LCA3 to plane defined by WCN and line WCN-SPN
            -100f, -20f,              // 36, 37, distance from UCA3 to plane defined by WCN and line WCN-SPN
            -100f, -20f,              // 38, 39  distance from TR2 to plane defined by WCN and line WCN-SPN
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
        private int _brakesPos = 0;  // outboard or inboard brakes 0 for outboard, 1 for inboard
        private float _verticalMovement = 30f;
        private int _coreNum = 5;
        private float _optimisationDuration = 10f;
        private float _objFunctionPeakWidth = 10f;

        public float ObjFunctionPeakWidth
        {
            get { return _objFunctionPeakWidth; }
            set { _objFunctionPeakWidth = value; }
        }

        public float[] HardpointLimits
        {
            get { return _hardpointLimits; }
        }

        public int CoreNum
        {
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
        public Hardpoint[] Hardpoints
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
            private float _xValLow;
            private float _yValLow;
            private float _zValLow;
            private float _xValHigh;
            private float _yValHigh;
            private float _zValHigh;
            private bool _xIsEditable;
            private bool _yIsEditable;
            private bool _zIsEditable;

            public Hardpoint(string hardpointName,
                float xValLow, float xValHigh,
                float yValLow, float yValHigh,
                float zValLow, float zValHigh,
                bool xIsEditable,
                bool yIsEditable,
                bool zIsEditable
                )
            {
                HardpointNameClass = hardpointName;
                XValLow = xValLow;
                YValLow = yValLow;
                ZValLow = zValLow;
                XValHigh = xValHigh;
                YValHigh = yValHigh;
                ZValHigh = zValHigh;
                _xIsEditable = xIsEditable;
                _yIsEditable = yIsEditable;
                _zIsEditable = zIsEditable;

            }


            public bool XIsEditable { get { return _xIsEditable; } }
            public bool YIsEditable { get { return _yIsEditable; } }
            public bool ZIsEditable { get { return _zIsEditable; } }

            public string HardpointNameClass
            {
                get { return _hardpointName; }
                set { _hardpointName = value; }
            }

            public float XValLow
            {
                get { return _xValLow; }
                set { _xValLow = value; }
            }
            public float YValLow
            {
                get { return _yValLow; }
                set { _yValLow = value; }
            }
            public float ZValLow
            {
                get { return _zValLow; }
                set { _zValLow = value; }
            }
            public float XValHigh
            {
                get { return _xValHigh; }
                set { _xValHigh = value; }
            }
            public float YValHigh
            {
                get { return _yValHigh; }
                set { _yValHigh = value; }
            }
            public float ZValHigh
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

            //// checks if hardpoints limits are correctly written, lower value really is lower
            //for (int i = 0; i < Hardpoints.Length; i++)
            //{
            //    if ((Hardpoints[i].XValHigh < Hardpoints[i].XValLow && Hardpoints[i].XIsEditable) ||
            //        (Hardpoints[i].YValHigh < Hardpoints[i].YValLow && Hardpoints[i].YIsEditable) || 
            //        (Hardpoints[i].ZValHigh < Hardpoints[i].ZValLow && Hardpoints[i].ZIsEditable))
            //        return false;
            //}

            //// checks if all other suspension chracteristics limits are correctly written, lower value really is lower
            //for (int i = 0; i < SuspensionFeatureLimits.Count / 2; i++)
            //{
            //    if (SuspensionFeatureLimits[2 * i] > SuspensionFeatureLimits[2 * i + 1])
            //        return false;
            //}
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
