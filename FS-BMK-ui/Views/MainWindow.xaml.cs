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
using System.Windows.Shapes;
using FS_BMK_ui.ViewModels;
using System.Windows.Media.Media3D;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ModelVisual3D visual3d = new ModelVisual3D();
            Model3DGroup group3d = new Model3DGroup();

            visual3d.Content = group3d;
            mainViewport.Children.Add(visual3d);

            // defining camera, lights and model
            DefineCamera(mainViewport);
            DefineLights(group3d);
            DefineModel(group3d);
        }

        private void DefineModel(Model3DGroup group)
        {
            // create the geometry
            MeshGeometry3D mesh = new MeshGeometry3D();

            // define the positions
            Point3D[] points =
            {
                new Point3D(-1, -1, -1), new Point3D(1, -1, -1),
                new Point3D(1, -1, 1), new Point3D(-1, -1, 1),
                new Point3D(-1, -1, 1), new Point3D(1, -1, 1),
                new Point3D(1, 1, 1), new Point3D(-1, 1, 1),
                new Point3D(1, -1, 1), new Point3D(1, -1, -1),
                new Point3D(1, 1, -1), new Point3D(1, 1, 1),
                new Point3D(1, 1, 1), new Point3D(1, 1, -1),
                new Point3D(-1, 1, -1), new Point3D(-1, 1, 1),
                new Point3D(-1, -1, 1), new Point3D(-1, 1, 1),
                new Point3D(-1, 1, -1), new Point3D(-1, -1, -1),
                new Point3D(-1, -1, -1), new Point3D(-1, 1, -1),
                new Point3D(1, 1, -1), new Point3D(1, -1, -1),
            };

            foreach (Point3D point in points) mesh.Positions.Add(point);

            // define triangles
            Tuple<int, int, int>[] triangles =
            {
                 new Tuple<int, int, int>(0, 1, 2),
                 new Tuple<int, int, int>(2, 3, 0),
                 new Tuple<int, int, int>(4, 5, 6),
                 new Tuple<int, int, int>(6, 7, 4),
                 new Tuple<int, int, int>(8, 9, 10),
                 new Tuple<int, int, int>(10, 11, 8),
                 new Tuple<int, int, int>(12, 13, 14),
                 new Tuple<int, int, int>(14, 15, 12),
                 new Tuple<int, int, int>(16, 17, 18),
                 new Tuple<int, int, int>(18, 19, 16),
                 new Tuple<int, int, int>(20, 21, 22),
                 new Tuple<int, int, int>(22, 23, 20)
            };
            foreach (Tuple<int, int, int> tuple in triangles)
            {
                mesh.TriangleIndices.Add(tuple.Item1);
                mesh.TriangleIndices.Add(tuple.Item2);
                mesh.TriangleIndices.Add(tuple.Item3);
            }

            // define the boject's material
            DiffuseMaterial material = new DiffuseMaterial(Brushes.LightBlue);

            // create the model, which includes the geometry and material
            GeometryModel3D model = new GeometryModel3D(mesh, material);

            // add the model to the geometry group
            group.Children.Add(model);

        }

        private void DefineLights(Model3DGroup group)
        {
            group.Children.Add(new AmbientLight(Colors.Gray));

            Vector3D direction = new Vector3D(1, -2, -3);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction));
        }

        private void DefineCamera(Viewport3D viewport)
        {
            Point3D position = new Point3D(1.5, 2, 3);
            Vector3D lookDirection = new Vector3D(
                -position.X, -position.Y, -position.Z);
            Vector3D upDirection = new Vector3D(0, 1, 0);
            double fieldOfView = 10;
            OrthographicCamera camera =
                new OrthographicCamera(position, lookDirection, upDirection, fieldOfView);
            camera.FarPlaneDistance=5;
            camera.NearPlaneDistance=1;

            viewport.Camera = camera;

        }


    }
}
