using ScottPlot;
using FS_BMK_ui.HelperClasses;
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

        public GraphWindowViewModel(float target, float peakWidth, float peakFlatness, string name)
        {

            

            PlotPoints pts = PlotFunction(target, peakWidth, peakFlatness, 15);

            Graph.Plot.AddScatter(pts.X, pts.Y);
            Graph.Plot.Title($"{name}\n Target: {target} Peak Width: {peakWidth} Peak Flatness: {peakFlatness}");
            Graph.Plot.YLabel("Objective function\nmodule result");
            Graph.Plot.XLabel("Variable");
            Graph.Refresh();
        }

        private PlotPoints PlotFunction(double target, double peakWidth, double peakFlatness, int resolution)
        {
            double relativeLimitWidth = 6;
            double[] x = new double[2 * resolution + 1];
            double[] y = new double[2 * resolution + 1];
            double interval = Math.Pow(relativeLimitWidth * peakWidth, 1 / peakFlatness);
            for (int i = 0; i < resolution; i++)
            {
                x[i] = target - interval + interval / resolution * i;
                y[i] = Math.Exp(-1 / peakWidth * Math.Pow(Math.Abs(x[i] - target), peakFlatness));
            }

            x[resolution] = target;
            y[resolution] = 1;

            for (int i = 0; i < resolution; i++)
            {
                x[resolution + 1 + i] = target + interval / resolution * (i + 1);
                y[resolution + 1 + i] = Math.Exp(-1 / peakWidth * Math.Pow(Math.Abs(x[resolution + 1 + i] - target), peakFlatness));
            }

            return new PlotPoints { X = x, Y = y};
        }

        private class PlotPoints
        {

            public double[] X { get; set; }
            public double[] Y { get; set; }
        }

    }
}