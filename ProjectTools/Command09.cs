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
using System.Windows.Documents;
using System.Windows.Media;

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class Command09 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            string sp_M1_Name_Name = "М1_Имя помещения в БД";
            string sp_M1_Number_Name = "М1_Номер помещения в БД";

            SharedParameterElement sp_M1_Name = new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement)).Cast<SharedParameterElement>().Where(x => x.GetDefinition().Name == sp_M1_Name_Name).FirstOrDefault();
            SharedParameterElement sp_M1_Number = new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement)).Cast<SharedParameterElement>().Where(x => x.GetDefinition().Name == sp_M1_Number_Name).FirstOrDefault();

            var rooms = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).ToList();

            FlowDocumentForReport flowDocumentForReport = new FlowDocumentForReport();
            flowDocumentForReport.FlowDocument = new System.Windows.Documents.FlowDocument();
            //flowDocumentForReport.AddHead("Были обнаружены помещения, в которых изменился номер");

            List<(string, string)> roomNumbers = new List<(string, string)>();
            foreach (var element in rooms)
            {
                try
                {
                    SpatialElement room = element as SpatialElement;
                    string roomName = room.LookupParameter("Имя").AsString();
                    string roomM1Name = room.LookupParameter(sp_M1_Name_Name).AsString();
                    string roomNumber = room.LookupParameter("Номер").AsString();
                    string roomM1Number = room.LookupParameter(sp_M1_Number_Name).AsString();
                    if (roomNumber != roomM1Number)
                    {
                        roomNumbers.Add(($"{roomM1Number} ({roomM1Name})", $"{roomNumber} ({roomName})"));
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); };
            }

            //////////////СОЗДАНИЕ ТАБЛИЦЫ/////////////////////
            Table table = new Table();
            int numberOfColumns = 2;

            //for (int i = 0; i < numberOfColumns; i++)
            //{
            //    table.Columns.Add(new TableColumn());
            //    if (i % 2 == 0)
            //        table.Columns[i].Background = Brushes.Beige;
            //    else
            //        table.Columns[i].Background = Brushes.LightSteelBlue;
            //}

            table.RowGroups.Add(new TableRowGroup());

            table.RowGroups[0].Rows.Add(new TableRow());
            TableRow currentRow = table.RowGroups[0].Rows[0];
            //currentRow.Background = Brushes.White;
            currentRow.FontSize = 18;
            currentRow.FontWeight = System.Windows.FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Помещения, в которых изменился номер"))));
            currentRow.Cells[0].ColumnSpan = numberOfColumns;

            table.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table.RowGroups[0].Rows[1];
            currentRow.FontSize = 16;
            currentRow.FontWeight = FontWeights.Bold;
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Было"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Стало"))));

            int rowNumber = 2;
            foreach (var r in roomNumbers)
            {
                table.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table.RowGroups[0].Rows[rowNumber];
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(r.Item1))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(r.Item2))));
                rowNumber += 1;
            }

            flowDocumentForReport.FlowDocument.Blocks.Add(table);
            ////////////////////////////////////////////////////////
            
            ReportWindow reportWindow = new ReportWindow();
            reportWindow.flowDocScrollViewer.Document = flowDocumentForReport.FlowDocument;
            reportWindow.Show();

            return Result.Succeeded;
        }

    }
}