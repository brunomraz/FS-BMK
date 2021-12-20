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
using System.ComponentModel;
using FS_BMK_ui.Views;

namespace FS_BMK_ui.ViewModels
{
    class CurrentSuspensionViewModel : INotifyPropertyChanged
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

        private int _vertPos;
        private int _steerPos;

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

        public int VertPos
        {
            get { return _vertPos; }
            set
            {
                if (_vertPos != value)
                {
                    _vertPos = value;
                    OnPropertyChanged("VertPos");
                    _suspensionVisualization.UpdateModels(VertPos, SteerPos);
                    CurrentSuspension.SuspensionCharacteristics[0].Value = CurrentSuspension.CamberAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[1].Value = CurrentSuspension.ToeAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[2].Value = CurrentSuspension.CasterAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[3].Value = CurrentSuspension.RcHeight.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[4].Value = CurrentSuspension.CasterTrail.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[5].Value = CurrentSuspension.ScrubRadius.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[6].Value = CurrentSuspension.KingpinAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[7].Value = CurrentSuspension.AntiDrive.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[8].Value = CurrentSuspension.AntiBrake.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[9].Value = CurrentSuspension.HalfTrackChange.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[10].Value = CurrentSuspension.WheelbaseChange.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                }
            }
        }
        public int SteerPos
        {
            get { return _steerPos; }
            set
            {
                if (_steerPos != value)
                {
                    _steerPos = value;
                    OnPropertyChanged("SteerPos");
                    _suspensionVisualization.UpdateModels(VertPos, SteerPos);
                    CurrentSuspension.SuspensionCharacteristics[0].Value = CurrentSuspension.CamberAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[1].Value = CurrentSuspension.ToeAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[2].Value = CurrentSuspension.CasterAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[3].Value = CurrentSuspension.RcHeight.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[4].Value = CurrentSuspension.CasterTrail.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[5].Value = CurrentSuspension.ScrubRadius.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[6].Value = CurrentSuspension.KingpinAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[7].Value = CurrentSuspension.AntiDrive.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[8].Value = CurrentSuspension.AntiBrake.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[9].Value = CurrentSuspension.HalfTrackChange.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
                    CurrentSuspension.SuspensionCharacteristics[10].Value = CurrentSuspension.WheelbaseChange.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];

                    CurrentSuspension.CamberAngle.SteerPos = SteerPos;
                    CurrentSuspension.ToeAngle.SteerPos = SteerPos;
                    CurrentSuspension.CasterAngle.SteerPos = SteerPos;
                    CurrentSuspension.RcHeight.SteerPos = SteerPos;
                    CurrentSuspension.CasterTrail.SteerPos = SteerPos;
                    CurrentSuspension.ScrubRadius.SteerPos = SteerPos;
                    CurrentSuspension.KingpinAngle.SteerPos = SteerPos;
                    CurrentSuspension.AntiDrive.SteerPos = SteerPos;
                    CurrentSuspension.AntiBrake.SteerPos = SteerPos;
                    CurrentSuspension.HalfTrackChange.SteerPos = SteerPos;
                    CurrentSuspension.WheelbaseChange.SteerPos = SteerPos;
                }
            }
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
                0.001f,//float precisionin,

                CurrentSuspension.CamberAngle.Characteristic,
                CurrentSuspension.ToeAngle.Characteristic,
                CurrentSuspension.CasterAngle.Characteristic,
                CurrentSuspension.RcHeight.Characteristic,
                CurrentSuspension.CasterTrail.Characteristic,
                CurrentSuspension.ScrubRadius.Characteristic,
                CurrentSuspension.KingpinAngle.Characteristic,
                CurrentSuspension.AntiDrive.Characteristic,
                CurrentSuspension.AntiBrake.Characteristic,
                CurrentSuspension.HalfTrackChange.Characteristic,
                CurrentSuspension.WheelbaseChange.Characteristic,
                CurrentSuspension.ConstOutputParams,

                CurrentSuspension.Lca3Moved,
                CurrentSuspension.Uca3Moved,
                CurrentSuspension.Tr1Moved,
                CurrentSuspension.Tr2Moved,
                CurrentSuspension.WcnMoved,
                CurrentSuspension.SpnMoved
                );



            _suspensionVisualization.UpdateModels(VertPos, SteerPos);

            CurrentSuspension.SuspensionCharacteristics[0].Value = CurrentSuspension.CamberAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[1].Value = CurrentSuspension.ToeAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[2].Value = CurrentSuspension.CasterAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[3].Value = CurrentSuspension.RcHeight.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[4].Value = CurrentSuspension.CasterTrail.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[5].Value = CurrentSuspension.ScrubRadius.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[6].Value = CurrentSuspension.KingpinAngle.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[7].Value = CurrentSuspension.AntiDrive.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[8].Value = CurrentSuspension.AntiBrake.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[9].Value = CurrentSuspension.HalfTrackChange.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];
            CurrentSuspension.SuspensionCharacteristics[10].Value = CurrentSuspension.WheelbaseChange.Characteristic[(CurrentSuspension.VertIncr + _vertPos) * (2 * CurrentSuspension.SteerIncr + 1) + CurrentSuspension.SteerIncr + _steerPos];

            CurrentSuspension.CamberAngle.SteerPos = SteerPos;
            CurrentSuspension.ToeAngle.SteerPos = SteerPos;
            CurrentSuspension.CasterAngle.SteerPos = SteerPos;
            CurrentSuspension.RcHeight.SteerPos = SteerPos;
            CurrentSuspension.CasterTrail.SteerPos = SteerPos;
            CurrentSuspension.ScrubRadius.SteerPos = SteerPos;
            CurrentSuspension.KingpinAngle.SteerPos = SteerPos;
            CurrentSuspension.AntiDrive.SteerPos = SteerPos;
            CurrentSuspension.AntiBrake.SteerPos = SteerPos;
            CurrentSuspension.HalfTrackChange.SteerPos = SteerPos;
            CurrentSuspension.WheelbaseChange.SteerPos = SteerPos;

            CurrentSuspension.SuspensionCharacteristics[11].Value = CurrentSuspension.ConstOutputParams[0];
            CurrentSuspension.SuspensionCharacteristics[12].Value = CurrentSuspension.ConstOutputParams[1];
            CurrentSuspension.SuspensionCharacteristics[13].Value = CurrentSuspension.ConstOutputParams[2];
            CurrentSuspension.SuspensionCharacteristics[14].Value = CurrentSuspension.ConstOutputParams[3];
            CurrentSuspension.SuspensionCharacteristics[15].Value = CurrentSuspension.ConstOutputParams[4];
            CurrentSuspension.SuspensionCharacteristics[16].Value = CurrentSuspension.ConstOutputParams[5];
        }


        private bool CanCalculateSuspensionMovementExecute(object parameter)
        {
            return true;

        }



        private void OpenCharacteristicsGraphWindow(object parameter)
        {
            var viewModel = new CharacteristicsGraphWindowViewModel(
                ((CurrentSuspCharacteristics)parameter).VertMovement,
                ((CurrentSuspCharacteristics)parameter).VertIncr,
                ((CurrentSuspCharacteristics)parameter).SteerIncr,
                ((CurrentSuspCharacteristics)parameter).SteerPos,
                ((CurrentSuspCharacteristics)parameter).Name,
                ((CurrentSuspCharacteristics)parameter).Characteristic
                );
            var view = new CharacteristicsGraphWindow { DataContext = viewModel };
            view.Show();
        }

        private ICommand _openCharacteristicsGraphWindowCommand;

        public ICommand OpenCharacteristicsGraphWindowCommand
        {
            get
            {
                if (_openCharacteristicsGraphWindowCommand == null)
                {
                    _openCharacteristicsGraphWindowCommand = new RelayCommand(OpenCharacteristicsGraphWindowExecute, 
                        CanOpenCharacteristicsGraphWindowExecute, false);
                }
                return _openCharacteristicsGraphWindowCommand;
            }
        }

        private void OpenCharacteristicsGraphWindowExecute(object parameter)
        {
            OpenCharacteristicsGraphWindow(parameter);
        }

        private bool CanOpenCharacteristicsGraphWindowExecute(object parameter)
        {
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
