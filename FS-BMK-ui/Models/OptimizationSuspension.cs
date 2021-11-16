namespace FS_BMK_ui.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using FS_BMK_ui.HelperClasses;

    public class OptimizationSuspension : INotifyPropertyChanged
    {

        private HardpointLimits[] _hardpointsLimits = new HardpointLimits[10] {
            new HardpointLimits("LCA1", -2040.1f,0,-420,-400,-140,-130, false, true,true),
            new HardpointLimits("LCA2", -2240,0,-420,-400,-140,-120, false, true,true),
            new HardpointLimits("LCA3", -2140,-2130,-610,-590,-150,-130,true,true,true),
            new HardpointLimits("UCA1", -2040,0,-420,-400,-280,-270,false,true,true),
            new HardpointLimits("UCA2", -2240,0,-420,-400,-275,-265, false, true,true),
            new HardpointLimits("UCA3", -2160,-2150,-590,-570,-320,-310,true,true,true),
            new HardpointLimits("TR1", -2240,-2230,-420,-400,-200,-190,true,true,true),
            new HardpointLimits("TR2", -2240,-2210,-600,-570,-230,-210,true,true,true),
            new HardpointLimits("WCN", -2143.6f,0,-620.5f,0,-220.07f,0,false,false,false),
            new HardpointLimits("SPN", -2143.6f,0,-595.5f,0,-219.34f,0,false,false,false)
        };


        private List<float> _suspensionFeatureLimits = new List<float>
        {
            -2.7f, -2.6f,          // 0, 1,     camber angle down pos, low high lim
            -1f, -0.9f,            // 2, 3,     camber angle up pos, low high lim
            -0.08f, 0f,            // 4, 5,     toe angle down pos low high lim
            0f, 0.05f,             // 6, 7,     toe angle up pos low high lim
            4f, 7f,                // 8, 9,     caster angle low high lim
            50f, 65f,              // 10, 11,   roll centre height low lim high lim
            15f, 25f,              // 12, 13,   caster trail low high lim
            -15f, -7f,             // 14, 15,   scrub radius low high lim
            4f, 8f,                // 16, 17,   kingpin angle low high lim
            10f, 18f,              // 18, 19,   anti drive low high lim
            0f, 10f,               // 20, 21,   anti brake low high lim
            -10f, 0f,              // 22, 23,   half track change down pos low high lim
            0f, 3f,                // 24, 25,   half track change up pos low high lim
            -1.5f, 1.5f,           // 26, 27,   wheelbase change down pos low high lim
            -1.5f, 1.5f,           // 28, 29,   wheelbase change up pos low high lim
            60f, 100f,             // 30, 31,   inside wheel free radius LCA3 low high lim
            60f, 100f,             // 32, 33,   inside wheel free radius UCA3 low high lim
            60f, 100f,             // 34, 35,   inside wheel free radius TR2 low high lim
            -100f, -20f,           // 36, 37,   distance from LCA3 to plane defined by WCN and line WCN-SPN
            -100f, -20f,           // 38, 39    distance from UCA3 to plane defined by WCN and line WCN-SPN
            -100f, -20f,           // 40, 41    distance from TR2 to plane defined by WCN and line WCN-SPN
        };

        private float[] _targetCharacteristicValues = { //new float[21];
            -2.65f,          // 0, 1,     camber angle down pos, low high lim
            -0.95f,            // 2, 3,     camber angle up pos, low high lim
            -0f,            // 4, 5,     toe angle down pos low high lim
            0f,             // 6, 7,     toe angle up pos low high lim
            7f,                // 8, 9,     caster angle low high lim
            50f,              // 10, 11,   roll centre height low lim high lim
            15f,              // 12, 13,   caster trail low high lim
            -7f,             // 14, 15,   scrub radius low high lim
            4f,                // 16, 17,   kingpin angle low high lim
            18f,              // 18, 19,   anti drive low high lim
            10f,               // 20, 21,   anti brake low high lim
            0f,              // 22, 23,   half track change down pos low high lim
            0f,                // 24, 25,   half track change up pos low high lim
            0f,           // 26, 27,   wheelbase change down pos low high lim
            0f,           // 28, 29,   wheelbase change up pos low high lim
            100f,             // 30, 31,   inside wheel free radius LCA3 low high lim
            100f,             // 32, 33,   inside wheel free radius UCA3 low high lim
            100f,             // 34, 35,   inside wheel free radius TR2 low high lim
            -20f,           // 36, 37,   distance from LCA3 to plane defined by WCN and line WCN-SPN
            -20f,           // 38, 39    distance from UCA3 to plane defined by WCN and line WCN-SPN
            -20f           // 40, 41    distance from TR2 to plane defined by WCN and line WCN-SPN
        };
        private float[] _featuresWeightFactors = new float[22];
        private float[] _significance = {    // new float[22];
            100f,          // 0, 1,     camber angle down pos, low high lim
            100f,            // 2, 3,     camber angle up pos, low high lim
            5f,            // 4, 5,     toe angle down pos low high lim
            5f,             // 6, 7,     toe angle up pos low high lim
            5f,                // 8, 9,     caster angle low high lim
            2f,              // 10, 11,   roll centre height low lim high lim
            5f,              // 12, 13,   caster trail low high lim
            2f,             // 14, 15,   scrub radius low high lim
            5f,                // 16, 17,   kingpin angle low high lim
            2f,              // 18, 19,   anti drive low high lim
            2f,               // 20, 21,   anti brake low high lim
            2f,              // 22, 23,   half track change down pos low high lim
            2f,                // 24, 25,   half track change up pos low high lim
            2f,           // 26, 27,   wheelbase change down pos low high lim
            2f,           // 28, 29,   wheelbase change up pos low high lim
            2f,             // 30, 31,   inside wheel free radius LCA3 low high lim
            2f,             // 32, 33,   inside wheel free radius UCA3 low high lim
            2f,             // 34, 35,   inside wheel free radius TR2 low high lim
            0f,           // 36, 37,   distance from LCA3 to plane defined by WCN and line WCN-SPN
            0f,           // 38, 39    distance from UCA3 to plane defined by WCN and line WCN-SPN
            0f           // 40, 41    distance from TR2 to plane defined by WCN and line WCN-SPN
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

        private float _camberDownSignificance = 100f;
        private float _camberUpSignificance = 100f;
        private float _toeDownSignificance = 5f;
        private float _toeUpSignificance = 5f;
        private float _casterAngleSignificance = 3f;
        private float _rollCentreHeightSignificance = 2f;
        private float _casterTrailSignificance = 5f;
        private float _scrubRadiusSignificance = 2f;
        private float _kingpinAngleSignificance = 2f;
        private float _antiDriveSignificance = 2f;
        private float _antiBrakeSignificance = 2f;
        private float _halftrackChangeDownSignificance = 2f;
        private float _halftrackChangeUpSignificance = 2f;
        private float _wheelbaseDownSignificance = 2f;
        private float _wheelbaseUpSignificance = 2f;
        private float _lca3InsideWheelSpaceSignificance = 2f;
        private float _uca3InsideWheelSpaceSignificance = 2f;
        private float _tr2InsideWheelSpaceSignificance = 2f;
        private float _lca3wcnDistSignificance = 2f;
        private float _uca3wcnDistSignificance = 2f;
        private float _tr2wcnDistSignificance = 2f;


        public float CamberDownSignificance
        {
            get { return _camberDownSignificance; }
            set { _camberDownSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("CamberDownSignificance"); }
        }
        public float CamberUpSignificance
        {
            get { return _camberUpSignificance; }
            set { _camberUpSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("CamberUpSignificance"); }
        }
        public float ToeDownSignificance
        {
            get { return _toeDownSignificance; }
            set { _toeDownSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("ToeDownSignificance"); }
        }
        public float ToeUpSignificance
        {
            get { return _toeUpSignificance; }
            set { _toeUpSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("ToeUpSignificance"); }
        }
        public float CasterAngleSignificance
        {
            get { return _casterAngleSignificance; }
            set { _casterAngleSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("CasterAngleSignificance"); }
        }
        public float RollCentreHeightSignificance
        {
            get { return _rollCentreHeightSignificance; }
            set { _rollCentreHeightSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("RollCentreHeightSignificance"); }
        }
        public float CasterTrailSignificance
        {
            get { return _casterTrailSignificance; }
            set { _casterTrailSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("CasterTrailSignificance"); }
        }
        public float ScrubRadiusSignificance
        {
            get { return _scrubRadiusSignificance; }
            set { _scrubRadiusSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("ScrubRadiusSignificance"); }
        }
        public float KingpinAngleSignificance
        {
            get { return _kingpinAngleSignificance; }
            set { _kingpinAngleSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("KingpinAngleSignificance"); }
        }
        public float AntiDriveSignificance
        {
            get { return _antiDriveSignificance; }
            set { _antiDriveSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("AntiDriveSignificance"); }
        }
        public float AntiBrakeSignificance
        {
            get { return _antiBrakeSignificance; }
            set { _antiBrakeSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("AntiBrakeSignificance"); }
        }
        public float HalftrackChangeDownSignificance
        {
            get { return _halftrackChangeDownSignificance; }
            set { _halftrackChangeDownSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("HalftrackChangeDownSignificance"); }
        }
        public float HalftrackChangeUpSignificance
        {
            get { return _halftrackChangeUpSignificance; }
            set { _halftrackChangeUpSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("HalftrackChangeUpSignificance"); }
        }
        public float WheelbaseDownSignificance
        {
            get { return _wheelbaseDownSignificance; }
            set { _wheelbaseDownSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("WheelbaseDownSignificance"); }
        }
        public float WheelbaseUpSignificance
        {
            get { return _wheelbaseUpSignificance; }
            set { _wheelbaseUpSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("WheelbaseUpSignificance"); }
        }
        public float Lca3InsideWheelSpaceSignificance
        {
            get { return _lca3InsideWheelSpaceSignificance; }
            set { _lca3InsideWheelSpaceSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("Lca3InsideWheelSpaceSignificance"); }
        }
        public float Uca3InsideWheelSpaceSignificance
        {
            get { return _uca3InsideWheelSpaceSignificance; }
            set { _uca3InsideWheelSpaceSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("Uca3InsideWheelSpaceSignificance"); }
        }
        public float Tr2InsideWheelSpaceSignificance
        {
            get { return _tr2InsideWheelSpaceSignificance; }
            set { _tr2InsideWheelSpaceSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("Tr2InsideWheelSpaceSignificance"); }
        }
        public float Lca3wcnDistSignificance
        {
            get { return _lca3wcnDistSignificance; }
            set { _lca3wcnDistSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("Lca3wcnDistSignificance"); }
        }
        public float Uca3wcnDistSignificance
        {
            get { return _uca3wcnDistSignificance; }
            set { _uca3wcnDistSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("Uca3wcnDistSignificance"); }
        }
        public float Tr2wcnDistSignificance
        {
            get { return _tr2wcnDistSignificance; }
            set { _tr2wcnDistSignificance = value; OnPropertyChanged("SignificanceSum"); OnPropertyChanged("Tr2wcnDistSignificance"); }
        }


        private float _significanceSum;
        public float SignificanceSum
        {
            get
            {
                //return _significanceSum = _camberDownSignificance;
                _significanceSum =
                    _camberDownSignificance + _camberUpSignificance + _toeDownSignificance + _toeUpSignificance +
                    _casterAngleSignificance + _rollCentreHeightSignificance + _casterTrailSignificance + _scrubRadiusSignificance +
                    _kingpinAngleSignificance + _antiDriveSignificance + _antiBrakeSignificance + _halftrackChangeDownSignificance +
                    _halftrackChangeUpSignificance + _wheelbaseDownSignificance + _wheelbaseUpSignificance +
                    _lca3InsideWheelSpaceSignificance + _uca3InsideWheelSpaceSignificance + _tr2InsideWheelSpaceSignificance +
                    _lca3wcnDistSignificance + _uca3wcnDistSignificance + _tr2wcnDistSignificance;

                return _significanceSum;
            }
            set
            {
                _significanceSum = value;
            }
        }

        public float[] Significance { get { return _significance; } }

        public float[] TargetCharacteristicValues { get { return _targetCharacteristicValues; } }

        public float[] FeaturesWeightFactors { get { return _featuresWeightFactors; } }
        public float ObjFunctionPeakWidth { get { return _objFunctionPeakWidth; } set { _objFunctionPeakWidth = value; } }
        public int CoreNum { get { return _coreNum; } set { _coreNum = value; } }
        public float OptimisationDuration { get { return _optimisationDuration; } set { _optimisationDuration = value; } }
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


        public float WheelRadius { get { return _wheelRadius; } set { _wheelRadius = value; } }

        public float Wheelbase { get { return _wheelbase; } set { _wheelbase = value; } }
        public float CoGHeight { get { return _cogHeight; } set { _cogHeight = value; } }
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
            set { _rearDriveBias = value; }
        }
        public float RearBrakeBias
        {
            get { return _rearBrakeBias = 1 - _frontBrakeBias; }
            set { _rearBrakeBias = value; }
        }
        public float VerticalMovement { get { return _verticalMovement; } set { _verticalMovement = value; } }
        public HardpointLimits[] HardpointsLimits { get { return _hardpointsLimits; } }
        public List<float> SuspensionFeatureLimits { get { return _suspensionFeatureLimits; } }



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

            // checks if hardpoints limits are correctly written, lower value really is lower
            for (int i = 0; i < HardpointsLimits.Length; i++)
            {
                if ((HardpointsLimits[i].XValHigh < HardpointsLimits[i].XValLow && HardpointsLimits[i].XIsEditable) ||
                    (HardpointsLimits[i].YValHigh < HardpointsLimits[i].YValLow && HardpointsLimits[i].YIsEditable) ||
                    (HardpointsLimits[i].ZValHigh < HardpointsLimits[i].ZValLow && HardpointsLimits[i].ZIsEditable))
                    return false;
            }

            // checks if all other suspension chracteristics limits are correctly written, lower value really is lower
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
