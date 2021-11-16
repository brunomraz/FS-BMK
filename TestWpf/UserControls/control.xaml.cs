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
    public partial class control : UserControl
    {
        public string CellValue
        {
            get { return (string)GetValue(CellValueProperty); }
            set { SetValue(CellValueProperty, value); }
        }
        // Using a DependencyProperty as the backing store for LimitValue.  This enables animation, styling, binding, etc...    
        public static readonly DependencyProperty CellValueProperty =
            DependencyProperty.Register("CellValue", typeof(string), typeof(control), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
            });
        public control()
        {
            InitializeComponent();
            this.DataContext = this;
            CellValue = "Test";
        }
    }
}
