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
    class Command03 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            var messList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .Cast<MEPCurve>()
                .ToList();

            var mepSystemNames = GetDistinctStringList(messList);

            var viewList = new List<View3D>();
            foreach (var item in mepSystemNames)
            {
                View3D view = CreateView(item, doc);
                if (view != null)
                    viewList.Add(view);
            }

            var elementIdsToHide = GetDistinctElementIdList(GetAllElementIds(doc));

            foreach (var view in viewList)
            {
                SetView(view, doc, elementIdsToHide);
            }

            return Result.Succeeded;
        }

        private List<string> GetDistinctStringList(List<string> messList)
        {
            var outputList = new List<string>();
            bool isStringInAList(string stringExample, List<string> list)
            {
                foreach (var i in list)
                {
                    if (i == stringExample)
                        return true;
                }
                return false;
            }
            foreach (var element in messList)
            {
                if (!isStringInAList(element, outputList))
                {
                    outputList.Add(element);
                }
            }
            return outputList;
        }
        private List<string> GetDistinctStringList(List<MEPCurve> messList)
        {
            var outputList = new List<string>();
            bool isStringInAList(string stringExample, List<string> list)
            {
                foreach (var i in list)
                {
                    if (i == stringExample)
                        return true;
                }
                return false;
            }
            foreach (var element in messList)
            {
                if (!isStringInAList(element.MEPSystem.Name, outputList))
                {
                    outputList.Add(element.MEPSystem.Name);
                }
            }
            return outputList;
        }
        private List<ElementId> GetDistinctElementIdList(List<ElementId> messList)
        {
            var outputList = new List<ElementId>();
            bool isStringInAList(ElementId elementId, List<ElementId> list)
            {
                foreach (var i in list)
                {
                    if (i.IntegerValue == elementId.IntegerValue)
                        return true;
                }
                return false;
            }
            foreach (var element in messList)
            {
                if (!isStringInAList(element, outputList))
                {
                    outputList.Add(element);
                }
            }
            return outputList;
        }
        private List<Element> GetAllElements(Document doc)
        {
            return new FilteredElementCollector(doc)
                    .WherePasses(new LogicalOrFilter(new ElementIsElementTypeFilter(false), new ElementIsElementTypeFilter(true))).ToList();
        }
        private List<ElementId> GetAllElementIds(Document doc)
        {
            var allElements = GetAllElements(doc);
            List<ElementId> allElementIds = new List<ElementId>();
            foreach (var el in allElements)
            {
                if (el != null)
                {
                    ElementId elId = el?.Category?.Id;
                    if (elId != null)
                        allElementIds.Add(elId);
                }

            }
            return allElementIds;
        }
        private View3D CreateView(string viewName, Document doc)
        {
            View3D view = null;

            var eyePos = new XYZ(0.707106781186548, 0.707106781186548, 0); //(-1, -1, 0); 
            var upDir = new XYZ(-0.408248290463863, 0.408248290463863, 0.816496580927726); //(0, 0, 1);  
            var forwardDir = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);  //(1, 1, 0);  

            var collector = new FilteredElementCollector(doc);
            var viewFamilyType = collector.OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>()
                .FirstOrDefault(x => x.ViewFamily == ViewFamily.ThreeDimensional);

            try
            {
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
                    view.Name = $"{viewName}";
                    ttNew.Commit();
                }

            }
            catch { };
            
            return view;
        }
        private void SetView(View3D activeView3D, Document doc, List<ElementId> hideList)
        {
            using (Transaction setView = new Transaction(doc, "Set view"))
            {
                setView.Start();

                try
                {
                    activeView3D.LookupParameter("ADSK_Назначение вида").Set("Схема труб");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                activeView3D.Discipline = ViewDiscipline.Mechanical;
                activeView3D.DetailLevel = ViewDetailLevel.Coarse;
                activeView3D.DisplayStyle = DisplayStyle.Wireframe;

                IList<Parameter> p1List = activeView3D.GetParameters(name: "М1_Группа вида");
                IList<Parameter> p2List = activeView3D.GetParameters(name: "М1_Подгруппа вида");
                IList<Parameter> p3List = activeView3D.GetParameters(name: "ADSK_Назначение вида");

                if (p1List != null)
                    foreach (var p in p1List)
                        if (!p.IsReadOnly)
                            p.Set("▲ Схемы");

                if (p2List != null)
                    foreach (var p in p2List)
                        if (!p.IsReadOnly)
                            p.Set("Схемы труб");

                if (p3List != null)
                    foreach (var p in p2List)
                        if (!p.IsReadOnly)
                            p.Set("Схемы труб");

                activeView3D.AreImportCategoriesHidden = true;
                activeView3D.AreAnalyticalModelCategoriesHidden = true;
                //activeView3D.AreAnnotationCategoriesHidden = true;

                foreach (var elId in hideList)
                {
                    activeView3D.HideCategoryTemporary(elId);
                }

                // переводит временно скрытые категории в постоянные
                activeView3D.ConvertTemporaryHideIsolateToPermanent();

                FilteredElementCollector pipeCurves = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves);
                if (pipeCurves.Count() != 0)
                    activeView3D.SetCategoryHidden(pipeCurves.FirstElement().Category.Id, false);
                FilteredElementCollector pipeAcc = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory);
                if (pipeAcc.Count() != 0)
                    activeView3D.SetCategoryHidden(pipeAcc.FirstElement().Category.Id, false);
                FilteredElementCollector pipeFit = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting);
                if (pipeFit.Count() != 0)
                    activeView3D.SetCategoryHidden(pipeFit.FirstElement().Category.Id, false);
                FilteredElementCollector plumFix = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlumbingFixtures);
                if (plumFix.Count() != 0)
                    activeView3D.SetCategoryHidden(plumFix.FirstElement().Category.Id, false);

                setView.Commit();
            }
        }
        public static void CreateViewFilter(Document doc, View view)
        {
            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_Walls));
            List<FilterRule> filterRules = new List<FilterRule>();

            using (Transaction t = new Transaction(doc, "Add view filter"))
            {
                t.Start();

                // Create filter element assocated to the input categories
                ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Example view filter", categories);

                // Criterion 1 - wall type Function is "Exterior"
                ElementId exteriorParamId = new ElementId(BuiltInParameter.FUNCTION_PARAM);
                filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(exteriorParamId, (int)WallFunction.Exterior));

                // Criterion 2 - wall height > some number
                ElementId lengthId = new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH);
                filterRules.Add(ParameterFilterRuleFactory.CreateGreaterOrEqualRule(lengthId, 28.0, 0.0001));

                // Criterion 3 - custom shared parameter value matches string pattern
                // Get the id for the shared parameter - the ElementId is not hardcoded, so we need to get an instance of this type to find it
                Guid spGuid = new Guid("96b00b61-7f5a-4f36-a828-5cd07890a02a");
                FilteredElementCollector collector = new FilteredElementCollector(doc);
                collector.OfClass(typeof(Wall));
                Wall wall = collector.FirstElement() as Wall;

                if (wall != null)
                {
                    Parameter sharedParam = wall.get_Parameter(spGuid);
                    ElementId sharedParamId = sharedParam.Id;

                    filterRules.Add(ParameterFilterRuleFactory.CreateBeginsWithRule(sharedParamId, "15.", true));
                }

                parameterFilterElement.SetRules(filterRules);

                // Apply filter to view
                view.AddFilter(parameterFilterElement.Id);
                view.SetFilterVisibility(parameterFilterElement.Id, false);
                t.Commit();
            }
        }
    }
}
