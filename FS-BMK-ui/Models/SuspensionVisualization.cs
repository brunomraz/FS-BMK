using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using FS_BMK_ui.CameraController;
using FS_BMK_ui.HelperClasses;


namespace FS_BMK_ui.Models
{
    class SuspensionVisualization
    {
        private Model3DGroup suspensionGroup;
        private Model3DGroup lcaGroup, ucaGroup, trGroup, wheelAssyGroup;
        private GeometryModel3D lca1Model, lca2Model, uca1Model, uca2Model, trModel, uprightModel, wheelModel;
        private CurrentSuspension _currSusp;
        private Model3DGroup axisSystem;

        private GeometryModel3D xAxis, yAxis, zAxis;


        public Model3DGroup SuspensionGroup 
        {
            get { return suspensionGroup; }
        }

        public SuspensionVisualization(CurrentSuspension currentSuspension)
        {
            _currSusp = currentSuspension;
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

            DefineLights(suspensionGroup);
            xAxis = VisualizationMethods.DefineCylinderModel(Colors.Red, 0, 0, 0, 100, 0, 0, 5);
            xAxis.Transform = new MatrixTransform3D(new Matrix3D(
                0, 1, 0, 0,
                0, 0, 1, 0,
                5, 0, 0, 0,
                0, -_currSusp.Hardpoints[8].Y, 0, 1));
            yAxis = VisualizationMethods.DefineCylinderModel(Colors.Green, 0, 0, 0, 100, 0, 0, 5);
            yAxis.Transform = new MatrixTransform3D(new Matrix3D(
                -1, 0, 0, 0,
                0, 0, 1, 0,
                0, 5, 0, 0,
                0, -_currSusp.Hardpoints[8].Y, 0, 1));
            zAxis = VisualizationMethods.DefineCylinderModel(Colors.Blue, 0, 0, 0, 100, 0, 0, 5);
            zAxis.Transform = new MatrixTransform3D(new Matrix3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 5, 0,
                0, -_currSusp.Hardpoints[8].Y, 0, 1));

            axisSystem.Children.Add(xAxis);
            axisSystem.Children.Add(yAxis);
            axisSystem.Children.Add(zAxis);

            lca1Model = VisualizationMethods.DefineCylinderModel2(Colors.Green, 5);
            lca1Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[0].X, _currSusp.Hardpoints[0].Y, _currSusp.Hardpoints[0].Z,
                    _currSusp.Hardpoints[2].X, _currSusp.Hardpoints[2].Y, _currSusp.Hardpoints[2].Z,
                    _currSusp.Hardpoints[1].X, _currSusp.Hardpoints[1].Y, _currSusp.Hardpoints[1].Z,
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );

            lca2Model = VisualizationMethods.DefineCylinderModel2(Colors.Green, 5);
            lca2Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[1].X, _currSusp.Hardpoints[1].Y, _currSusp.Hardpoints[1].Z,
                    _currSusp.Hardpoints[2].X, _currSusp.Hardpoints[2].Y, _currSusp.Hardpoints[2].Z,
                    _currSusp.Hardpoints[0].X, _currSusp.Hardpoints[0].Y, _currSusp.Hardpoints[0].Z,
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );


            uca1Model = VisualizationMethods.DefineCylinderModel2(Colors.Red, 5);
            uca1Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[3].X, _currSusp.Hardpoints[3].Y, _currSusp.Hardpoints[3].Z,
                    _currSusp.Hardpoints[5].X, _currSusp.Hardpoints[5].Y, _currSusp.Hardpoints[5].Z,
                    _currSusp.Hardpoints[4].X, _currSusp.Hardpoints[4].Y, _currSusp.Hardpoints[4].Z,
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );


            uca2Model = VisualizationMethods.DefineCylinderModel2(Colors.Red, 5);
            uca2Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[4].X, _currSusp.Hardpoints[4].Y, _currSusp.Hardpoints[4].Z,
                    _currSusp.Hardpoints[5].X, _currSusp.Hardpoints[5].Y, _currSusp.Hardpoints[5].Z,
                    _currSusp.Hardpoints[3].X, _currSusp.Hardpoints[3].Y, _currSusp.Hardpoints[3].Z,
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );


            trModel = VisualizationMethods.DefineCylinderModel2(Colors.Cyan, 5);
            trModel.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[6].X, _currSusp.Hardpoints[6].Y, _currSusp.Hardpoints[6].Z,
                    _currSusp.Hardpoints[7].X, _currSusp.Hardpoints[7].Y, _currSusp.Hardpoints[7].Z,
                    _currSusp.Hardpoints[0].X, _currSusp.Hardpoints[0].Y, _currSusp.Hardpoints[0].Z,
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );


            wheelModel = VisualizationMethods.DefineWheelModel(Colors.Black, _currSusp.WheelWidth, _currSusp.WheelRadius, _currSusp.WheelInsideRadius);
            wheelModel.Transform = VisualizationMethods.TransformHollowCylindricalModel(
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z,
                    _currSusp.Hardpoints[9].X, _currSusp.Hardpoints[9].Y, _currSusp.Hardpoints[9].Z,
                    _currSusp.Hardpoints[7].X, _currSusp.Hardpoints[7].Y, _currSusp.Hardpoints[7].Z,
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );

            uprightModel = VisualizationMethods.DefineUprightModel(Colors.Blue,
                new Point3D(_currSusp.Hardpoints[2].X, _currSusp.Hardpoints[2].Y, _currSusp.Hardpoints[2].Z),
                new Point3D(_currSusp.Hardpoints[5].X, _currSusp.Hardpoints[5].Y, _currSusp.Hardpoints[5].Z),
                new Point3D(_currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z),
                new Point3D(_currSusp.Hardpoints[7].X, _currSusp.Hardpoints[7].Y, _currSusp.Hardpoints[7].Z),
                new Point3D(_currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z)
                );

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

        }
        private void DefineLights(Model3DGroup group)
        {
            group.Children.Add(new AmbientLight(Colors.Gray));

            Vector3D direction = new Vector3D(1, -2, -3);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction));

            Vector3D direction2 = new Vector3D(0, -1, 0);
            group.Children.Add(new DirectionalLight(Colors.Gray, direction2));

            group.Children.Add(new PointLight(Colors.Gray, new Point3D(
                _currSusp.Hardpoints[8].X,
                _currSusp.Hardpoints[8].X - 500,
                _currSusp.Hardpoints[8].X - 500)));
        }
        public void UpdateModels()
        {
            // method called when a change in CurrentSuspension occurs

            wheelAssyGroup.Children.Clear();

            uprightModel = VisualizationMethods.DefineUprightModel(Colors.Blue,
                new Point3D(_currSusp.HardpointsMoved[0], _currSusp.HardpointsMoved[1], _currSusp.HardpointsMoved[2]),   // LCA3
                new Point3D(_currSusp.HardpointsMoved[3], _currSusp.HardpointsMoved[4], _currSusp.HardpointsMoved[5]),   // UCA3
                new Point3D(_currSusp.HardpointsMoved[9], _currSusp.HardpointsMoved[10],_currSusp.HardpointsMoved[11]), // WCN
                new Point3D(_currSusp.HardpointsMoved[6], _currSusp.HardpointsMoved[7], _currSusp.HardpointsMoved[8]),    // TR2
                new Point3D(_currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z)
                );
            wheelAssyGroup.Children.Add(uprightModel);

            wheelModel = VisualizationMethods.DefineWheelModel(Colors.Black, _currSusp.WheelWidth, _currSusp.WheelRadius, _currSusp.WheelInsideRadius);

            wheelModel.Transform = VisualizationMethods.TransformHollowCylindricalModel(
                    _currSusp.HardpointsMoved[9], _currSusp.HardpointsMoved[10], _currSusp.HardpointsMoved[11],    // WCN
                    _currSusp.HardpointsMoved[12],_currSusp.HardpointsMoved[13], _currSusp.HardpointsMoved[14],   // SPN
                    _currSusp.HardpointsMoved[6], _currSusp.HardpointsMoved[7],  _currSusp.HardpointsMoved[8],       // TR2
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z

                );

            wheelAssyGroup.Children.Add(wheelModel);

            lca1Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[0].X, _currSusp.Hardpoints[0].Y, _currSusp.Hardpoints[0].Z,   // LCA1
                    _currSusp.HardpointsMoved[0], _currSusp.HardpointsMoved[1], _currSusp.HardpointsMoved[2],   // LCA3
                    _currSusp.Hardpoints[1].X, _currSusp.Hardpoints[1].Y, _currSusp.Hardpoints[1].Z,   // LCA2
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z

                );

            lca2Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[1].X, _currSusp.Hardpoints[1].Y, _currSusp.Hardpoints[1].Z,   // LCA2
                    _currSusp.HardpointsMoved[0], _currSusp.HardpointsMoved[1], _currSusp.HardpointsMoved[2],   // LCA3
                    _currSusp.Hardpoints[0].X, _currSusp.Hardpoints[0].Y, _currSusp.Hardpoints[0].Z,    // LCA1
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );

            uca1Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[3].X, _currSusp.Hardpoints[3].Y, _currSusp.Hardpoints[3].Z,   // UCA1
                    _currSusp.HardpointsMoved[3], _currSusp.HardpointsMoved[4], _currSusp.HardpointsMoved[5],   // UCA3
                    _currSusp.Hardpoints[4].X, _currSusp.Hardpoints[4].Y, _currSusp.Hardpoints[4].Z,   // UCA2
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );

            uca2Model.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[4].X, _currSusp.Hardpoints[4].Y, _currSusp.Hardpoints[4].Z,   // UCA2
                    _currSusp.HardpointsMoved[3], _currSusp.HardpointsMoved[4], _currSusp.HardpointsMoved[5],   // UCA3
                    _currSusp.Hardpoints[3].X, _currSusp.Hardpoints[3].Y, _currSusp.Hardpoints[3].Z,   // UCA1
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z
                );

            trModel.Transform = VisualizationMethods.TransformCylindricalModel(
                    _currSusp.Hardpoints[6].X, _currSusp.Hardpoints[6].Y, _currSusp.Hardpoints[6].Z,   // TR1
                    _currSusp.HardpointsMoved[6], _currSusp.HardpointsMoved[7], _currSusp.HardpointsMoved[8],   // TR2
                    _currSusp.Hardpoints[0].X, _currSusp.Hardpoints[0].Y, _currSusp.Hardpoints[0].Z,   // LCA1
                    _currSusp.Hardpoints[8].X, _currSusp.Hardpoints[8].Y, _currSusp.Hardpoints[8].Z

                );
        }
    }
}
