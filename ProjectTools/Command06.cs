using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Application = Autodesk.Revit.ApplicationServices.Application;
using System.Linq;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class Command06 : IExternalCommand
    {
        // поворачивает вид по назначенным углам
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                View3D view3d = doc.ActiveView as View3D;
                var viewOrientation = view3d.GetOrientation();

                XYZ fd = viewOrientation.ForwardDirection;
                XYZ up = viewOrientation.UpDirection;

                Command06_Window01 window = new Command06_Window01()
                {
                    externalCommandData = commandData
                };

                window.HorizAngle.Text = GetAngles(fd).Item1.ToString();
                window.VertAngle.Text = GetAngles(fd).Item2.ToString();
                window.Show();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            };

            return Result.Succeeded;

        }
        private (double, double) GetAngles(XYZ xyz)
        {
            double degToRadian = Math.PI * 2 / 360;

            return (Math.Acos(xyz.X  / Math.Cos(Math.Asin(xyz.Z))) / degToRadian, Math.Asin(xyz.Z) / degToRadian);
        }
    }
}
