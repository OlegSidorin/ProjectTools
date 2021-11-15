using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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

namespace ProjectTools
{
    /// <summary>
    /// Логика взаимодействия для Command06_Window01.xaml
    /// </summary>
    public partial class Command06_Window01 : Window
    {
        public ExternalCommandData externalCommandData { get; set; }
        ApplyViewAnglesEventHandler applyViewAnglesEventHandler;
        ExternalEvent externalEventApplyViewAngles;
        public Command06_Window01()
        {
            InitializeComponent();
            applyViewAnglesEventHandler = new ApplyViewAnglesEventHandler();
            externalEventApplyViewAngles = ExternalEvent.Create(applyViewAnglesEventHandler);
        }

        private void ApplyViewAngles(object sender, RoutedEventArgs e)
        {
            applyViewAnglesEventHandler._CommandData = externalCommandData;
            double.TryParse(HorizAngle.Text, out double horizAngle);
            applyViewAnglesEventHandler.angleHorizD = horizAngle;
            double.TryParse(VertAngle.Text, out double vertAngle);
            applyViewAnglesEventHandler.angleVertD = vertAngle;

            externalEventApplyViewAngles.Raise();
        }


    }

    public class ApplyViewAnglesEventHandler : IExternalEventHandler
    {
        /// <summary>
        /// The angle in the XY plane (azimuth),
        /// typically 0 to 360.
        /// </summary>
        public double angleHorizD = 30;

        /// <summary>
        /// The vertical tilt (altitude),
        /// typically -90 to 90.
        /// </summary>
        public double angleVertD = 45;

        public ExternalCommandData _CommandData { get; set; }
        public void Execute(UIApplication uiApp)
        {
            UIDocument uiDoc = _CommandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            XYZ eye = XYZ.Zero;

            XYZ forward = VectorFromHorizVertAngles(angleHorizD, angleVertD);

            XYZ up = VectorFromHorizVertAngles(angleHorizD, angleVertD + 90);

            ViewOrientation3D viewOrientation3D = new ViewOrientation3D(eye, up, forward);

            ViewFamilyType viewFamilyType3D = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .Cast<ViewFamilyType>()
                .FirstOrDefault<ViewFamilyType>(x => ViewFamily.ThreeDimensional == x.ViewFamily);

            using (Transaction tr = new Transaction(doc, "Set view"))
            {
                tr.Start();
                View3D view3d = doc.ActiveView as View3D;
                //View3D view3d = View3D.CreateIsometric(doc, viewFamilyType3D.Id);
                view3d.SetOrientation(viewOrientation3D);
                uiApp.ActiveUIDocument.RefreshActiveView();
                tr.Commit();
            }
        }

        public string GetName()
        {
            return "ApplyViewAnglesEventHandler";
        }

        /// <summary>
        /// Return a unit vector in the specified direction.
        /// </summary>
        /// <param name="angleHorizD">Angle in XY plane 
        /// in degrees</param>
        /// <param name="angleVertD">Vertical tilt between 
        /// -90 and +90 degrees</param>
        /// <returns>Unit vector in the specified 
        /// direction.</returns>
        private XYZ VectorFromHorizVertAngles(double angleHorizD, double angleVertD)
        {
            // Convert degreess to radians.

            double degToRadian = Math.PI * 2 / 360;
            double angleHorizR = angleHorizD * degToRadian;
            double angleVertR = angleVertD * degToRadian;

            // Return unit vector in 3D

            double a = Math.Cos(angleVertR);
            double b = Math.Cos(angleHorizR);
            double c = Math.Sin(angleHorizR);
            double d = Math.Sin(angleVertR);

            return new XYZ(a * b, a * c, d);
        }

        
    }
}
