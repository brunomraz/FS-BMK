﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FS_BMK_ui.HelperClasses;

namespace FS_BMK_ui.Models
{
    class CurrentSuspension : INotifyPropertyChanged
    {


        private float _wheelRadius = 210f;
        private float _wheelWidth = 200f;
        private float _wheelInsideRadius = 100f;
        private float _wheelbase = 1530f;
        private float _cogHeight = 300f;
        private float _frontDriveBias = 0f;
        private float _frontBrakeBias = 0.6f;
        private float _rearDriveBias;
        private float _rearBrakeBias;
        private float _verticalMovement = 30f;
        private float _steeringMovement = 10f;
        private int _vertIncr = 20;
        private int _steerIncr = 20;
        private int _vertPosMin;
        private int _vertPosMax;
        private int _steerPosMin;
        private int _steerPosMax;

        private float[] _camberAngle = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _toeAngle = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _casterAngle = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _rcHeight = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _casterTrail = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _scrubRadius = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _kingpinAngle = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _antiDrive = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _antiBrake = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _halfTrackChange = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _wheelbaseChange = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _constOutputParams = new float[15]; /* lca3, uca3, tr2, wcn, spn*/

        private float[] _lca3Moved = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _uca3Moved = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _tr1Moved = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _tr2Moved = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _wcnMoved = new float[15]; /* lca3, uca3, tr2, wcn, spn*/
        private float[] _spnMoved = new float[15]; /* lca3, uca3, tr2, wcn, spn*/

        private int _suspensionPos = 1; // front or rear suspension 0 for front, 1 for rear
        private int _drivePos = 1;  // outboard or inboard drive 0 for outboard, 1 for inboard
        private int _brakesPos = 0; // outboard or inboard brakes 0 for outboard, 1 for inboard
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
            new WPFFloat(),   // Camber angle
            new WPFFloat(),   // toe angle
            new WPFFloat(),   // caster angle
            new WPFFloat(),   // RC height
            new WPFFloat(),   // caster trail
            new WPFFloat(),   // scrub radius
            new WPFFloat(),   // kingpin angle
            new WPFFloat(),   // anti drive
            new WPFFloat(),   // anti brake
            new WPFFloat(),   // half track change
            new WPFFloat(),    // wheelbase change
            new WPFFloat(),    // LCA3 free radius
            new WPFFloat(),    // UCA3 free radius
            new WPFFloat(),    // TR2 free radius
            new WPFFloat(),    // LCA3 - WCN distance
            new WPFFloat(),    // UCA3 - WCN distance
            new WPFFloat()    // TR2 - WCN distance
        };

        public CurrentSuspension()
        {
            _camberAngle = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _toeAngle = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _casterAngle = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _rcHeight = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _casterTrail = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _scrubRadius = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _kingpinAngle = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _antiDrive = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _antiBrake = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _halfTrackChange = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _wheelbaseChange = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];
            _constOutputParams = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1)];

            _lca3Moved = new float[(VertIncr * 2 + 1) * 3];
            _uca3Moved = new float[(VertIncr * 2 + 1) * 3];
            _tr1Moved = new float[(SteerIncr * 2 + 1) * 3];
            _tr2Moved = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1) * 3];
            _wcnMoved = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1) * 3];
            _spnMoved = new float[(VertIncr * 2 + 1) * (SteerIncr * 2 + 1) * 3];
        }

        

        public int VertIncr { get { return _vertIncr; } set { _vertIncr = value; OnPropertyChanged("VertPosMin"); OnPropertyChanged("VertPosMax"); } }
        public int VertPosMin { get { return _vertPosMin = -VertIncr; } set { _vertPosMin = value; } }
        public int VertPosMax { get { return _vertPosMax = VertIncr; } set { _vertPosMax = value; } }
        public int SteerIncr { get { return _steerIncr; } set { _steerIncr = value; OnPropertyChanged("SteerPosMin"); OnPropertyChanged("SteerPosMax"); } }
        public int SteerPosMin { get { return _steerPosMin = -SteerIncr; } set { _steerPosMin = value; } }
        public int SteerPosMax { get { return _steerPosMax = SteerIncr; } set { _steerPosMax = value; } }

        public int SuspensionPos { get { return _suspensionPos; } set { _suspensionPos = value; OnPropertyChanged("SuspensionPos"); } }
        public int DrivePos { get { return _drivePos; } set { _drivePos = value; OnPropertyChanged("DrivePos"); } }
        public int BrakesPos { get { return _brakesPos; } set { _brakesPos = value; OnPropertyChanged("BrakesPos"); } }
        //public float[] HardpointsMoved { get { return _hardpointsMoved; } set { _hardpointsMoved = value; } }

        public float[] CamberAngle { get { return _camberAngle; } }
        public float[] ToeAngle { get { return _toeAngle; } }
        public float[] CasterAngle { get { return _casterAngle; } }
        public float[] RcHeight { get { return _rcHeight; } }
        public float[] CasterTrail { get { return _casterTrail; } }
        public float[] ScrubRadius { get { return _scrubRadius; } }
        public float[] KingpinAngle { get { return _kingpinAngle; } }
        public float[] AntiDrive { get { return _antiDrive; } }
        public float[] AntiBrake { get { return _antiBrake; } }
        public float[] HalfTrackChange { get { return _halfTrackChange; } }
        public float[] WheelbaseChange { get { return _wheelbaseChange; } }
        public float[] ConstOutputParams { get { return _constOutputParams; } }

        public float[] Lca3Moved { get { return _lca3Moved; } }
        public float[] Uca3Moved { get { return _uca3Moved; } }
        public float[] Tr1Moved { get { return _tr1Moved; } }
        public float[] Tr2Moved { get { return _tr2Moved; } }
        public float[] WcnMoved { get { return _wcnMoved; } }
        public float[] SpnMoved { get { return _spnMoved; } }

        public List<WPFFloat> SuspensionCharacteristics { get { return _suspensionCharacteristics; } set { _suspensionCharacteristics = value; } }
        public List<Hardpoint> Hardpoints { get { return _hardpoints; } }
        public float WheelRadius { get { return _wheelRadius; } set { _wheelRadius = value; } }
        public float WheelInsideRadius { get { return _wheelInsideRadius; } set { _wheelInsideRadius = value; } }
        public float WheelWidth { get { return _wheelWidth; } set { _wheelWidth = value; } }
        public float Wheelbase { get { return _wheelbase; } set { _wheelbase = value; } }
        public float CoGHeight { get { return _cogHeight; } set { _cogHeight = value; } }
        public float FrontDriveBias { get { return _frontDriveBias; } set { _frontDriveBias = value; OnPropertyChanged("RearDriveBias"); } }
        public float FrontBrakeBias { get { return _frontBrakeBias; } set { _frontBrakeBias = value; OnPropertyChanged("RearBrakeBias"); } }
        public float RearDriveBias { get { return _rearDriveBias = 1 - _frontDriveBias; } set { _rearDriveBias = value; } }
        public float RearBrakeBias { get { return _rearBrakeBias = 1 - _frontBrakeBias; } set { _rearBrakeBias = value; } }
        public float VerticalMovement { get { return _verticalMovement; } set { _verticalMovement = value; } }
        public float SteeringMovement { get { return _steeringMovement; } set { _steeringMovement = value; } }


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
