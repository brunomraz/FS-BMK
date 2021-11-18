﻿namespace FS_BMK_ui.Models
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
            -2.65f,             // 0,     camber angle down pos, low high lim
            -0.95f,             // 1,     camber angle up pos, low high lim
            -0f,                // 2,     toe angle down pos low high lim
            0f,                 // 3,     toe angle up pos low high lim
            7f,                 // 4,     caster angle low high lim
            50f,                // 5,     roll centre height low lim high lim
            15f,                // 6,     caster trail low high lim
            -7f,                // 7,     scrub radius low high lim
            4f,                 // 8,     kingpin angle low high lim
            18f,                // 9,     anti drive low high lim
            10f,                // 10,    anti brake low high lim
            0f,                 // 11,    half track change down pos low high lim
            0f,                 // 12,    half track change up pos low high lim
            0f,                 // 13,    wheelbase change down pos low high lim
            0f,                 // 14,    wheelbase change up pos low high lim
            100f,               // 15,    inside wheel free radius LCA3 low high lim
            100f,               // 16,    inside wheel free radius UCA3 low high lim
            100f,               // 17,    inside wheel free radius TR2 low high lim
            -20f,               // 18,    distance from LCA3 to plane defined by WCN and line WCN-SPN
            -20f,               // 19,    distance from UCA3 to plane defined by WCN and line WCN-SPN
            -20f                // 20     distance from TR2 to plane defined by WCN and line WCN-SPN
        };

        private float[] _significance =
        {
            100f,               // 0,     camber angle down pos, low high lim
            100f,               // 1,     camber angle up pos, low high lim
            5f,                 // 2,     toe angle down pos low high lim
            5f,                 // 3,     toe angle up pos low high lim
            3f,                 // 4,     caster angle low high lim
            2f,                 // 5,     roll centre height low lim high lim
            5f,                 // 6,     caster trail low high lim
            2f,                 // 7,     scrub radius low high lim
            2f,                 // 8,     kingpin angle low high lim
            2f,                 // 9,     anti drive low high lim
            2f,                 // 10,    anti brake low high lim
            2f,                 // 11,    half track change down pos low high lim
            2f,                 // 12,    half track change up pos low high lim
            2f,                 // 13,    wheelbase change down pos low high lim
            2f,                 // 14,    wheelbase change up pos low high lim
            2f,                 // 15,    inside wheel free radius LCA3 low high lim
            2f,                 // 16,    inside wheel free radius UCA3 low high lim
            2f,                 // 17,    inside wheel free radius TR2 low high lim
            2f,                 // 18,    distance from LCA3 to plane defined by WCN and line WCN-SPN
            2f,                 // 19,    distance from UCA3 to plane defined by WCN and line WCN-SPN
            2f                  // 20     distance from TR2 to plane defined by WCN and line WCN-SPN
        };

        private float[] _weightFactors = new float[21];
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
        private float _significanceSum;

        public float[] WeightFactors { get { return _weightFactors; } }
        public float[] TargetCharacteristicValues { get { return _targetCharacteristicValues; } }

        #region Optimisation characteristics significance properties
        public float CamberDownSignificance
        {
            get { return _significance[0]; }
            set { _significance[0] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float CamberUpSignificance
        {
            get { return _significance[1]; }
            set { _significance[1] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float ToeDownSignificance
        {
            get { return _significance[2]; }
            set { _significance[2] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float ToeUpSignificance
        {
            get { return _significance[3]; }
            set { _significance[3] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float CasterAngleSignificance
        {
            get { return _significance[4]; }
            set { _significance[4] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float RollCentreHeightSignificance
        {
            get { return _significance[5]; }
            set { _significance[5] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float CasterTrailSignificance
        {
            get { return _significance[6]; }
            set { _significance[6] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float ScrubRadiusSignificance
        {
            get { return _significance[7]; }
            set { _significance[7] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float KingpinAngleSignificance
        {
            get { return _significance[8]; }
            set { _significance[8] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float AntiDriveSignificance
        {
            get { return _significance[9]; }
            set { _significance[9] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float AntiBrakeSignificance
        {
            get { return _significance[10]; }
            set { _significance[10] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float HalftrackChangeDownSignificance
        {
            get { return _significance[11]; }
            set { _significance[11] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float HalftrackChangeUpSignificance
        {
            get { return _significance[12]; }
            set { _significance[12] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float WheelbaseDownSignificance
        {
            get { return _significance[13]; }
            set { _significance[13] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float WheelbaseUpSignificance
        {
            get { return _significance[14]; }
            set { _significance[14] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float Lca3InsideWheelSpaceSignificance
        {
            get { return _significance[15]; }
            set { _significance[15] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float Uca3InsideWheelSpaceSignificance
        {
            get { return _significance[16]; }
            set { _significance[16] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float Tr2InsideWheelSpaceSignificance
        {
            get { return _significance[17]; }
            set { _significance[17] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float Lca3wcnDistSignificance
        {
            get { return _significance[18]; }
            set { _significance[18] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float Uca3wcnDistSignificance
        {
            get { return _significance[19]; }
            set { _significance[19] = value; OnPropertyChanged("SignificanceSum"); }
        }
        public float Tr2wcnDistSignificance
        {
            get { return _significance[20]; }
            set { _significance[20] = value; OnPropertyChanged("SignificanceSum"); }
        }
        #endregion


        #region Optimisation characteristics weight factor properties
        public float CamberDownWeightFactor
        {
            get { return _weightFactors[0] = _significance[0] / _significanceSum; }
            set { _weightFactors[0] = value; }
        }
        public float CamberUpWeightFactor
        {
            get { return _weightFactors[1] = _significance[1] / _significanceSum; }
            set { _weightFactors[1] = value; }
        }
        public float ToeDownWeightFactor
        {
            get { return _weightFactors[2] = _significance[2] / _significanceSum; }
            set { _weightFactors[2] = value; }
        }
        public float ToeUpWeightFactor
        {
            get { return _weightFactors[3] = _significance[3] / _significanceSum; }
            set { _weightFactors[3] = value; }
        }
        public float CasterAngleWeightFactor
        {
            get { return _weightFactors[4] = _significance[4] / _significanceSum; }
            set { _weightFactors[4] = value; }
        }
        public float RollCentreHeightWeightFactor
        {
            get { return _weightFactors[5] = _significance[5] / _significanceSum; }
            set { _weightFactors[5] = value; }
        }
        public float CasterTrailWeightFactor
        {
            get { return _weightFactors[6] = _significance[6] / _significanceSum; }
            set { _weightFactors[6] = value; }
        }
        public float ScrubRadiusWeightFactor
        {
            get { return _weightFactors[7] = _significance[7] / _significanceSum; }
            set { _weightFactors[7] = value; }
        }
        public float KingpinAngleWeightFactor
        {
            get { return _weightFactors[8] = _significance[8] / _significanceSum; }
            set { _weightFactors[8] = value; }
        }
        public float AntiDriveWeightFactor
        {
            get { return _weightFactors[9] = _significance[9] / _significanceSum; }
            set { _weightFactors[9] = value; }
        }
        public float AntiBrakeWeightFactor
        {
            get { return _weightFactors[10] = _significance[10] / _significanceSum; }
            set { _weightFactors[10] = value; }
        }
        public float HalftrackChangeDownWeightFactor
        {
            get { return _weightFactors[11] = _significance[11] / _significanceSum; }
            set { _weightFactors[11] = value; }
        }
        public float HalftrackChangeUpWeightFactor
        {
            get { return _weightFactors[12] = _significance[12] / _significanceSum; }
            set { _weightFactors[12] = value; }
        }
        public float WheelbaseDownWeightFactor
        {
            get { return _weightFactors[13] = _significance[13] / _significanceSum; }
            set { _weightFactors[13] = value; }
        }
        public float WheelbaseUpWeightFactor
        {
            get { return _weightFactors[14] = _significance[14] / _significanceSum; }
            set { _weightFactors[14] = value; }
        }
        public float Lca3InsideWheelSpaceWeightFactor
        {
            get { return _weightFactors[15] = _significance[15] / _significanceSum; }
            set { _weightFactors[15] = value; }
        }
        public float Uca3InsideWheelSpaceWeightFactor
        {
            get { return _weightFactors[16] = _significance[16] / _significanceSum; }
            set { _weightFactors[16] = value; }
        }
        public float Tr2InsideWheelSpaceWeightFactor
        {
            get { return _weightFactors[17] = _significance[17] / _significanceSum; }
            set { _weightFactors[17] = value; }
        }
        public float Lca3wcnDistWeightFactor
        {
            get { return _weightFactors[18] = _significance[18] / _significanceSum; }
            set { _weightFactors[18] = value; }
        }
        public float Uca3wcnDistWeightFactor
        {
            get { return _weightFactors[19] = _significance[19] / _significanceSum; }
            set { _weightFactors[19] = value; }
        }
        public float Tr2wcnDistWeightFactor
        {
            get { return _weightFactors[20] = _significance[20] / _significanceSum; }
            set { _weightFactors[20] = value; }
        }
        #endregion

        public float SignificanceSum
        {
            get
            {
                _significanceSum = 0;

                foreach (float significance in _significance)
                {
                    _significanceSum += significance;
                }
                OnPropertyChanged("CamberDownWeightFactor");
                OnPropertyChanged("CamberUpWeightFactor");
                OnPropertyChanged("ToeDownWeightFactor");
                OnPropertyChanged("ToeUpWeightFactor");
                OnPropertyChanged("CasterAngleWeightFactor");
                OnPropertyChanged("RollCentreHeightWeightFactor");
                OnPropertyChanged("CasterTrailWeightFactor");
                OnPropertyChanged("ScrubRadiusWeightFactor");
                OnPropertyChanged("KingpinAngleWeightFactor");
                OnPropertyChanged("AntiDriveWeightFactor");
                OnPropertyChanged("AntiBrakeWeightFactor");
                OnPropertyChanged("HalftrackChangeDownWeightFactor");
                OnPropertyChanged("HalftrackChangeUpWeightFactor");
                OnPropertyChanged("WheelbaseDownWeightFactor");
                OnPropertyChanged("WheelbaseUpWeightFactor");
                OnPropertyChanged("Lca3InsideWheelSpaceWeightFactor");
                OnPropertyChanged("Uca3InsideWheelSpaceWeightFactor");
                OnPropertyChanged("Tr2InsideWheelSpaceWeightFactor");
                OnPropertyChanged("Lca3wcnDistWeightFactor");
                OnPropertyChanged("Uca3wcnDistWeightFactor");
                OnPropertyChanged("Tr2wcnDistWeightFactor");

                return _significanceSum;
            }
        }
        public float ObjFunctionPeakWidth { get { return _objFunctionPeakWidth; } set { _objFunctionPeakWidth = value; } }
        public int CoreNum { get { return _coreNum; } set { _coreNum = value; } }
        public float OptimisationDuration { get { return _optimisationDuration; } set { _optimisationDuration = value; } }
        public int SuspensionPos { get { return _suspensionPos; } set { _suspensionPos = value; OnPropertyChanged("SuspensionPos"); } }
        public int DrivePos { get { return _drivePos; } set { _drivePos = value; OnPropertyChanged("DrivePos"); } }
        public int BrakesPos { get { return _brakesPos; } set { _brakesPos = value; OnPropertyChanged("BrakesPos"); } }
        public float WheelRadius { get { return _wheelRadius; } set { _wheelRadius = value; } }
        public float Wheelbase { get { return _wheelbase; } set { _wheelbase = value; } }
        public float CoGHeight { get { return _cogHeight; } set { _cogHeight = value; } }
        public float FrontDriveBias { get { return _frontDriveBias; } set { _frontDriveBias = value; OnPropertyChanged("RearDriveBias"); } }
        public float FrontBrakeBias { get { return _frontBrakeBias; } set { _frontBrakeBias = value; OnPropertyChanged("RearBrakeBias"); } }
        public float RearDriveBias { get { return _rearDriveBias = 1 - _frontDriveBias; } set { _rearDriveBias = value; } }
        public float RearBrakeBias { get { return _rearBrakeBias = 1 - _frontBrakeBias; } set { _rearBrakeBias = value; } }
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
            foreach (float significance in _significance)
            {
                _significanceSum += significance;
            }
            for (int i = 0; i < _significance.Length; i++)
            {
                _weightFactors[i] = _significance[i] / _significanceSum;
            }


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
