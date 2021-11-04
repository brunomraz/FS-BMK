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

        public  Model3DGroup suspensionGroup;
        private Model3DGroup lcaGroup, ucaGroup, trGroup, wheelAssyGroup;
        private GeometryModel3D lca1Model, lca2Model, uca1Model, uca2Model, trModel, uprightModel, wheelModel;

        private Model3DGroup axisSystem;

        private GeometryModel3D xAxis, yAxis, zAxis;

        private CurrentSuspensionViewModel vm1;

        // The camera.
        private OrthographicCamera TheCamera = null;

        // The camera controller.
        private SphericalCameraController CameraController = null;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Define WPF objects.
            ModelVisual3D visual3d = new ModelVisual3D();
            suspensionGroup = new Model3DGroup();

            lcaGroup = new Model3DGroup();
            ucaGroup = new Model3DGroup();
            trGroup = new Model3DGroup();
            wheelAssyGroup = new Model3DGroup();

            axisSystem = new Model3DGroup();

            xAxis = new GeometryModel3D();
            yAxis = new GeometryModel3D();
            zAxis = new GeometryModel3D();

            lca1Model = new GeometryModel3D();
            lca2Model = new GeometryModel3D();
            uca1Model = new GeometryModel3D();
            uca2Model = new GeometryModel3D();
            trModel = new GeometryModel3D();
            uprightModel = new GeometryModel3D();
            wheelModel = new GeometryModel3D();

            // Define the camera, lights, and model.
            DefineCamera(mainViewport);
            DefineLights(suspensionGroup);

            xAxis = DefineCylinderModel(Colors.Red, 0, 0, 0, 100, 0, 0, 5);
            xAxis.Transform = new MatrixTransform3D(new Matrix3D(
                0, 1, 0, 0,
                0, 0, 1, 0,
                5, 0, 0, 0,
                0, -vm1.CurrentSuspension.Hardpoints[8].YVal, 0, 1));
            yAxis = DefineCylinderModel(Colors.Green, 0, 0, 0, 100, 0, 0, 5);
            yAxis.Transform = new MatrixTransform3D(new Matrix3D(
                -1, 0, 0, 0,
                0, 0, 1, 0,
                0, 5, 0, 0,
                0, -vm1.CurrentSuspension.Hardpoints[8].YVal, 0, 1));
            zAxis = DefineCylinderModel(Colors.Blue, 0, 0, 0, 100, 0, 0, 5);
            zAxis.Transform = new MatrixTransform3D(new Matrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 5, 0,
                0, -vm1.CurrentSuspension.Hardpoints[8].YVal, 0, 1));

            axisSystem.Children.Add(xAxis);
            axisSystem.Children.Add(yAxis);
            axisSystem.Children.Add(zAxis);

            lca1Model = DefineCylinderModel2(Colors.Green, 5);
            lca1Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal,
                    vm1.CurrentSuspension.Hardpoints[2].XVal, vm1.CurrentSuspension.Hardpoints[2].YVal, vm1.CurrentSuspension.Hardpoints[2].ZVal,
                    vm1.CurrentSuspension.Hardpoints[1].XVal, vm1.CurrentSuspension.Hardpoints[1].YVal, vm1.CurrentSuspension.Hardpoints[1].ZVal
                );

            lca2Model = DefineCylinderModel2(Colors.Green, 5);
            lca2Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[1].XVal, vm1.CurrentSuspension.Hardpoints[1].YVal, vm1.CurrentSuspension.Hardpoints[1].ZVal,
                    vm1.CurrentSuspension.Hardpoints[2].XVal, vm1.CurrentSuspension.Hardpoints[2].YVal, vm1.CurrentSuspension.Hardpoints[2].ZVal,
                    vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal
                );


            uca1Model = DefineCylinderModel2(Colors.Red, 5);
            uca1Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[3].XVal, vm1.CurrentSuspension.Hardpoints[3].YVal, vm1.CurrentSuspension.Hardpoints[3].ZVal,
                    vm1.CurrentSuspension.Hardpoints[5].XVal, vm1.CurrentSuspension.Hardpoints[5].YVal, vm1.CurrentSuspension.Hardpoints[5].ZVal,
                    vm1.CurrentSuspension.Hardpoints[4].XVal, vm1.CurrentSuspension.Hardpoints[4].YVal, vm1.CurrentSuspension.Hardpoints[4].ZVal
                );


            uca2Model = DefineCylinderModel2(Colors.Red, 5);
            uca2Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[4].XVal, vm1.CurrentSuspension.Hardpoints[4].YVal, vm1.CurrentSuspension.Hardpoints[4].ZVal,
                    vm1.CurrentSuspension.Hardpoints[5].XVal, vm1.CurrentSuspension.Hardpoints[5].YVal, vm1.CurrentSuspension.Hardpoints[5].ZVal,
                    vm1.CurrentSuspension.Hardpoints[3].XVal, vm1.CurrentSuspension.Hardpoints[3].YVal, vm1.CurrentSuspension.Hardpoints[3].ZVal
                );


            trModel = DefineCylinderModel2(Colors.Cyan, 5);
            trModel.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[6].XVal, vm1.CurrentSuspension.Hardpoints[6].YVal, vm1.CurrentSuspension.Hardpoints[6].ZVal,
                    vm1.CurrentSuspension.Hardpoints[7].XVal, vm1.CurrentSuspension.Hardpoints[7].YVal, vm1.CurrentSuspension.Hardpoints[7].ZVal,
                    vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal
                );


            wheelModel = DefineWheelModel(Colors.Black);
            wheelModel.Transform = TransformHollowCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[8].XVal, vm1.CurrentSuspension.Hardpoints[8].YVal, vm1.CurrentSuspension.Hardpoints[8].ZVal,
                    vm1.CurrentSuspension.Hardpoints[9].XVal, vm1.CurrentSuspension.Hardpoints[9].YVal, vm1.CurrentSuspension.Hardpoints[9].ZVal,
                    vm1.CurrentSuspension.Hardpoints[7].XVal, vm1.CurrentSuspension.Hardpoints[7].YVal, vm1.CurrentSuspension.Hardpoints[7].ZVal
                );

            uprightModel = DefineUprightModel(Colors.Blue, 
                new Point3D(vm1.CurrentSuspension.Hardpoints[2].XVal, vm1.CurrentSuspension.Hardpoints[2].YVal, vm1.CurrentSuspension.Hardpoints[2].ZVal),
                new Point3D(vm1.CurrentSuspension.Hardpoints[5].XVal, vm1.CurrentSuspension.Hardpoints[5].YVal, vm1.CurrentSuspension.Hardpoints[5].ZVal),
                new Point3D(vm1.CurrentSuspension.Hardpoints[8].XVal, vm1.CurrentSuspension.Hardpoints[8].YVal, vm1.CurrentSuspension.Hardpoints[8].ZVal),
                new Point3D(vm1.CurrentSuspension.Hardpoints[7].XVal, vm1.CurrentSuspension.Hardpoints[7].YVal, vm1.CurrentSuspension.Hardpoints[7].ZVal));



            lcaGroup.Children.Add(lca1Model);
            lcaGroup.Children.Add(lca2Model);
            ucaGroup.Children.Add(uca1Model);
            ucaGroup.Children.Add(uca2Model);
            trGroup.Children.Add(trModel);
            wheelAssyGroup.Children.Add(uprightModel);
            wheelAssyGroup.Children.Add(wheelModel);

            suspensionGroup.Children.Add(axisSystem);
            suspensionGroup.Children.Add(lcaGroup);
            suspensionGroup.Children.Add(ucaGroup);
            suspensionGroup.Children.Add(trGroup);
            suspensionGroup.Children.Add(wheelAssyGroup);

            visual3d.Content = suspensionGroup;
            mainViewport.Children.Add(visual3d);

        }

        private MatrixTransform3D TransformCylindricalModel(
            float startx, float starty, float startz,
            float endx, float endy, float endz,
            float orientx, float orienty, float orientz)
        {
            MatrixTransform3D transformMatrix;
            Vector3D v1 = new Vector3D(endx - startx, endy - starty, endz - startz);
            Vector3D v2 = Vector3D.CrossProduct(v1, new Vector3D(orientx - startx, orienty - starty, orientz - startz));
            Vector3D v3 = Vector3D.CrossProduct(v1, v2);
            double length = v1.Length;
            v1.Normalize();
            v2.Normalize();
            v3.Normalize();

            transformMatrix = new MatrixTransform3D(new Matrix3D(
                v3.X, v3.Y, v3.Z, 0,
                v2.X, v2.Y, v2.Z, 0,
                v1.X * length, v1.Y * length, v1.Z * length, 0,
                 startx - vm1.CurrentSuspension.Hardpoints[8].XVal, starty- vm1.CurrentSuspension.Hardpoints[8].YVal, startz - vm1.CurrentSuspension.Hardpoints[8].ZVal, 1
                ));

            return transformMatrix;
        }

        private MatrixTransform3D TransformHollowCylindricalModel(
            float startx, float starty, float startz,
            float endx, float endy, float endz,
            float orientx, float orienty, float orientz)
        {
            MatrixTransform3D transformMatrix;
            Vector3D v1 = new Vector3D(endx - startx, endy - starty, endz - startz);
            Vector3D v2 = Vector3D.CrossProduct(v1, new Vector3D(orientx - startx, orienty - starty, orientz - startz));
            Vector3D v3 = Vector3D.CrossProduct(v1, v2);
            double length = v1.Length;
            v1.Normalize();
            v2.Normalize();
            v3.Normalize();

            transformMatrix = new MatrixTransform3D(new Matrix3D(
                v3.X, v3.Y, v3.Z, 0,
                v2.X, v2.Y, v2.Z, 0,
                v1.X, v1.Y, v1.Z, 0,
                startx-vm1.CurrentSuspension.Hardpoints[8].XVal, starty- vm1.CurrentSuspension.Hardpoints[8].YVal, startz - vm1.CurrentSuspension.Hardpoints[8].ZVal, 1
                ));

            return transformMatrix;
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

        private void DefineLights(Model3DGroup group)
        {
            group.Children.Add(new AmbientLight(Colors.Gray));

            Vector3D direction = new Vector3D(1, -2, -3);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction));

            Vector3D direction2 = new Vector3D(0, -1, 0);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction2));

            group.Children.Add(new PointLight(Colors.Gray, new Point3D(vm1.CurrentSuspension.Hardpoints[8].XVal, vm1.CurrentSuspension.Hardpoints[8].XVal - 500, vm1.CurrentSuspension.Hardpoints[8].XVal - 500)));
        }

        private GeometryModel3D DefineUprightModel(Color color, Point3D lca3, Point3D uca3, Point3D wcn, Point3D tr2)
        {
            //MeshGeometry3D mesh = MakeUprightMesh(
            //    new Point3D(vm1.CurrentSuspension.Hardpoints[2].XVal, vm1.CurrentSuspension.Hardpoints[2].YVal, vm1.CurrentSuspension.Hardpoints[2].ZVal),
            //    new Point3D(vm1.CurrentSuspension.Hardpoints[5].XVal, vm1.CurrentSuspension.Hardpoints[5].YVal, vm1.CurrentSuspension.Hardpoints[5].ZVal),
            //    new Point3D(vm1.CurrentSuspension.Hardpoints[8].XVal, vm1.CurrentSuspension.Hardpoints[8].YVal, vm1.CurrentSuspension.Hardpoints[8].ZVal),
            //    new Point3D(vm1.CurrentSuspension.Hardpoints[7].XVal, vm1.CurrentSuspension.Hardpoints[7].YVal, vm1.CurrentSuspension.Hardpoints[7].ZVal)
            //    );           
            MeshGeometry3D mesh = MakeUprightMesh(
                lca3,
                uca3,
                wcn,
                tr2
                );

            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(color));

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

        private GeometryModel3D DefineCylinderModel(Color color, double startx, double starty, double startz, double endx, double endy, double endz, double radius)
        {
            double length = new Vector3D(startx - endx, starty - endy, startz - endz).Length;
            MeshGeometry3D mesh = MakeCylinderMesh(length, radius, 40, 10, 20);

            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(color));

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

        private GeometryModel3D DefineCylinderModel2(Color color, double radius)
        {
            MeshGeometry3D mesh = MakeCylinderMesh(1, radius, 40, 10, 20);

            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(color));

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

        private GeometryModel3D DefineWheelModel(Color color)
        {

            MeshGeometry3D mesh = MakeHollowCylinderMesh(
                vm1.CurrentSuspension.WheelWidth, vm1.CurrentSuspension.WheelRadius, vm1.CurrentSuspension.WheelInsideRadius, 40, 10, 20);

            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(Color.FromArgb(200, 50, 50, 50)));


            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

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
            Tuple<int, int, int>[] triangles = new Tuple<int, int, int>[
                radialIncrements * 2 * axialIncrements + 2 * (radialIncrements + (faceIncrements - 1) * radialIncrements * 2)];
            int bottomTriangleIndex = radialIncrements * 2 * axialIncrements;
            int topTriangleIndex = radialIncrements * 2 * axialIncrements + radialIncrements + (faceIncrements - 1) * radialIncrements * 2;



            // create triangles for side wall
            for (int j = 0; j < axialIncrements; j++)
            {
                for (int i = 0; i < radialIncrements - 1; i++)
                {
                    triangles[2 * i + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(j * radialIncrements + i, j * radialIncrements + i + 1, (j + 1) * radialIncrements + i);
                    triangles[2 * i + 1 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(j * radialIncrements + i + 1, (j + 1) * radialIncrements + i + 1, (j + 1) * radialIncrements + i);

                }

                triangles[2 * radialIncrements - 2 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>
                        (j * radialIncrements + radialIncrements - 1, j * radialIncrements, (j + 1) * radialIncrements + radialIncrements - 1);

                triangles[2 * radialIncrements - 1 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>
                        (j * radialIncrements, j * radialIncrements + radialIncrements, (j + 1) * radialIncrements + radialIncrements - 1);
            }

            // cerate triangles for faces
            for (int j = 0; j < radialIncrements - 1; j++)
            {
                // inner triangles bottom
                int currentBottomInnerTriangleIndex = bottomTriangleIndex + j * (1 + (faceIncrements - 1) * 2);
                triangles[currentBottomInnerTriangleIndex] =
                        new Tuple<int, int, int>(
                            bottomPtIndex,
                            bottomPtIndex + faceIncrements * (j + 1) + 1,
                            bottomPtIndex + faceIncrements * j + 1);

                // inner triangles top
                int currentTopInnerTriangleIndex = topTriangleIndex + j * (1 + (faceIncrements - 1) * 2);
                triangles[currentTopInnerTriangleIndex] =
                        new Tuple<int, int, int>(
                            topPtIndex,
                            topPtIndex + faceIncrements * j + 1,
                            topPtIndex + faceIncrements * (j + 1) + 1);

                for (int i = 0; i < faceIncrements - 1; i++)
                {
                    // outer triangles bottom
                    triangles[currentBottomInnerTriangleIndex + 2 * i + 1] =
                        new Tuple<int, int, int>(
                            bottomPtIndex + j * faceIncrements + i + 1,
                            bottomPtIndex + (j + 1) * faceIncrements + i + 1,
                            bottomPtIndex + j * faceIncrements + i + 1 + 1
                            );
                    triangles[currentBottomInnerTriangleIndex + 2 * i + 2] =
                        new Tuple<int, int, int>(
                            1 + bottomPtIndex + i + (j + 1) * faceIncrements,
                            1 + bottomPtIndex + i + 1 + (j + 1) * faceIncrements,
                            1 + bottomPtIndex + i + 1 + j * faceIncrements
                            );
                    // outer triangles top
                    triangles[currentTopInnerTriangleIndex + 2 * i + 1] =
                        new Tuple<int, int, int>(
                            topPtIndex + j * faceIncrements + i + 1,
                            topPtIndex + j * faceIncrements + i + 1 + 1,
                            topPtIndex + (j + 1) * faceIncrements + i + 1
                            );
                    triangles[currentTopInnerTriangleIndex + 2 * i + 2] =
                        new Tuple<int, int, int>(
                            1 + topPtIndex + i + (j + 1) * faceIncrements,
                            1 + topPtIndex + i + 1 + j * faceIncrements,
                            1 + topPtIndex + i + 1 + (j + 1) * faceIncrements
                            );

                }

            }

            // last inner triangle
            int lastBottomInnerTriangleIndex = bottomTriangleIndex + (radialIncrements - 1) * (1 + (faceIncrements - 1) * 2);
            int lastTopInnerTriangleIndex = topTriangleIndex + (radialIncrements - 1) * (1 + (faceIncrements - 1) * 2);
            triangles[lastBottomInnerTriangleIndex] =
                new Tuple<int, int, int>(
                    bottomPtIndex,
                    bottomPtIndex + 1,
                    bottomPtIndex + (radialIncrements - 1) * faceIncrements + 1
                    );
            triangles[lastTopInnerTriangleIndex] =
                new Tuple<int, int, int>(
                    topPtIndex,
                    topPtIndex + (radialIncrements - 1) * faceIncrements + 1,
                    topPtIndex + 1
                    );


            // last outer triangles
            for (int i = 0; i < faceIncrements - 1; i++)
            {
                // last radial section outer bottom triangles
                triangles[lastBottomInnerTriangleIndex + 2 * i + 1] =
                    new Tuple<int, int, int>(
                        bottomPtIndex + (radialIncrements - 1) * faceIncrements + 1 + i,
                        bottomPtIndex + 1 + i,
                        bottomPtIndex + (radialIncrements - 1) * faceIncrements + 1 + i + 1
                        );
                triangles[lastBottomInnerTriangleIndex + 2 * i + 2] =
                    new Tuple<int, int, int>(
                        bottomPtIndex + (radialIncrements - 1) * faceIncrements + 1 + i + 1,
                        bottomPtIndex + 1 + i,
                        bottomPtIndex + 1 + i + 1
                        );

                // last radial section outer top triangles
                triangles[lastTopInnerTriangleIndex + 2 * i + 1] =
                    new Tuple<int, int, int>(
                        topPtIndex + (radialIncrements - 1) * faceIncrements + 1 + i,
                        topPtIndex + (radialIncrements - 1) * faceIncrements + 1 + i + 1,
                        topPtIndex + 1 + i
                        );
                triangles[lastTopInnerTriangleIndex + 2 * i + 2] =
                    new Tuple<int, int, int>(
                        topPtIndex + (radialIncrements - 1) * faceIncrements + 1 + i + 1,
                        topPtIndex + 1 + i + 1,
                        topPtIndex + 1 + i
                        );
            }





            // add triangles to mesh
            foreach (Tuple<int, int, int> tuple in triangles)
            {
                mesh.TriangleIndices.Add(tuple.Item1);
                mesh.TriangleIndices.Add(tuple.Item2);
                mesh.TriangleIndices.Add(tuple.Item3);
            }

            return mesh;
        }

        private MeshGeometry3D MakeHollowCylinderMesh(double length, double outerRadius, double innerRadius, int radialIncrements, int axialIncrements, int faceIncrements)
        {
            // Create the geometry.
            MeshGeometry3D mesh = new MeshGeometry3D();

            // Define the positions, there are separate points for faces and side wall.
            Point3D[] points = new Point3D[radialIncrements * (axialIncrements + 1) * 2 + 2 * radialIncrements * (faceIncrements + 1)];

            int bottomPtIndex = radialIncrements * (axialIncrements + 1);
            int topPtIndex = bottomPtIndex + radialIncrements * (faceIncrements + 1);
            int innerSidePtIndex = topPtIndex + radialIncrements * (faceIncrements + 1);

            double angleSegment = 2 * Math.PI / radialIncrements;
            double lengthSegment = length / axialIncrements;
            double faceSegment = (outerRadius - innerRadius) / faceIncrements;

            // creates points for outer and inner side wall mesh
            for (int j = 0; j < axialIncrements + 1; j++)
            {
                for (int i = 0; i < radialIncrements; i++)
                {
                    // outer side wall
                    points[i + j * radialIncrements] =
                        new Point3D(
                            outerRadius * Math.Cos(angleSegment * i),
                            outerRadius * Math.Sin(angleSegment * i),
                            lengthSegment * j - length/2);

                    // inner side wall
                    points[innerSidePtIndex + i + j * radialIncrements] =
                        new Point3D(
                            innerRadius * Math.Cos(angleSegment * i),
                            innerRadius * Math.Sin(angleSegment * i),
                            lengthSegment * j - length/2);
                }
            }


            // creates points for faces
            for (int j = 0; j < radialIncrements; j++)
            {

                for (int i = 0; i < faceIncrements + 1; i++)
                {
                    // bottom face
                    points[bottomPtIndex + j * (faceIncrements + 1) + i] = new Point3D(
                        (innerRadius + faceSegment * i) * Math.Cos(angleSegment * j),
                        (innerRadius + faceSegment * i) * Math.Sin(angleSegment * j),
                        0-length/2
                        );

                    // top face
                    points[topPtIndex + j * (faceIncrements + 1) + i] = new Point3D(
                        (innerRadius + faceSegment * i) * Math.Cos(angleSegment * j),
                        (innerRadius + faceSegment * i) * Math.Sin(angleSegment * j),
                        length/2
                        );
                }
            }


            foreach (Point3D point in points) mesh.Positions.Add(point);

            //// Define the triangles.
            Tuple<int, int, int>[] triangles = new Tuple<int, int, int>[
                radialIncrements * 2 * axialIncrements * 2 + 2 * faceIncrements * radialIncrements * 2];
            int bottomTriangleIndex = radialIncrements * 2 * axialIncrements;
            int topTriangleIndex = bottomTriangleIndex + faceIncrements * radialIncrements * 2;
            int innerSideTriangleIndex = topTriangleIndex + faceIncrements * radialIncrements * 2;


            // create triangles for side wall
            for (int j = 0; j < axialIncrements; j++)
            {
                for (int i = 0; i < radialIncrements - 1; i++)
                {
                    // outer side wall
                    triangles[2 * i + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            j * radialIncrements + i,
                            j * radialIncrements + i + 1,
                            (j + 1) * radialIncrements + i);
                    triangles[2 * i + 1 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            j * radialIncrements + i + 1,
                            (j + 1) * radialIncrements + i + 1,
                            (j + 1) * radialIncrements + i);

                    // inner side wall
                    triangles[innerSideTriangleIndex + 2 * i + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            innerSidePtIndex + j * radialIncrements + i,
                            innerSidePtIndex + (j + 1) * radialIncrements + i,
                            innerSidePtIndex + j * radialIncrements + i + 1);
                    triangles[innerSideTriangleIndex + 2 * i + 1 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            innerSidePtIndex + j * radialIncrements + i + 1,
                            innerSidePtIndex + (j + 1) * radialIncrements + i,
                            innerSidePtIndex + (j + 1) * radialIncrements + i + 1);

                }

                // outer side wall
                triangles[2 * radialIncrements - 2 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            j * radialIncrements + radialIncrements - 1,
                            j * radialIncrements,
                            (j + 1) * radialIncrements + radialIncrements - 1);

                triangles[2 * radialIncrements - 1 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            j * radialIncrements,
                            j * radialIncrements + radialIncrements,
                            (j + 1) * radialIncrements + radialIncrements - 1);

                // inner side wall
                triangles[innerSideTriangleIndex + 2 * radialIncrements - 2 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            innerSidePtIndex + j * radialIncrements + radialIncrements - 1,
                            innerSidePtIndex + (j + 1) * radialIncrements + radialIncrements - 1,
                            innerSidePtIndex + j * radialIncrements);

                triangles[innerSideTriangleIndex + 2 * radialIncrements - 1 + 2 * j * radialIncrements] =
                        new Tuple<int, int, int>(
                            innerSidePtIndex + j * radialIncrements,
                            innerSidePtIndex + (j + 1) * radialIncrements + radialIncrements - 1,
                            innerSidePtIndex + j * radialIncrements + radialIncrements);
            }

            // cerate triangles for faces
            for (int j = 0; j < radialIncrements - 1; j++)
            {
                // inner triangles bottom
                int currentBottomInnerTriangleIndex = bottomTriangleIndex + j * faceIncrements * 2;

                // inner triangles top
                int currentTopInnerTriangleIndex = topTriangleIndex + j * faceIncrements * 2;

                for (int i = 0; i < faceIncrements; i++)
                {
                    // outer triangles bottom
                    triangles[currentBottomInnerTriangleIndex + 2 * i] =
                        new Tuple<int, int, int>(
                            bottomPtIndex + j * (faceIncrements + 1) + i,
                            bottomPtIndex + (j + 1) * (faceIncrements + 1) + i,
                            bottomPtIndex + j * (faceIncrements + 1) + i + 1
                            );
                    triangles[currentBottomInnerTriangleIndex + 2 * i + 1] =
                        new Tuple<int, int, int>(
                            bottomPtIndex + (j + 1) * (faceIncrements + 1) + i,
                            bottomPtIndex + (j + 1) * (faceIncrements + 1) + i + 1,
                            bottomPtIndex + j * (faceIncrements + 1) + i + 1
                            );
                    // outer triangles top
                    triangles[currentTopInnerTriangleIndex + 2 * i] =
                        new Tuple<int, int, int>(
                            topPtIndex + j * (faceIncrements + 1) + i,
                            topPtIndex + j * (faceIncrements + 1) + i + 1,
                            topPtIndex + (j + 1) * (faceIncrements + 1) + i
                            );
                    triangles[currentTopInnerTriangleIndex + 2 * i + 1] =
                        new Tuple<int, int, int>(
                            topPtIndex + (j + 1) * (faceIncrements + 1) + i,
                            topPtIndex + j * (faceIncrements + 1) + i + 1,
                            topPtIndex + (j + 1) * (faceIncrements + 1) + i + 1
                            );

                }

            }

            // last inner triangle
            int lastBottomInnerTriangleIndex = bottomTriangleIndex + (radialIncrements - 1) * faceIncrements * 2;
            int lastTopInnerTriangleIndex = topTriangleIndex + (radialIncrements - 1) * faceIncrements * 2;


            // last outer triangles
            for (int i = 0; i < faceIncrements; i++)
            {
                // last radial section outer bottom triangles
                triangles[lastBottomInnerTriangleIndex + 2 * i] =
                    new Tuple<int, int, int>(
                        bottomPtIndex + (radialIncrements - 1) * (faceIncrements + 1) + i,
                        bottomPtIndex + i,
                        bottomPtIndex + (radialIncrements - 1) * (faceIncrements + 1) + i + 1
                        );
                triangles[lastBottomInnerTriangleIndex + 2 * i + 1] =
                    new Tuple<int, int, int>(
                        bottomPtIndex + (radialIncrements - 1) * (faceIncrements + 1) + 1 + i,
                        bottomPtIndex + i,
                        bottomPtIndex + i + 1
                        );

                // last radial section outer top triangles
                triangles[lastTopInnerTriangleIndex + 2 * i] =
                    new Tuple<int, int, int>(
                        topPtIndex + (radialIncrements - 1) * (faceIncrements + 1) + i,
                        topPtIndex + (radialIncrements - 1) * (faceIncrements + 1) + i + 1,
                        topPtIndex + i
                        );
                triangles[lastTopInnerTriangleIndex + 2 * i + 1] =
                    new Tuple<int, int, int>(
                        topPtIndex + (radialIncrements - 1) * (faceIncrements + 1) + 1 + i,
                        topPtIndex + i + 1,
                        topPtIndex + i
                        );
            }





            // add triangles to mesh
            foreach (Tuple<int, int, int> tuple in triangles)
            {
                mesh.TriangleIndices.Add(tuple.Item1);
                mesh.TriangleIndices.Add(tuple.Item2);
                mesh.TriangleIndices.Add(tuple.Item3);
            }

            return mesh;
        }

        private MeshGeometry3D MakeUprightMesh(Point3D lca3, Point3D uca3, Point3D wcn, Point3D tr2)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D offset = new Point3D(vm1.CurrentSuspension.Hardpoints[8].XVal, vm1.CurrentSuspension.Hardpoints[8].YVal, vm1.CurrentSuspension.Hardpoints[8].ZVal);


            Point3D[] points =
            {
                 (Point3D)(lca3-offset),
                 (Point3D)(uca3-offset),
                 (Point3D)(wcn - offset),
                 (Point3D)(tr2-offset)
            };

            foreach (Point3D point in points) mesh.Positions.Add(point);

            // Define the triangles.
            Tuple<int, int, int>[] triangles =
            {
                 new Tuple<int, int, int>(0, 2, 1),
                 new Tuple<int, int, int>(2, 3, 1),
                 new Tuple<int, int, int>(0, 3, 2),
                 new Tuple<int, int, int>(0, 1, 3)
            };
            foreach (Tuple<int, int, int> tuple in triangles)
            {
                mesh.TriangleIndices.Add(tuple.Item1);
                mesh.TriangleIndices.Add(tuple.Item2);
                mesh.TriangleIndices.Add(tuple.Item3);
            }
            return mesh;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm1.CalculateSuspensionMovement2();

            wheelAssyGroup.Children.Clear();

            uprightModel = DefineUprightModel(Colors.Blue, 
                new Point3D(vm1.CurrentSuspension.HardpointsMoved[0], vm1.CurrentSuspension.HardpointsMoved[1], vm1.CurrentSuspension.HardpointsMoved[2]),   // LCA3
                new Point3D(vm1.CurrentSuspension.HardpointsMoved[3], vm1.CurrentSuspension.HardpointsMoved[4], vm1.CurrentSuspension.HardpointsMoved[5]),   // UCA3
                new Point3D(vm1.CurrentSuspension.HardpointsMoved[9], vm1.CurrentSuspension.HardpointsMoved[10], vm1.CurrentSuspension.HardpointsMoved[11]), // WCN
                new Point3D(vm1.CurrentSuspension.HardpointsMoved[6], vm1.CurrentSuspension.HardpointsMoved[7], vm1.CurrentSuspension.HardpointsMoved[8])    // TR2
                );
            wheelAssyGroup.Children.Add(uprightModel);

            wheelModel = DefineWheelModel(Colors.Black);



            wheelModel.Transform = TransformHollowCylindricalModel(
                    vm1.CurrentSuspension.HardpointsMoved[9], vm1.CurrentSuspension.HardpointsMoved[10], vm1.CurrentSuspension.HardpointsMoved[11],    // WCN
                    vm1.CurrentSuspension.HardpointsMoved[12], vm1.CurrentSuspension.HardpointsMoved[13], vm1.CurrentSuspension.HardpointsMoved[14],   // SPN
                    vm1.CurrentSuspension.HardpointsMoved[6], vm1.CurrentSuspension.HardpointsMoved[7], vm1.CurrentSuspension.HardpointsMoved[8]       // TR2
                );

            wheelAssyGroup.Children.Add(wheelModel);

            lca1Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal,   // LCA1
                    vm1.CurrentSuspension.HardpointsMoved[0], vm1.CurrentSuspension.HardpointsMoved[1], vm1.CurrentSuspension.HardpointsMoved[2],   // LCA3
                    vm1.CurrentSuspension.Hardpoints[1].XVal, vm1.CurrentSuspension.Hardpoints[1].YVal, vm1.CurrentSuspension.Hardpoints[1].ZVal    // LCA2
                );

            lca2Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[1].XVal, vm1.CurrentSuspension.Hardpoints[1].YVal, vm1.CurrentSuspension.Hardpoints[1].ZVal,   // LCA2
                    vm1.CurrentSuspension.HardpointsMoved[0], vm1.CurrentSuspension.HardpointsMoved[1], vm1.CurrentSuspension.HardpointsMoved[2],   // LCA3
                    vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal    // LCA1
                );

            uca1Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[3].XVal, vm1.CurrentSuspension.Hardpoints[3].YVal, vm1.CurrentSuspension.Hardpoints[3].ZVal,   // UCA1
                    vm1.CurrentSuspension.HardpointsMoved[3], vm1.CurrentSuspension.HardpointsMoved[4], vm1.CurrentSuspension.HardpointsMoved[5],   // UCA3
                    vm1.CurrentSuspension.Hardpoints[4].XVal, vm1.CurrentSuspension.Hardpoints[4].YVal, vm1.CurrentSuspension.Hardpoints[4].ZVal    // UCA2
                );

            uca2Model.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[4].XVal, vm1.CurrentSuspension.Hardpoints[4].YVal, vm1.CurrentSuspension.Hardpoints[4].ZVal,   // UCA2
                    vm1.CurrentSuspension.HardpointsMoved[3], vm1.CurrentSuspension.HardpointsMoved[4], vm1.CurrentSuspension.HardpointsMoved[5],   // UCA3
                    vm1.CurrentSuspension.Hardpoints[3].XVal, vm1.CurrentSuspension.Hardpoints[3].YVal, vm1.CurrentSuspension.Hardpoints[3].ZVal    // UCA1
                );

            trModel.Transform = TransformCylindricalModel(
                    vm1.CurrentSuspension.Hardpoints[6].XVal, vm1.CurrentSuspension.Hardpoints[6].YVal, vm1.CurrentSuspension.Hardpoints[6].ZVal,   // TR1
                    vm1.CurrentSuspension.HardpointsMoved[6], vm1.CurrentSuspension.HardpointsMoved[7], vm1.CurrentSuspension.HardpointsMoved[8],   // TR2
                    vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal    // LCA1
                );

        }
    }
}
