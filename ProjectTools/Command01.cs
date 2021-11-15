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
    class Command01 : IExternalCommand
    {
        // Создает 3Д вид и называет его Navisworks
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //commandData.Application.Application.SharedParametersFilename = FOP.PathToFOP;
            

            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            if (doc.IsFamilyDocument) return Result.Succeeded;
            var activeView = doc.ActiveView;
            if (activeView.ViewType == ViewType.ThreeD)
            {
                MessageBox.Show("Просьба покинуть 3D вид");
                return Result.Succeeded;
            }

            var infoText = "";
            string fileName = doc.Title;
            string cropFileName = CropFileName(fileName);

            View activeView3D = CreateView(doc);

            infoText = activeView3D.Name;
            using (Transaction setView = new Transaction(doc, "Some"))
            {
                setView.Start();

                //activeView3D.Name = "Export to Navisworks & BIM360";
                activeView3D.Discipline = ViewDiscipline.Coordination;
                activeView3D.DetailLevel = ViewDetailLevel.Fine;
                activeView3D.DisplayStyle = DisplayStyle.ShadingWithEdges;

                // установка уровней группировки вида
                IList<Parameter> p1List = activeView3D.GetParameters(name: "М1_Группа вида");
                IList<Parameter> p2List = activeView3D.GetParameters(name: "М1_Подгруппа вида");

                if (p1List != null)
                    foreach (var p in p1List)
                        if (!p.IsReadOnly)
                            p.Set("▲ КООРДИНАЦИЯ");

                if (p2List != null)
                    foreach (var p in p2List)
                        if (!p.IsReadOnly)
                            p.Set("BIM");

                activeView3D.AreImportCategoriesHidden = true;
                activeView3D.AreAnalyticalModelCategoriesHidden = true;
                activeView3D.AreAnnotationCategoriesHidden = true;

                // поиск и отключение ссылок RVT файлов в виде
                //FilteredElementCollector rvtLinkCollect = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks);
                //ICollection<ElementId> rvtLinkIdList = new List<ElementId>();

                //foreach (var link in rvtLinkCollect)
                //    rvtLinkIdList.Add(link.Id);

                //if (rvtLinkIdList != null)
                //    activeView3D.HideElementsTemporary(rvtLinkIdList);

                // отключение осевых линий воздуховодов
                FilteredElementCollector centerDLines = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurvesCenterLine);

                if (centerDLines.Count() != 0)
                    activeView3D.HideCategoryTemporary(centerDLines.FirstElement().Category.Id);

                // отключение осевых линий труб
                FilteredElementCollector centerLines = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurvesCenterLine);
                if (centerLines.Count() != 0)
                    activeView3D.HideCategoryTemporary(centerLines.FirstElement().Category.Id);

                // отключение линий
                FilteredElementCollector lines = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Lines);
                if (lines.Count() != 0)
                    activeView3D.HideCategoryTemporary(lines.FirstElement().Category.Id);

                // переводит временно скрытые категории в постоянные
                activeView3D.ConvertTemporaryHideIsolateToPermanent();

                setView.Commit();
            }

            // Подготовка к публикации
            // Удаляет предыдущий набор, содержащий BIM360 (что бы не было ошибки потом при создании)
            using (Transaction removeSheetSet = new Transaction(doc, "Remove SheetSet BIM360"))
            {
                removeSheetSet.Start();
                FilteredElementCollector coll = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet));
                List<ViewSheetSet> lSets = coll.Cast<ViewSheetSet>().ToList();
                ViewSheetSet vSSet = lSets.Where(x => x.Name == "BIM360").FirstOrDefault();
                if (vSSet != null)
                {
                    ElementId eid = vSSet.Id;
                    doc.Delete(eid);
                }

                removeSheetSet.Commit();
            }

            // создает набор BIM360 для публикации в облако
            using (Transaction publication = new Transaction(doc, "Publication in Cloud"))
            {
                publication.Start();
                string sheetSetsInfo = "Наборы в проекте: \n    ";

                ViewSet myViewSet = new ViewSet();
                foreach (View3D v3D in new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>()
                         .Where(q => q.Name.Contains("Navis")))
                {
                    myViewSet.Insert(v3D);
                }
                foreach (ViewSheet viewSheet in new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).Cast<ViewSheet>()
                    .Where(q => q.CanBePrinted == true))
                {
                    myViewSet.Insert(viewSheet);
                }
                PrintManager printManager = doc.PrintManager;
                printManager.PrintRange = PrintRange.Select;
                ViewSheetSetting viewSheetSetting = printManager.ViewSheetSetting;
                viewSheetSetting.CurrentViewSheetSet.Views = myViewSet;
                Element vss = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet))
                    .Where(x => x.Name.Contains("BIM360" + "-" + cropFileName)).ToList().FirstOrDefault();
                if (vss == null)
                {
                    try
                    {
                        viewSheetSetting.SaveAs("BIM360" + "-" + cropFileName);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                FilteredElementCollector coll = new FilteredElementCollector(doc).OfClass(typeof(ViewSheetSet));
                List<ViewSheetSet> lSets = coll.Cast<ViewSheetSet>().ToList();
                foreach (ViewSheetSet v in lSets)
                {
                    sheetSetsInfo += v.Name + " \n    ";
                }
                // TaskDialog.Show("infa", str5);
                publication.Commit();
            }

            // скрытие обобщенных элементов, которые содержат имя КПСП_Отверстие
            //using (Transaction tr = new Transaction(doc, "Hide generic elements"))
            //{
            //    tr.Start();
            //    ElementId elId;
            //    FilteredElementCollector collector = new FilteredElementCollector(doc)
            //        .OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel);
            //    ICollection<ElementId> elementIdList = new List<ElementId>();
            //    foreach (Element elem in collector)
            //    {
            //        FamilyInstance inst = elem as FamilyInstance;
            //        if (elem.Name.Contains("КПСП_Отверстие"))
            //        {
            //            elId = inst.Id as ElementId;
            //            elementIdList.Add(elId);
            //        }
            //    }

            //    if (elementIdList != null)
            //        activeView3D.HideElementsTemporary(elementIdList);

            //    activeView3D.ConvertTemporaryHideIsolateToPermanent();
            //    tr.Commit();
            //}

            // TaskDialog.Show("Hello", "ЗАДАНИЕ ВЫПОЛНЕНО" + Environment.NewLine + str);


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
        private View3D CreateView(Document doc)
        {
            View3D view;
            string deletedViewsListText = "Список удаленных видов: ";
            using (Transaction deleteViews = new Transaction(doc, "Delete Views"))
            {
                deleteViews.Start();
                FilteredElementCollector view3DCollector = new FilteredElementCollector(doc).OfClass(typeof(View3D));
                ICollection<ElementId> deletedViewsList = new List<ElementId>();

                foreach (Element view3DElement in view3DCollector)
                {
                    View view3D = view3DElement as View3D;
                    if (view3D.Name.Contains("{3D}") || view3D.Name.Contains("Navis") || view3D.Name.Contains("BIM360"))
                    {
                        deletedViewsList.Add(view3D.Id);
                    }
                    deletedViewsListText += view3D.Name + ", ";
                };

                try
                {
                    if (deletedViewsList != null)
                    {
                        doc.Delete(deletedViewsList);
                    }
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException ex)
                {
                    //TaskDialog.Show("in doc:", "err" + e.Message);
                    MessageBox.Show(ex.ToString());
                }
                deleteViews.Commit();
            }

            // Отключение внешних ссылок
            //try
            //{
            //    FilteredElementCollector linkCollector = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance));
            //    foreach (Element elem in linkCollector)
            //    {
            //        RevitLinkInstance instance = elem as RevitLinkInstance;
            //        Document linkDoc = instance.GetLinkDocument();
            //        RevitLinkType type = doc.GetElement(instance.GetTypeId()) as RevitLinkType;
            //        if (!type.IsNestedLink)
            //        {
            //            type.Unload(null);
            //        }
            //    }

            //}
            //catch (Exception e)
            //{
            //    TaskDialog.Show("in doc:", "err" + e.Message);
            //}

            var eyePos = new XYZ (0.707106781186548, 0.707106781186548, 0); //(-1, -1, 0); 
            var upDir = new XYZ(-0.408248290463863, 0.408248290463863, 0.816496580927726); //(0, 0, 1);  
            var forwardDir = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);  //(1, 1, 0);  

            var collector = new FilteredElementCollector(doc);
            var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

            using (Transaction t = new Transaction(doc, "set DefaultTemplate to null"))
            {
                t.Start();
                viewFamilyType.DefaultTemplateId = ElementId.InvalidElementId;
                t.Commit();
            }

            using (Transaction ttNew = new Transaction(doc, "new 3D View"))
            {
                ttNew.Start();
                view = View3D.CreateIsometric(doc, viewFamilyType.Id);
                view.SetOrientation(new ViewOrientation3D(eyePos, upDir, forwardDir));
                view.Name = "Navisworks";
                ttNew.Commit();
            }
            return view;
        }
    }
}
