using ScottPlot;
using System;
using System.Windows.Input;

namespace FS_BMK_ui.ViewModels
{
    internal class GraphWindowViewModel
    {
        private float[] _xValues = new float[] { 1, 2, 3 };
        private WpfPlot _graph = new WpfPlot();

        public WpfPlot Graph { get { return _graph; } }
        public float[] XValues { get { return _xValues; } }

        private string _xLabel = "Variables";


        public string XLabel { get { return _xLabel; } set { _xLabel = value; } }

        //public ICommand OkCommand { get; }
        //public ICommand CancelCommand { get; }

        public GraphWindowViewModel()
        {
            double[] dataX = new double[] { 1, 2, 3, 4, 50 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };

            Graph.Plot.AddScatter(dataX, dataY);
            Graph.Plot.Title("smth");
            Graph.Plot.YLabel("Objective function\nmodule result");
            Graph.Plot.XLabel("Variable");
            Graph.Refresh();
        }
    }
}