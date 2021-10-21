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

namespace FS_BMK_ui.ViewModels
{
    class CurrentSuspensionViewModel
    {
        public const string MechanicsDLL = @"..\..\..\bin\x64\Release\MechanicsDll.dll";
        [DllImport(MechanicsDLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void suspension_movement(float[] hardpoints, float wRadiusin,
    float wheelbase, float cogHeight, float frontDriveBias, float frontBrakeBias,
    int suspPos, int drivePos, int brakePos,
    float wVertin, float wSteerin, int vertIncrin, int steerIncrin, float precisionin, float[] outputParams, float[] outputHardpoints);


        // private members
        private CurrentSuspension _currentSuspension;


        /// <summary>
        /// Initializes a new instance of the CustomerViewModel class
        /// </summary>
        public CurrentSuspensionViewModel()
        {
            _currentSuspension = new CurrentSuspension();
        }


        /// <summary>
        /// Gets the customer instance
        /// </summary>
        public CurrentSuspension CurrentSuspension
        {
            get
            {
                return _currentSuspension;
            }
        }

        private List<float>  chars = new List<float> { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f };




        private ICommand _CalculateSuspensionMovementCommand;
        public ICommand CalculateSuspensionMovementCommand
        {
            get
            {
                if (_CalculateSuspensionMovementCommand == null)
                {
                    _CalculateSuspensionMovementCommand = new RelayCommand(CalculateSuspensionMovement, CanCalculateSuspensionMovementExecute, false);
                }
                return _CalculateSuspensionMovementCommand;
            }
        }


        private void CalculateSuspensionMovement(object parameter)
        {
            float[] _suspChars = new float[11];
            float[] _movedHps = new float[15];
            float[] hardpoints = {
                CurrentSuspension.Hardpoints[0].XVal, CurrentSuspension.Hardpoints[0].YVal, CurrentSuspension.Hardpoints[0].ZVal, 
                CurrentSuspension.Hardpoints[1].XVal, CurrentSuspension.Hardpoints[1].YVal, CurrentSuspension.Hardpoints[1].ZVal, 
                CurrentSuspension.Hardpoints[2].XVal, CurrentSuspension.Hardpoints[2].YVal, CurrentSuspension.Hardpoints[2].ZVal, 
                CurrentSuspension.Hardpoints[3].XVal, CurrentSuspension.Hardpoints[3].YVal, CurrentSuspension.Hardpoints[3].ZVal, 
                CurrentSuspension.Hardpoints[4].XVal, CurrentSuspension.Hardpoints[4].YVal, CurrentSuspension.Hardpoints[4].ZVal, 
                CurrentSuspension.Hardpoints[5].XVal, CurrentSuspension.Hardpoints[5].YVal, CurrentSuspension.Hardpoints[5].ZVal, 
                CurrentSuspension.Hardpoints[6].XVal, CurrentSuspension.Hardpoints[6].YVal, CurrentSuspension.Hardpoints[6].ZVal, 
                CurrentSuspension.Hardpoints[7].XVal, CurrentSuspension.Hardpoints[7].YVal, CurrentSuspension.Hardpoints[7].ZVal, 
                CurrentSuspension.Hardpoints[8].XVal, CurrentSuspension.Hardpoints[8].YVal, CurrentSuspension.Hardpoints[8].ZVal, 
                CurrentSuspension.Hardpoints[9].XVal, CurrentSuspension.Hardpoints[9].YVal, CurrentSuspension.Hardpoints[9].ZVal
            };


            suspension_movement(hardpoints, CurrentSuspension.WheelRadius,
     CurrentSuspension.Wheelbase, CurrentSuspension.CoGHeight, CurrentSuspension.FrontDriveBias, CurrentSuspension.FrontBrakeBias,
    1,//int suspPos, 
    1,//int drivePos, 
    0,//int brakePos,
    CurrentSuspension.VerticalMovement,//float wVertin, 
    30f,//float wSteerin, 
    1,//int vertIncrin, 
    10,//int steerIncrin, 
    0.01f,//float precisionin,
    _suspChars,
    _movedHps);


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

        }

        private bool CanCalculateSuspensionMovementExecute(object parameter)
        {
            return true;

        }


    }
}
