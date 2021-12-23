using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class Command08 : IExternalCommand
    {
        // рисует крестик в нуле
        const double K = 304.80;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //View3D view3d = doc.ActiveView as View3D;

            XYZ startPoint = new XYZ(0, 0, 0);
            XYZ endPoint = new XYZ(100, 0, 0);
            Line geomLine = Line.CreateBound(startPoint, endPoint);

            // Create a geometry arc in Revit application
            //XYZ end0 = new XYZ(1, 0, 0);
            //XYZ end1 = new XYZ(10, 10, 10);
            //XYZ pointOnCurve = new XYZ(10, 0, 0);
            //Arc geomArc = Arc.Create(end0, end1, pointOnCurve);

            if (doc.IsFamilyDocument)
            {
                using (Transaction t = new Transaction(doc, "Creating sketchplane"))
                {
                    t.Start();

                    Plane plane1 = Plane.CreateByNormalAndOrigin(new XYZ(0, 1, 0), XYZ.Zero);
                    SketchPlane sp1 = SketchPlane.Create(doc, plane1);
                    ModelLine line1 = doc.FamilyCreate.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(GetItInMetric(500), 0, 0)), sp1) as ModelLine;
                    ModelLine line2 = doc.FamilyCreate.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 0, GetItInMetric(500))), sp1) as ModelLine;

                    Plane plane2 = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), XYZ.Zero);
                    SketchPlane sp2 = SketchPlane.Create(doc, plane2);
                    ModelLine line3 = doc.FamilyCreate.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, GetItInMetric(500), 0)), sp2) as ModelLine;

                    //Plane plane2 = Plane.CreateByNormalAndOrigin(new XYZ(0, 1, 0), XYZ.Zero);
                    //SketchPlane sp2 = SketchPlane.Create(doc, plane2);
                    //ModelLine line2 = doc.Create.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0)), sp2) as ModelLine;

                    t.Commit();
                }
            }
            else
            {
                using (Transaction t = new Transaction(doc, "Creating sketchplane"))
                {
                    t.Start();

                    Plane plane1 = Plane.CreateByNormalAndOrigin(new XYZ(0, 1, 0), XYZ.Zero);
                    SketchPlane sp1 = SketchPlane.Create(doc, plane1);
                    ModelLine line1 = doc.Create.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(GetItInMetric(500), 0, 0)), sp1) as ModelLine;
                    ModelLine line2 = doc.Create.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 0, GetItInMetric(500))), sp1) as ModelLine;

                    Plane plane2 = Plane.CreateByNormalAndOrigin(new XYZ(1, 0, 0), XYZ.Zero);
                    SketchPlane sp2 = SketchPlane.Create(doc, plane2);
                    ModelLine line3 = doc.Create.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, GetItInMetric(500), 0)), sp2) as ModelLine;

                    //Plane plane2 = Plane.CreateByNormalAndOrigin(new XYZ(0, 1, 0), XYZ.Zero);
                    //SketchPlane sp2 = SketchPlane.Create(doc, plane2);
                    //ModelLine line2 = doc.Create.NewModelCurve(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(100, 0, 0)), sp2) as ModelLine;

                    t.Commit();
                }
            }
            

            return Result.Succeeded;
        }
        private double GetItInMetric(double dim)
        {
            return dim / K;
        }

    }
}