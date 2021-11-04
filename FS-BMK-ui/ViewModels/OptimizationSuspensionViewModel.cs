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

    internal class OptimizationSuspensionViewModel:INotifyPropertyChanged
    {

        // private members
        private OptimizationSuspension _optimizationSuspension;
        private bool _canPressOptimizeButton = true;

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
            try
            {
                //await Task.Factory.StartNew(ExecProcess);
                await Task.Factory.StartNew(() => Thread.Sleep(3000));
            }
            finally
            {
                CanPressOptimizeButton = true;
            }
        }

        private void ExecProcess()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\ProgramData\Anaconda3\python.exe";

            var script = @"C:\dev\FS-BMK\Optimization\module1.py";
            var a = OptimizationSuspension.SuspensionFeatureLimits[0]; //"rtgf";

            psi.Arguments = string.Format("\"{0}\" \"{1}\"", script, a); // $"\"{script}\"\"{a}\"";

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            psi.RedirectStandardOutput = false;
            psi.RedirectStandardError = false;

            //var results = OptimizationSuspension.SuspensionFeatureLimits[0];

            using (var process = Process.Start(psi))
            {
                //results = process.StandardOutput.ReadToEnd();
            }
            //MessageBox.Show($"done");

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

            //MessageBox.Show($"executed optimise button xval high {OptimizationSuspension.Hardpoints[0].XValHigh} " +
            //    $"xval low {OptimizationSuspension.Hardpoints[0].XValLow}");
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
