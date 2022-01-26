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
using System.Diagnostics;
using Microsoft.VisualBasic;
using MessageBox = System.Windows.Forms.MessageBox;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using Autodesk.Revit.DB.Mechanical;

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class Command13 : IExternalCommand
    {
        Document doc { get; set; }
        double wallIndent { get; set; }
        double wallGap { get; set; }
        public Result Execute(ExternalCommandData cmdData, ref string message, ElementSet elements)
        {

            Application app = cmdData.Application.Application;
            UIDocument uidoc = cmdData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            Command13View view = new Command13View();
            Command13ViewModel vm = (Command13ViewModel)view.DataContext;
            view.CommandData = cmdData;
            view.Show();

            return Result.Succeeded;
        }

    }

    
}
