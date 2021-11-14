using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FS_BMK_ui.UserControls
{
    /// <summary>
    /// Interaction logic for FeatureLimitsUserControl.xaml
    /// </summary>
    public partial class FeatureLimitsUserControl : UserControl
    {


        public string HeaderName
        {
            get { return (string)GetValue(HeaderNameProperty); }
            set { SetValue(HeaderNameProperty, value); }
        }
        public static readonly DependencyProperty HeaderNameProperty =
            DependencyProperty.Register("HeaderName", typeof(string), typeof(FeatureLimitsUserControl), new PropertyMetadata(""));


        public float LowerValue
        {
            get { return (float)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }
        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(float), typeof(FeatureLimitsUserControl),
                new PropertyMetadata(0f));


        public float UpperValue
        {
            get { return (float)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }
        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(float), typeof(FeatureLimitsUserControl), new PropertyMetadata(0f));


        public float TargetValue
        {
            get { return (float)GetValue(TargetValueProperty); }
            set { SetValue(TargetValueProperty, value); }
        }
        public static readonly DependencyProperty TargetValueProperty =
            DependencyProperty.Register("TargetValue", typeof(float), typeof(FeatureLimitsUserControl), new PropertyMetadata(0f));


        public float SignificanceValue
        {
            get { return (float)GetValue(SignificanceValueProperty); }
            set { SetValue(SignificanceValueProperty, value); }
        }

        public static readonly DependencyProperty SignificanceValueProperty =
            DependencyProperty.Register("SignificanceValue", typeof(float), typeof(FeatureLimitsUserControl), new PropertyMetadata(0f));


        public float WeightFactorValue
        {
            get { return (float)GetValue(WeightFactorValueProperty); }
            set { SetValue(WeightFactorValueProperty, value); }
        }
        public static readonly DependencyProperty WeightFactorValueProperty =
            DependencyProperty.Register("WeightFactorValue", typeof(float), typeof(FeatureLimitsUserControl), new PropertyMetadata(0f));



        public FeatureLimitsUserControl()
        {
            InitializeComponent();
        }
    }
}
