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

namespace TestWpf.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {


        public string MyProperty1
        {
            get { return (string)GetValue(MyProperty1Property); }
            set { SetValue(MyProperty1Property, value); }
        }

        public static readonly DependencyProperty MyProperty1Property =
            DependencyProperty.Register("MyProperty1", typeof(string), typeof(UserControl1), new PropertyMetadata(""));



        public float MyProperty2
        {
            get { return (float)GetValue(MyProperty2Property); }
            set { SetValue(MyProperty2Property, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyProperty2Property =
            DependencyProperty.Register("MyProperty2", typeof(float), typeof(UserControl1), new PropertyMetadata(0f));





        public UserControl1()
        {
            InitializeComponent();
        }
    }
}
