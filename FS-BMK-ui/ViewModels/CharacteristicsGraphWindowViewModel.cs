using ScottPlot;
using FS_BMK_ui.HelperClasses;
using System;
using System.Windows.Input;

namespace FS_BMK_ui.ViewModels
{
    internal class CharacteristicsGraphWindowViewModel
    {
        private float[] _xValues = new float[] { 1, 2, 3 };
        private WpfPlot _graph = new WpfPlot();

        public WpfPlot Graph { get { return _graph; } }
        public float[] XValues { get { return _xValues; } }

        private string _xLabel = "Variables";


        public string XLabel { get { return _xLabel; } set { _xLabel = value; } }

        //public ICommand OkCommand { get; }
        //public ICommand CancelCommand { get; }

        public CharacteristicsGraphWindowViewModel(float vertMovement, int vertIncr, int steerIncr, int steerPos, string name, float[] x)
        {

            double[] x_d = new double[2 * vertIncr + 1];
            double[] y_d = new double[2 * vertIncr + 1];


            for (int i = 0; i < 2 * vertIncr + 1; i++)
            {
                x_d[i] = -vertMovement + vertMovement / vertIncr * i;
                y_d[i] = x[i * (steerIncr * 2 + 1) + steerIncr + steerPos];
            }



            //for (int i = 0; i < x.Length; i++)
            //{
            //    x_d[i] = x[i];
            //    y_d[i] = y[i];
            //}

            Graph.Plot.AddScatter(x_d, y_d);
            Graph.Plot.Title($"{name}");
            Graph.Plot.YLabel("Objective function\nmodule result");
            Graph.Plot.XLabel("Variable");
            Graph.Refresh();
        }

   

    }
}