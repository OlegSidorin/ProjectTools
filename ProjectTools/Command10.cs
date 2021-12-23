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
    class Command10 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            string sp_M1_Name_Name = "М1_Имя помещения в БД";
            string sp_M1_Number_Name = "М1_Номер помещения в БД";
            FOP.AddSharedParameters_M1_ForRooms(commandData);

            SharedParameterElement sp_M1_Name = new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement)).Cast<SharedParameterElement>().Where(x => x.GetDefinition().Name == sp_M1_Name_Name).FirstOrDefault();
            SharedParameterElement sp_M1_Number = new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement)).Cast<SharedParameterElement>().Where(x => x.GetDefinition().Name == sp_M1_Number_Name).FirstOrDefault();

            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToList();
            using (Transaction t = new Transaction(doc, " Add parameters and set "))
            {
                t.Start();
                foreach (var element in rooms)
                {
                    try
                    {
                        SpatialElement room = element as SpatialElement;
                        string pName = room.LookupParameter("Имя").AsString();
                        room.LookupParameter(sp_M1_Name_Name).Set(pName);
                        string pNumber = room.LookupParameter("Номер").AsString();
                        room.LookupParameter(sp_M1_Number_Name).Set(pNumber);
                    }
                    catch (Exception ex) { /*MessageBox.Show(ex.ToString());*/ };
                }
                t.Commit(); 
            }
            
            return Result.Succeeded;
        }

    }
}