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
    class Command01a : IExternalCommand
    {
        // Делает активным 3Д вид Navisworks
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            if (doc.IsFamilyDocument) return Result.Succeeded;
            try
            {
                var view3D = new FilteredElementCollector(doc).OfClass(typeof(View3D))?.Cast<View3D>().Where(x => x.Name.Contains("Navis"))?.ToList().First();
                if (view3D != null)
                {
                    commandData.Application.ActiveUIDocument.ActiveView = view3D;
                }
            }
            catch { };

            return Result.Succeeded;

        }
        public static string CropFileName(string fileName)
        {
            string cropFileName = fileName;
            if (fileName.Contains("_отсоединено"))
                cropFileName = cropFileName.Substring(0, cropFileName.Length - 12);
            if (fileName.ToLower().Contains("_o.sidorin"))
                cropFileName = cropFileName.Substring(0, cropFileName.Length - 10);

            return cropFileName;
        }
    }
}
