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
    class Command05 : IExternalCommand
    {
        // создает рабочую поверхность по направлению вида
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            using (Transaction t = new Transaction(doc, "Creating sketchplane"))
            {
                t.Start();
                Plane plane = Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, XYZ.Zero); //doc.ActiveView.Origin);
                SketchPlane sp = SketchPlane.Create(doc, plane);
                doc.ActiveView.SketchPlane = sp;
                t.Commit();
            }

            return Result.Succeeded;

        }
    }
}
