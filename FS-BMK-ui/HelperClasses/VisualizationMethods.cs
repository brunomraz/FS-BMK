using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace FS_BMK_ui.HelperClasses
{
    public static class VisualizationMethods
    {
        #region Mesh Methods
        public static MeshGeometry3D MakeCubeMesh(double x, double y, double z, double width)
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

        public static MeshGeometry3D MakeCylinderMesh(double length, double radius, int radialIncrements, int axialIncrements, int faceIncrements)
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

        public static MeshGeometry3D MakeHollowCylinderMesh(double length, double outerRadius, double innerRadius, int radialIncrements, int axialIncrements, int faceIncrements)
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
                            lengthSegment * j - length / 2);

                    // inner side wall
                    points[innerSidePtIndex + i + j * radialIncrements] =
                        new Point3D(
                            innerRadius * Math.Cos(angleSegment * i),
                            innerRadius * Math.Sin(angleSegment * i),
                            lengthSegment * j - length / 2);
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
                        0 - length / 2
                        );

                    // top face
                    points[topPtIndex + j * (faceIncrements + 1) + i] = new Point3D(
                        (innerRadius + faceSegment * i) * Math.Cos(angleSegment * j),
                        (innerRadius + faceSegment * i) * Math.Sin(angleSegment * j),
                        length / 2
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

        public static MeshGeometry3D MakeUprightMesh(Point3D lca3, Point3D uca3, Point3D wcn, Point3D tr2, Point3D offset)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            Point3D[] points =
            {
                 (Point3D)(lca3 - offset),
                 (Point3D)(uca3 - offset),
                 (Point3D)(wcn - offset),
                 (Point3D)(tr2 - offset)
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
        #endregion

        #region Create Model Methods
        public static GeometryModel3D DefineCylinderModel(Color color, double startx, double starty, double startz, double endx, double endy, double endz, double radius)
        {
            double length = new Vector3D(startx - endx, starty - endy, startz - endz).Length;
            MeshGeometry3D mesh = VisualizationMethods.MakeCylinderMesh(length, radius, 40, 10, 20);

            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(color));

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

        public static GeometryModel3D DefineCylinderModel2(Color color, double radius)
        {
            MeshGeometry3D mesh = VisualizationMethods.MakeCylinderMesh(1, radius, 40, 10, 20);

            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(color));

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

        public static GeometryModel3D DefineUprightModel(Color color, Point3D lca3, Point3D uca3, Point3D wcn, Point3D tr2, Point3D offset)
        {
            MeshGeometry3D mesh = VisualizationMethods.MakeUprightMesh(lca3, uca3, wcn, tr2, offset);

            DiffuseMaterial material = new DiffuseMaterial(new SolidColorBrush(color));

            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }

        public static GeometryModel3D DefineWheelModel(Color color, float width, float outsideRadius, float insideRadius)
        {

            MeshGeometry3D mesh = VisualizationMethods.MakeHollowCylinderMesh(width, outsideRadius, insideRadius, 40, 10, 20);

            DiffuseMaterial material = new DiffuseMaterial(
                new SolidColorBrush(Color.FromArgb(200, 50, 50, 50)));


            GeometryModel3D model = new GeometryModel3D(mesh, material);
            return model;
        }
        #endregion

        #region Transformation methods
        public static  MatrixTransform3D TransformCylindricalModel(
            float startx, float starty, float startz,
            float endx, float endy, float endz,
            float orientx, float orienty, float orientz,
            float offsetx, float offsety, float offsetz)
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
                 startx - offsetx, starty - offsety, startz - offsetz, 1
                ));

            return transformMatrix;
        }

        public static MatrixTransform3D TransformHollowCylindricalModel(
            float startx, float starty, float startz,
            float endx, float endy, float endz,
            float orientx, float orienty, float orientz,
            float offsetx, float offsety, float offsetz)
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
                startx - offsetx, starty - offsety, startz - offsetz, 1
                ));

            return transformMatrix;
        }
        #endregion
    }
}
