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

        // The camera.
        private OrthographicCamera TheCamera = null;

        //// The camera controller.
        private SphericalCameraController CameraController = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Define the camera, lights, and model.
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //vm1.CalculateSuspensionMovement2(sender);

            //wheelAssyGroup.Children.Clear();

            //uprightModel = DefineUprightModel(Colors.Blue, 
            //    new Point3D(vm1.CurrentSuspension.HardpointsMoved[0], vm1.CurrentSuspension.HardpointsMoved[1], vm1.CurrentSuspension.HardpointsMoved[2]),   // LCA3
            //    new Point3D(vm1.CurrentSuspension.HardpointsMoved[3], vm1.CurrentSuspension.HardpointsMoved[4], vm1.CurrentSuspension.HardpointsMoved[5]),   // UCA3
            //    new Point3D(vm1.CurrentSuspension.HardpointsMoved[9], vm1.CurrentSuspension.HardpointsMoved[10], vm1.CurrentSuspension.HardpointsMoved[11]), // WCN
            //    new Point3D(vm1.CurrentSuspension.HardpointsMoved[6], vm1.CurrentSuspension.HardpointsMoved[7], vm1.CurrentSuspension.HardpointsMoved[8])    // TR2
            //    );
            //wheelAssyGroup.Children.Add(uprightModel);

            //wheelModel = DefineWheelModel(Colors.Black);



            //wheelModel.Transform = TransformHollowCylindricalModel(
            //        vm1.CurrentSuspension.HardpointsMoved[9], vm1.CurrentSuspension.HardpointsMoved[10], vm1.CurrentSuspension.HardpointsMoved[11],    // WCN
            //        vm1.CurrentSuspension.HardpointsMoved[12], vm1.CurrentSuspension.HardpointsMoved[13], vm1.CurrentSuspension.HardpointsMoved[14],   // SPN
            //        vm1.CurrentSuspension.HardpointsMoved[6], vm1.CurrentSuspension.HardpointsMoved[7], vm1.CurrentSuspension.HardpointsMoved[8]       // TR2
            //    );

            //wheelAssyGroup.Children.Add(wheelModel);

            //lca1Model.Transform = TransformCylindricalModel(
            //        vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal,   // LCA1
            //        vm1.CurrentSuspension.HardpointsMoved[0], vm1.CurrentSuspension.HardpointsMoved[1], vm1.CurrentSuspension.HardpointsMoved[2],   // LCA3
            //        vm1.CurrentSuspension.Hardpoints[1].XVal, vm1.CurrentSuspension.Hardpoints[1].YVal, vm1.CurrentSuspension.Hardpoints[1].ZVal    // LCA2
            //    );

            //lca2Model.Transform = TransformCylindricalModel(
            //        vm1.CurrentSuspension.Hardpoints[1].XVal, vm1.CurrentSuspension.Hardpoints[1].YVal, vm1.CurrentSuspension.Hardpoints[1].ZVal,   // LCA2
            //        vm1.CurrentSuspension.HardpointsMoved[0], vm1.CurrentSuspension.HardpointsMoved[1], vm1.CurrentSuspension.HardpointsMoved[2],   // LCA3
            //        vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal    // LCA1
            //    );

            //uca1Model.Transform = TransformCylindricalModel(
            //        vm1.CurrentSuspension.Hardpoints[3].XVal, vm1.CurrentSuspension.Hardpoints[3].YVal, vm1.CurrentSuspension.Hardpoints[3].ZVal,   // UCA1
            //        vm1.CurrentSuspension.HardpointsMoved[3], vm1.CurrentSuspension.HardpointsMoved[4], vm1.CurrentSuspension.HardpointsMoved[5],   // UCA3
            //        vm1.CurrentSuspension.Hardpoints[4].XVal, vm1.CurrentSuspension.Hardpoints[4].YVal, vm1.CurrentSuspension.Hardpoints[4].ZVal    // UCA2
            //    );

            //uca2Model.Transform = TransformCylindricalModel(
            //        vm1.CurrentSuspension.Hardpoints[4].XVal, vm1.CurrentSuspension.Hardpoints[4].YVal, vm1.CurrentSuspension.Hardpoints[4].ZVal,   // UCA2
            //        vm1.CurrentSuspension.HardpointsMoved[3], vm1.CurrentSuspension.HardpointsMoved[4], vm1.CurrentSuspension.HardpointsMoved[5],   // UCA3
            //        vm1.CurrentSuspension.Hardpoints[3].XVal, vm1.CurrentSuspension.Hardpoints[3].YVal, vm1.CurrentSuspension.Hardpoints[3].ZVal    // UCA1
            //    );

            //trModel.Transform = TransformCylindricalModel(
            //        vm1.CurrentSuspension.Hardpoints[6].XVal, vm1.CurrentSuspension.Hardpoints[6].YVal, vm1.CurrentSuspension.Hardpoints[6].ZVal,   // TR1
            //        vm1.CurrentSuspension.HardpointsMoved[6], vm1.CurrentSuspension.HardpointsMoved[7], vm1.CurrentSuspension.HardpointsMoved[8],   // TR2
            //        vm1.CurrentSuspension.Hardpoints[0].XVal, vm1.CurrentSuspension.Hardpoints[0].YVal, vm1.CurrentSuspension.Hardpoints[0].ZVal    // LCA1
            //    );

        }
    }
}
