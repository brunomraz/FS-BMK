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
        public static extern void suspension_movement(float[] inputArray);


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
            float[] intermediateArray = new float[11];

            suspension_movement(intermediateArray);



            MessageBox.Show($"novo calculating suspension characteristics");

            //for (int i = 0; i < intermediateArray.Length; i++)
            //{
            //    CurrentSuspension.SuspensionCharacteristics[i].Value = intermediateArray[i];
            //}
            CurrentSuspension.SuspensionCharacteristics[0].Value = intermediateArray[0];
            CurrentSuspension.SuspensionCharacteristics[1].Value = intermediateArray[1];
            CurrentSuspension.SuspensionCharacteristics[2].Value = intermediateArray[2];
            CurrentSuspension.SuspensionCharacteristics[3].Value = intermediateArray[3];
            CurrentSuspension.SuspensionCharacteristics[4].Value = intermediateArray[4];
            CurrentSuspension.SuspensionCharacteristics[5].Value = intermediateArray[5];
            CurrentSuspension.SuspensionCharacteristics[6].Value = intermediateArray[6];
            CurrentSuspension.SuspensionCharacteristics[7].Value = intermediateArray[7];
            CurrentSuspension.SuspensionCharacteristics[8].Value = intermediateArray[8];
            CurrentSuspension.SuspensionCharacteristics[9].Value = intermediateArray[9];
            CurrentSuspension.SuspensionCharacteristics[10].Value = intermediateArray[10];
            //CurrentSuspension.Test = 5f * CurrentSuspension.Hardpoints[0].XVal;



        }

        private bool CanCalculateSuspensionMovementExecute(object parameter)
        {
            return true;

        }


    }
}
