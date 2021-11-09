namespace FS_BMK_ui.ViewModels
{
    using System;
    using FS_BMK_ui.Models;
    using System.Diagnostics;
    using System.Windows.Input;
    using System.Collections.Generic;
    using FS_BMK_ui.Commands;
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
        private string _pythonFilesPath= @"C:\dev\FS-BMK\Optimization\optimisation.py";
        private string _pythonEnvironmentPath= @"C:\ProgramData\Anaconda3\python.exe";

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
        public OptimizationSuspensionViewModel()
        {
            _optimizationSuspension = new OptimizationSuspension();
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

            await Task.Delay((int)OptimizationSuspension.OptimisationDuration*1000);

            CanPressOptimizeButton = true;

        }

        private void ExecProcess()
        {
            var psi = new ProcessStartInfo();
            string script = PythonFilesPath;//@"C:\dev\FS-BMK\Optimization\optimisation.py";
            string hardpointsString = " ";
            string featuresString = "";
            string generalSetupString;
            string argumentsString;

            psi.FileName = PythonEnvironmentPath;//@"C:\ProgramData\Anaconda3\python.exe";

            foreach (var hp in OptimizationSuspension.Hardpoints)
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


            foreach (float feature in OptimizationSuspension.SuspensionFeatureLimits)
                featuresString += feature.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";

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

            MessageBox.Show(
                    $"CurrentSuspension.WheelRadius {OptimizationSuspension.WheelRadius}\n" +
    $"CurrentSuspension.Wheelbase {OptimizationSuspension.Wheelbase}\n" +
    $"CurrentSuspension.CoGHeight {OptimizationSuspension.CoGHeight}\n" +
    $"CurrentSuspension.RearDriveBias {OptimizationSuspension.RearDriveBias}\n" +
    $"CurrentSuspension.RearBrakeBias {OptimizationSuspension.RearBrakeBias}\n" +
    $"CurrentSuspension.SuspensionPos {OptimizationSuspension.SuspensionPos}\n" +
    $"CurrentSuspension.DrivePos {OptimizationSuspension.DrivePos}\n" +
    $"CurrentSuspension.BrakesPos {OptimizationSuspension.BrakesPos}\n" +
    $"CurrentSuspension.VerticalMovement {OptimizationSuspension.VerticalMovement}"
                );


            argumentsString = script + hardpointsString + featuresString + generalSetupString;
            string path = Directory.GetCurrentDirectory();

            File.WriteAllText(@"C:\dev\FS-BMK\FS-BMK-ui\bin\Release\args.txt", argumentsString);
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
