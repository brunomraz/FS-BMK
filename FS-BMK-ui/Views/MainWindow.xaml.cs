using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using FS_BMK_ui.CameraController;

namespace FS_BMK_ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private OrthographicCamera TheCamera = null;

        private SphericalCameraController CameraController = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DefineCamera(mainViewport);
        }

        private void DefineCamera(Viewport3D viewport)
        {
            //TheCamera = new PerspectiveCamera();
            TheCamera = new OrthographicCamera();
            TheCamera.Width = 60;
            //TheCamera.FieldOfView = 60;
            CameraController = new SphericalCameraController
                (TheCamera, viewport, this, mainViewport, mainViewport);
        }

    }
}
