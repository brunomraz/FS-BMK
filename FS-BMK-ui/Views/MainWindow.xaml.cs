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
            vm1 = (CurrentSuspensionViewModel)CurrSuspDataContext.DataContext;

        }

        private Model3DGroup xAxis, yAxis, zAxis, lca1Model, lca2Model, uca1Model, uca2Model, trModel, uprightModel, wheelModel;

        private CurrentSuspensionViewModel vm1;

        // The camera.
        private PerspectiveCamera TheCamera = null;

        // The camera controller.
        private SphericalCameraController CameraController = null;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            xAxis = new Model3DGroup();
            yAxis = new Model3DGroup();
            visual3d.Content = xAxis;
            //visual3d.Content = yAxis;
            mainViewport.Children.Add(visual3d);

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(xAxis);
            DefineModel(xAxis, Colors.Blue);
            xAxis.Transform = new MatrixTransform3D(new Matrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1));
            //DefineModel(yAxis, Colors.Red);
            //yAxis.Transform = new MatrixTransform3D(new Matrix3D(
            //    0, -1, 0, 0,
            //    1, 0, 0, 0,
            //    0, 0, 1, 0,
            //    0, 0, 0, 1));
        }

        // Define the camera.
        private void DefineCamera(Viewport3D viewport)
        {
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            CameraController = new SphericalCameraController
                (TheCamera, viewport, this, mainViewport, mainViewport);
        }



        // Define the lights.
        private void DefineLights(Model3DGroup group)
        {
            group.Children.Add(new AmbientLight(Colors.Gray));

            Vector3D direction = new Vector3D(1, -2, -3);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction));

            Vector3D direction2 = new Vector3D(0, -1, 0);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction2));
        }

        // Define the model.
        private void DefineModel(Model3DGroup group, Color color)
        {
            //// Make some cubes.
            //for (int x = -2; x <= 2; x += 2)
            //{
            //    for (int y = -2; y <= 2; y += 2)
            //    {
            //        for (int z = -2; z <= 2; z += 2)
            //        {
            //            MeshGeometry3D mesh = MakeCubeMesh(x, y, z, 1);

            //            byte r = (byte)(128 + x * 50);
            //            byte g = (byte)(128 + y * 50);
            //            byte b = (byte)(128 + z * 50);
            //            Color color = Color.FromArgb(255, r, g, b);
            //            DiffuseMaterial material = new DiffuseMaterial(
            //                new SolidColorBrush(color));

            //            GeometryModel3D model = new GeometryModel3D(mesh, material);
            //            group.Children.Add(model);
            //        }
            //    }
            //}
            MeshGeometry3D mesh = MakeCylinderMesh(3, 1, 3, 1, 2);
            //MeshGeometry3D mesh = MakeCubeMesh(0, 0, 0, 1);

            //byte r = (byte)(128 + 0 * 50);
            //byte g = (byte)(128 + 0 * 50);
            //byte b = (byte)(128 + 0* 50);
            //Color color = Color.FromArgb(255, r, g, b);
            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(color));

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            group.Children.Add(model);
        }

        // Make a mesh containing a cube centered at this point.
        private MeshGeometry3D MakeCubeMesh(double x, double y, double z, double width)
        {
            // Create the geometry.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Define the positions.
            width /= 2;
            Point3D[] points =
            {
                new Point3D(x - width, y - width, z - width),
                new Point3D(x + width, y - width, z - width),
                new Point3D(x + width, y - width, z + width),
                new Point3D(x - width, y - width, z + width),
                new Point3D(x - width, y - width, z + width),
                new Point3D(x + width, y - width, z + width),
                new Point3D(x + width, y + width, z + width),
                new Point3D(x - width, y + width, z + width),
                new Point3D(x + width, y - width, z + width),
                new Point3D(x + width, y - width, z - width),
                new Point3D(x + width, y + width, z - width),
                new Point3D(x + width, y + width, z + width),
                new Point3D(x + width, y + width, z + width),
                new Point3D(x + width, y + width, z - width),
                new Point3D(x - width, y + width, z - width),
                new Point3D(x - width, y + width, z + width),
                new Point3D(x - width, y - width, z + width),
                new Point3D(x - width, y + width, z + width),
                new Point3D(x - width, y + width, z - width),
                new Point3D(x - width, y - width, z - width),
                new Point3D(x - width, y - width, z - width),
                new Point3D(x - width, y + width, z - width),
                new Point3D(x + width, y + width, z - width),
                new Point3D(x + width, y - width, z - width),
            };
            foreach (Point3D point in points) mesh.Positions.Add(point);

            // Define the triangles.
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
                 new Tuple<int, int, int>(22, 23, 20),
            };
            foreach (Tuple<int, int, int> tuple in triangles)
            {
                mesh.TriangleIndices.Add(tuple.Item1);
                mesh.TriangleIndices.Add(tuple.Item2);
                mesh.TriangleIndices.Add(tuple.Item3);
            }

            return mesh;
        }

        private MeshGeometry3D MakeCylinderMesh(double length, double radius, int radialIncrements, int axialIncrements, int faceIncrements)
        {
            // Create the geometry.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Define the positions, there are separate points for faces and side wall.
            Point3D[] points = new Point3D[radialIncrements * (axialIncrements + 1) + 2 * (1 + radialIncrements * faceIncrements)];

            int bottomPtIndex = radialIncrements * (axialIncrements + 1);
            int topPtIndex = bottomPtIndex + radialIncrements * faceIncrements + 1;

            double angleSegment = 2 * Math.PI / radialIncrements;
            double heightSegment = length / axialIncrements;
            double faceSegment = radius / faceIncrements;

            // creates points for side wall mesh
            for (int j = 0; j < axialIncrements + 1; j++)
            {
                for (int i = 0; i < radialIncrements; i++)
                {
                    points[i + j * radialIncrements] =
                        new Point3D(
                            radius * Math.Cos(angleSegment * i),
                            radius * Math.Sin(angleSegment * i),
                            heightSegment * j);
                }
            }


            // creates points for faces
            points[bottomPtIndex] = new Point3D(0, 0, 0);
            points[topPtIndex] = new Point3D(0, 0, length);
            for (int j = 0; j < radialIncrements; j++)
            {

                for (int i = 0; i < faceIncrements; i++)
                {
                    // bottom face
                    points[bottomPtIndex + j * faceIncrements + 1 + i] = new Point3D(
                        faceSegment * (i + 1) * Math.Cos(angleSegment * j),
                        faceSegment * (i + 1) * Math.Sin(angleSegment * j),
                        0
                        );

                    // top face
                    points[topPtIndex + j * faceIncrements + 1 + i] = new Point3D(
                        faceSegment * (i + 1) * Math.Cos(angleSegment * j),
                        faceSegment * (i + 1) * Math.Sin(angleSegment * j),
                        length
                        );
                }
            }


            foreach (Point3D point in points) mesh.Positions.Add(point);

            //// Define the triangles.
            Tuple<int, int, int>[] triangles = new Tuple<int, int, int>[radialIncrements * 2 * axialIncrements];


            // create triangles for side wall
            for (int j = 0; j < axialIncrements; j++)
            {
                for (int i = 0; i < radialIncrements - 1; i++)
                {
                    triangles[2 * i + j * radialIncrements] =
                        new Tuple<int, int, int>(j * radialIncrements + i, j * radialIncrements + i + 1, (j + 1) * radialIncrements + i);
                    triangles[2 * i + 1 + j * radialIncrements] =
                        new Tuple<int, int, int>(j * radialIncrements + i + 1, (j + 1) * radialIncrements + i + 1, (j + 1) * radialIncrements + i);

                }

                triangles[2 * radialIncrements - 2 + j * radialIncrements] =
                        new Tuple<int, int, int>
                        (j * radialIncrements + radialIncrements - 1, j * radialIncrements, (j + 1) * radialIncrements + radialIncrements - 1);

                triangles[2 * radialIncrements - 1 + j * radialIncrements] =
                        new Tuple<int, int, int>
                        (j * radialIncrements, j * radialIncrements + radialIncrements, (j + 1) * radialIncrements + radialIncrements - 1);
            }

            // cerate triangles for faces
            for (int j=0;j<radialIncrements-1;j++)
            {
                // inner triangles bottom

                // inner triangles top

                for (int i = 0; i < faceIncrements - 1; i++)
                {
                    // outer triangles bottom


                    // outer triangles top

                    
                }

            }

            foreach (Tuple<int, int, int> tuple in triangles)
            {
                mesh.TriangleIndices.Add(tuple.Item1);
                mesh.TriangleIndices.Add(tuple.Item2);
                mesh.TriangleIndices.Add(tuple.Item3);
            }

            return mesh;
        }

        double t = 0.5;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Add the rotation transform to a Transform3DGroup
            Transform3DGroup myTransform3DGroup = new Transform3DGroup();

            // Create and apply a scale transformation that stretches the object along the local x-axis
            // by 200 percent and shrinks it along the local y-axis by 50 percent.
            ScaleTransform3D myScaleTransform3D = new ScaleTransform3D();
            myScaleTransform3D.ScaleX = vm1.TestVar;
            myScaleTransform3D.ScaleY = 0.5;
            myScaleTransform3D.ScaleZ = 1;

            // Add the scale transform to the Transform3DGroup.
            myTransform3DGroup.Children.Add(myScaleTransform3D);

            // Set the Transform property of the GeometryModel to the Transform3DGroup which includes
            // both transformations. The 3D object now has two Transformations applied to it.
            xAxis.Transform = myTransform3DGroup;
            xAxis.Transform = new MatrixTransform3D(
    new Matrix3D(
        t, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, t));
            t += 1;

        }
    }

}
