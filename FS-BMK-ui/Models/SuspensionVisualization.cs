using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;


namespace FS_BMK_ui.Models
{
    class SuspensionVisualization
    {
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModelVisual3D visual3d = new ModelVisual3D();
            Model3DGroup group3d = new Model3DGroup();

            visual3d.Content = group3d;

        }
    }
}
