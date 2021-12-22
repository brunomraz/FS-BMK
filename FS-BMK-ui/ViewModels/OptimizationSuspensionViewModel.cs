namespace FS_BMK_ui.ViewModels
{
    using System;
    using FS_BMK_ui.Models;
    using System.Diagnostics;
    using System.Windows.Input;
    using System.Collections.Generic;
    using FS_BMK_ui.Commands;
    using FS_BMK_ui.Views;
    using FS_BMK_ui.HelperClasses;
    using System.Windows;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Threading;
    using System.IO;

    internal class OptimizationSuspensionViewModel : INotifyPropertyChanged
    {

        // private members
        private OptimizationSuspension _optimizationSuspension;
        private bool _canPressOptimizeButton = true;
        private string _pythonFilesPath = @"C:\dev\FS-BMK\Optimization\optimisation2.py";
        private string _pythonEnvironmentPath = @"C:\ProgramData\Anaconda3\python.exe";

        public OptimizationSuspensionViewModel( )
        {
            _optimizationSuspension = new OptimizationSuspension();
        }

        public string PythonFilesPath
        {
            get { return _pythonFilesPath; }
            set { _pythonFilesPath = value; }
        }

        public string PythonEnvironmentPath
        {
            get { return _pythonEnvironmentPath; }
            set { _pythonEnvironmentPath = value; }
        }

        public bool CanPressOptimizeButton
        {
            get
            {
                return _canPressOptimizeButton;
            }
            set
            {
                _canPressOptimizeButton = value;
                OnPropertyChanged("CanPressOptimizeButton");
            }
        }
        

        public OptimizationSuspension OptimizationSuspension
        {
            get
            {
                return _optimizationSuspension;
            }
        }

        private async void AsyncCallTest()
        {
            CanPressOptimizeButton = false;

            await Task.Delay((int)OptimizationSuspension.OptimisationDuration * 1000);

            CanPressOptimizeButton = true;

        }

        private void ExecProcess()
        {
            var psi = new ProcessStartInfo();
            string script = PythonFilesPath; 
            string hardpointsString = " ";
            string featuresString = "";
            string generalSetupString;
            string argumentsString;

            psi.FileName = PythonEnvironmentPath; 

            foreach (var hp in OptimizationSuspension.HardpointsLimits)
            {
                if (hp.XIsEditable)
                {
                    hardpointsString += hp.XValLow.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                    hardpointsString += hp.XValHigh.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                }
                else
                    hardpointsString += hp.XValLow.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";

                if (hp.YIsEditable)
                {
                    hardpointsString += hp.YValLow.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                    hardpointsString += hp.YValHigh.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                }
                else
                    hardpointsString += hp.YValLow.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";

                if (hp.ZIsEditable)
                {
                    hardpointsString += hp.ZValLow.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                    hardpointsString += hp.ZValHigh.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                }
                else
                    hardpointsString += hp.ZValLow.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
            }

            for (int i = 0; i < 21; i++)
            {
                //featuresString += 
                //    OptimizationSuspension.SuspensionFeatureLimits[2 * i].ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                //    OptimizationSuspension.SuspensionFeatureLimits[2 * i + 1].ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                //featuresString += OptimizationSuspension.TargetCharacteristicValues[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                //featuresString += OptimizationSuspension.WeightFactors[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                //featuresString += OptimizationSuspension.PeakWidthValues[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                //featuresString += OptimizationSuspension.PeakFlatnessValues[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";

                featuresString +=
                    OptimizationSuspension.OptimisationCharacteristics[i].Lower.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                    OptimizationSuspension.OptimisationCharacteristics[i].Upper.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                featuresString += OptimizationSuspension.OptimisationCharacteristics[i].Target.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                featuresString += OptimizationSuspension.WeightFactors[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                featuresString += OptimizationSuspension.OptimisationCharacteristics[i].PeakWidth.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
                featuresString += OptimizationSuspension.OptimisationCharacteristics[i].PeakFlatness.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";

            }

            float DriveBias;
            float BrakeBias;
            if (OptimizationSuspension.SuspensionPos == 0)
            {
                DriveBias = OptimizationSuspension.FrontDriveBias;
                BrakeBias = OptimizationSuspension.FrontBrakeBias;
            }
            else
            {
                DriveBias = OptimizationSuspension.RearDriveBias;
                BrakeBias = OptimizationSuspension.RearBrakeBias;
            }

            generalSetupString =
                OptimizationSuspension.SuspensionPos.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.WheelRadius.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.Wheelbase.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.CoGHeight.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                DriveBias.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                BrakeBias.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.DrivePos.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.BrakesPos.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.VerticalMovement.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.CoreNum.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " +
                OptimizationSuspension.OptimisationDuration.ToString(System.Globalization.CultureInfo.InvariantCulture);


            argumentsString = script + hardpointsString + featuresString + generalSetupString;
            string path = Directory.GetCurrentDirectory();

            File.WriteAllText(@"args.txt", argumentsString);
            MessageBox.Show(argumentsString);

            psi.Arguments = argumentsString;

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;

            //var results = OptimizationSuspension.SuspensionFeatureLimits[0];

            using (var process = Process.Start(psi))
            {
                //results = process.StandardOutput.ReadToEnd();
            }

        }

        private ICommand _OptimiseCommand;

        public ICommand OptimiseCommand
        {
            get
            {
                if (_OptimiseCommand == null)
                {
                    _OptimiseCommand = new RelayCommand(OptimiseExecute, CanOptimiseExecute, false);
                }
                return _OptimiseCommand;
            }
        }

        private void OptimiseExecute(object parameter)
        {
            AsyncCallTest();
            ExecProcess();
        }

        private bool CanOptimiseExecute(object parameter)
        {
            if (OptimizationSuspension.CompareLowHighValues() && _canPressOptimizeButton)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OpenGraphWindow(object parameter)
        {
            MessageBox.Show(((OptimisationCharacteristic)parameter).Name);
            var viewModel = new GraphWindowViewModel(
                ((OptimisationCharacteristic)parameter).Target, 
                ((OptimisationCharacteristic)parameter).PeakWidth, 
                ((OptimisationCharacteristic)parameter).PeakFlatness,
                ((OptimisationCharacteristic)parameter).Name
                );
            var view = new GraphWindow { DataContext = viewModel };
            view.Show();
        }

        private ICommand _openGraphWindowCommand;

        public ICommand OpenGraphWindowCommand
        {
            get
            {
                if (_openGraphWindowCommand == null)
                {
                    _openGraphWindowCommand = new RelayCommand(OpenGraphWindowExecute, CanOpenGraphWindowExecute, false);
                }
                return _openGraphWindowCommand;
            }
        }

        private void OpenGraphWindowExecute(object parameter)
        {
            OpenGraphWindow(parameter);
        }

        private bool CanOpenGraphWindowExecute(object parameter)
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
