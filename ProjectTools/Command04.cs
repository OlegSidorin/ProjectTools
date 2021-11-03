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
    class Command04 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            #region Add Parameter M1_MEP System
            CategorySet catSet = commandData.Application.Application.Create.NewCategorySet();
            catSet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves));
            catSet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctAccessory));
            catSet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting));
            catSet.Insert(doc.Settings.Categories.get_Item(BuiltInCategory.OST_MechanicalEquipment));

            try
            {
                ParameterViewModel pvm = new ParameterViewModel()
                {
                    Name = "М1_MEP система"
                };
                pvm.AddSharedParameterIntoProject(commandData, doc, catSet, BuiltInParameterGroup.PG_MECHANICAL);
            }
            catch { };
            #endregion

            List<Element> allElements = new List<Element>();
            var ductCurves = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).ToList();
            ductCurves.ForEach(x => allElements.Add(x));
            var ductAccessory = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctAccessory).ToList();
            ductAccessory.ForEach(x => allElements.Add(x));
            var ductFittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctFitting).ToList();
            ductFittings.ForEach(x => allElements.Add(x));
            var mechanicalEquipment = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MechanicalEquipment).ToList();
            mechanicalEquipment.ForEach(x => allElements.Add(x));


            foreach (var el in allElements)
            {
                if (el != null)
                {
                    Parameter p = el.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
                    if (p != null)
                    {
                        try
                        {
                            using (Transaction tr = new Transaction(doc, "Set parameter M1"))
                            {
                                tr.Start();
                                el.LookupParameter("М1_MEP система").Set(p.AsString());
                                tr.Commit();
                            }

                        }
                        catch { };
                    }
                }

            }

            var messList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_DuctCurves)
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
                CreateViewFilter(doc, view, view.Name);
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
                    activeView3D.LookupParameter("ADSK_Назначение вида").Set("Схема вентиляции");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

                activeView3D.Discipline = ViewDiscipline.Mechanical;
                activeView3D.DetailLevel = ViewDetailLevel.Medium;
                activeView3D.DisplayStyle = DisplayStyle.HLR;

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
                            p.Set("Схемы вентиляции");

                if (p3List != null)
                    foreach (var p in p2List)
                        if (!p.IsReadOnly)
                            p.Set("Схемы вентиляции");

                activeView3D.AreImportCategoriesHidden = true;
                activeView3D.AreAnalyticalModelCategoriesHidden = true;
                //activeView3D.AreAnnotationCategoriesHidden = true;

                foreach (var elId in hideList)
                {
                    activeView3D.HideCategoryTemporary(elId);
                }

                // переводит временно скрытые категории в постоянные
                activeView3D.ConvertTemporaryHideIsolateToPermanent();

                FilteredElementCollector pipeCurves = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves);
                if (pipeCurves.Count() != 0)
                    activeView3D.SetCategoryHidden(pipeCurves.FirstElement().Category.Id, false);
                FilteredElementCollector pipeAcc = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctAccessory);
                if (pipeAcc.Count() != 0)
                    activeView3D.SetCategoryHidden(pipeAcc.FirstElement().Category.Id, false);
                FilteredElementCollector pipeFit = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctFitting);
                if (pipeFit.Count() != 0)
                    activeView3D.SetCategoryHidden(pipeFit.FirstElement().Category.Id, false);
                FilteredElementCollector mechEq = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
                if (mechEq.Count() != 0)
                    activeView3D.SetCategoryHidden(mechEq.FirstElement().Category.Id, false);

                setView.Commit();
            }
        }

        private static ElementFilter CreateElementFilterFromFilterRules(IList<FilterRule> filterRules)
        {
            // We use a LogicalAndFilter containing one ElementParameterFilter
            // for each FilterRule. We could alternatively create a single
            // ElementParameterFilter containing the entire list of FilterRules.
            IList<ElementFilter> elemFilters = new List<ElementFilter>();
            foreach (FilterRule filterRule in filterRules)
            {
                ElementParameterFilter elemParamFilter = new ElementParameterFilter(filterRule);
                elemFilters.Add(elemParamFilter);
            }
            LogicalAndFilter elemFilter = new LogicalAndFilter(elemFilters);

            return elemFilter;
        }

        public static void CreateViewFilter(Document doc, View view, string value)
        {
            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_DuctCurves));
            categories.Add(new ElementId(BuiltInCategory.OST_DuctAccessory));
            categories.Add(new ElementId(BuiltInCategory.OST_DuctFitting));
            categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
            List<FilterRule> filterRules = new List<FilterRule>();

            using (Transaction t = new Transaction(doc, "Add view filter"))
            {
                t.Start();

                // Create filter element assocated to the input categories
                ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Фильтр для Системы " + value, categories);

                /*
                // Criterion 1 - wall type Function is "Exterior"
                ElementId exteriorParamId = new ElementId(BuiltInParameter.FUNCTION_PARAM);
                filterRules.Add(ParameterFilterRuleFactory.CreateEqualsRule(exteriorParamId, (int)WallFunction.Exterior));

                // Criterion 2 - wall height > some number
                ElementId lengthId = new ElementId(BuiltInParameter.CURVE_ELEM_LENGTH);
                filterRules.Add(ParameterFilterRuleFactory.CreateGreaterOrEqualRule(lengthId, 28.0, 0.0001));
                */
                // Criterion 3 - custom shared parameter value matches string pattern
                // Get the id for the shared parameter - the ElementId is not hardcoded, so we need to get an instance of this type to find it
                Guid spGuid = new Guid("ff5da4f2-129e-4ae6-ad78-8ac062fe377a"); // М1_MEP система


                var ducts = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).ToList();
                //var pipeAccessories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory).ToList();
                //var pipeFittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting).ToList();
                //var plumbingFixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlumbingFixtures).ToList();

                Element el = ducts.First();

                if (el != null)
                {
                    Parameter sharedParam = el.get_Parameter(spGuid);
                    ElementId sharedParamId = sharedParam.Id;

                    filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(sharedParamId, value, true));
                }

                ElementFilter elemFilter = CreateElementFilterFromFilterRules(filterRules);
                parameterFilterElement.SetElementFilter(elemFilter);

                // Apply filter to view
                view.AddFilter(parameterFilterElement.Id);
                view.SetFilterVisibility(parameterFilterElement.Id, false);
                t.Commit();
            }
        }

        public bool IsDiffersByOneLetter(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;
            int difference = 0;
            if (len1 != len2)
            {
                return false;
            }
            else
            {
                if (str1.Contains("T11") && str2.Contains("T21") || str1.Contains("Т11") && str2.Contains("Т21"))
                    return true;
                if (str1.Contains("T12") && str2.Contains("T22") || str1.Contains("Т12") && str2.Contains("Т22"))
                    return true;
            }
            return false;
        }
    }
}
