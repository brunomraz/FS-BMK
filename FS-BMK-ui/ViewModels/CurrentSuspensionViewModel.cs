using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FS_BMK_ui.Models;
using FS_BMK_ui.Commands;
using System.Windows.Input;
using System.Windows;
using System.Runtime.InteropServices;
using FS_BMK_ui.HelperClasses;

namespace FS_BMK_ui.ViewModels
{
    class CurrentSuspensionViewModel
    {
        public const string MechanicsDLL = @"MechanicsDll.dll";
        [DllImport(MechanicsDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void suspension_movement(
            float[] hardpoints, 
            float wRadiusin,
            float wheelbase,
            float cogHeight, 
            float frontDriveBias, 
            float frontBrakeBias,
            int suspPos, 
            int drivePos, 
            int brakePos,
            float wVertin, 
            float wSteerin, 
            int vertIncrin, 
            int steerIncrin, 
            float precisionin,

            float[] CamberAngle,
            float[] ToeAngle,
            float[] CasterAngle,
            float[] RcHeight,
            float[] CasterTrail,
            float[] ScrubRadius,
            float[] KingpinAngle,
            float[] AntiDrive,
            float[] AntiBrake,
            float[] HalfTrackChange,
            float[] WheelbaseChange,
            float[] ConstOutputParams,

            float[] Lca3Moved,
            float[] Uca3Moved,
            float[] Tr1Moved,
            float[] Tr2Moved,
            float[] WcnMoved,
            float[] SpnMoved
            );


        // private members
        private CurrentSuspension _currentSuspension;
        private SuspensionVisualization _suspensionVisualization;

        public CurrentSuspensionViewModel()
        {
            _currentSuspension = new CurrentSuspension();
            _suspensionVisualization = new SuspensionVisualization(_currentSuspension);
        }

        public CurrentSuspension CurrentSuspension
        {
            get
            {
                return _currentSuspension;
            }
        }

        public SuspensionVisualization SuspensionVisualization 
        {
            get { return _suspensionVisualization; }
        }


        private ICommand _CalculateSuspensionMovementCommand;
        public ICommand CalculateSuspensionMovementCommand
        {
            get
            {
                if (_CalculateSuspensionMovementCommand == null)
                {
                    _CalculateSuspensionMovementCommand = new RelayCommand(CalculateSuspensionMovement2, CanCalculateSuspensionMovementExecute, false);
                }
                return _CalculateSuspensionMovementCommand;
            }
        }


        public void CalculateSuspensionMovement2(object parameter)
        {
            float[] _suspChars = new float[17];
            float[] hardpoints = {
                CurrentSuspension.Hardpoints[0].X, CurrentSuspension.Hardpoints[0].Y, CurrentSuspension.Hardpoints[0].Z,
                CurrentSuspension.Hardpoints[1].X, CurrentSuspension.Hardpoints[1].Y, CurrentSuspension.Hardpoints[1].Z,
                CurrentSuspension.Hardpoints[2].X, CurrentSuspension.Hardpoints[2].Y, CurrentSuspension.Hardpoints[2].Z,
                CurrentSuspension.Hardpoints[3].X, CurrentSuspension.Hardpoints[3].Y, CurrentSuspension.Hardpoints[3].Z,
                CurrentSuspension.Hardpoints[4].X, CurrentSuspension.Hardpoints[4].Y, CurrentSuspension.Hardpoints[4].Z,
                CurrentSuspension.Hardpoints[5].X, CurrentSuspension.Hardpoints[5].Y, CurrentSuspension.Hardpoints[5].Z,
                CurrentSuspension.Hardpoints[6].X, CurrentSuspension.Hardpoints[6].Y, CurrentSuspension.Hardpoints[6].Z,
                CurrentSuspension.Hardpoints[7].X, CurrentSuspension.Hardpoints[7].Y, CurrentSuspension.Hardpoints[7].Z,
                CurrentSuspension.Hardpoints[8].X, CurrentSuspension.Hardpoints[8].Y, CurrentSuspension.Hardpoints[8].Z,
                CurrentSuspension.Hardpoints[9].X, CurrentSuspension.Hardpoints[9].Y, CurrentSuspension.Hardpoints[9].Z
            };

            float driveBias;
            float brakeBias;

            if (CurrentSuspension.SuspensionPos == 0)   // front suspension
            {
                driveBias = CurrentSuspension.FrontDriveBias;
                brakeBias = CurrentSuspension.FrontBrakeBias;
            }
            else
            {                
                driveBias = CurrentSuspension.RearDriveBias;
                brakeBias = CurrentSuspension.RearBrakeBias;
            }

            suspension_movement(hardpoints, 
                CurrentSuspension.WheelRadius,
                CurrentSuspension.Wheelbase, 
                CurrentSuspension.CoGHeight, 
                driveBias, 
                brakeBias,
                CurrentSuspension.SuspensionPos,//1,//int suspPos, 
                CurrentSuspension.DrivePos,//1,//int drivePos, 
                CurrentSuspension.BrakesPos,//0,//int brakePos,
                CurrentSuspension.VerticalMovement,//float wVertin, 
                CurrentSuspension.SteeringMovement,//30f,//float wSteerin, 
                CurrentSuspension.VertIncr,//int vertIncrin, 
                CurrentSuspension.SteerIncr,//10,//int steerIncrin, 
                0.01f,//float precisionin,

                CurrentSuspension.CamberAngle,
                CurrentSuspension.ToeAngle,
                CurrentSuspension.CasterAngle,
                CurrentSuspension.RcHeight,
                CurrentSuspension.CasterTrail,
                CurrentSuspension.ScrubRadius,
                CurrentSuspension.KingpinAngle,
                CurrentSuspension.AntiDrive, 
                CurrentSuspension.AntiBrake,
                CurrentSuspension.HalfTrackChange,
                CurrentSuspension.WheelbaseChange, 
                CurrentSuspension.ConstOutputParams,


                CurrentSuspension.Lca3Moved,
                CurrentSuspension.Uca3Moved,
                CurrentSuspension.Tr1Moved,
                CurrentSuspension.Tr2Moved,
                CurrentSuspension.WcnMoved,
                CurrentSuspension.SpnMoved
        
                );

            _suspensionVisualization.UpdateModels();

            CurrentSuspension.SuspensionCharacteristics[0].Value = _suspChars[0];
            CurrentSuspension.SuspensionCharacteristics[1].Value = _suspChars[1];
            CurrentSuspension.SuspensionCharacteristics[2].Value = _suspChars[2];
            CurrentSuspension.SuspensionCharacteristics[3].Value = _suspChars[3];
            CurrentSuspension.SuspensionCharacteristics[4].Value = _suspChars[4];
            CurrentSuspension.SuspensionCharacteristics[5].Value = _suspChars[5];
            CurrentSuspension.SuspensionCharacteristics[6].Value = _suspChars[6];
            CurrentSuspension.SuspensionCharacteristics[7].Value = _suspChars[7];
            CurrentSuspension.SuspensionCharacteristics[8].Value = _suspChars[8];
            CurrentSuspension.SuspensionCharacteristics[9].Value = _suspChars[9];
            CurrentSuspension.SuspensionCharacteristics[10].Value = _suspChars[10];
            CurrentSuspension.SuspensionCharacteristics[11].Value = _suspChars[11];
            CurrentSuspension.SuspensionCharacteristics[12].Value = _suspChars[12];
            CurrentSuspension.SuspensionCharacteristics[13].Value = _suspChars[13];
            CurrentSuspension.SuspensionCharacteristics[14].Value = _suspChars[14];
            CurrentSuspension.SuspensionCharacteristics[15].Value = _suspChars[15];
            CurrentSuspension.SuspensionCharacteristics[16].Value = _suspChars[16];
        }


        private bool CanCalculateSuspensionMovementExecute(object parameter)
        {
            return true;

        }


    }
}
