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
        // Готовит виды схемы для вентиляции по количеству систем в проекте
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            FOP.AddSharedParameter_M1_MEP_System(commandData);

            List<Element> allElements = new List<Element>();
            var ductCurves = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).ToList();
            ductCurves.ForEach(x => allElements.Add(x));
            var ductAccessory = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctAccessory).ToList();
            ductAccessory.ForEach(x => allElements.Add(x));
            var ductFittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctFitting).ToList();
            ductFittings.ForEach(x => allElements.Add(x));
            var mechanicalEquipment = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MechanicalEquipment).ToList();
            mechanicalEquipment.ForEach(x => allElements.Add(x));

            List<string> messStrings = new List<string>();
            foreach (var el in allElements)
            {
                if (el != null)
                {
                    Parameter p = el.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM);

                    if (p != null)
                    {
                        string pValue = "";
                        pValue = p.AsString();
                        if (!string.IsNullOrEmpty(pValue)) messStrings.Add(pValue);
                        if (!string.IsNullOrEmpty(pValue))
                        {
                            if (pValue.Contains(","))
                            {
                                string[] ss = pValue.Split(',');
                                string newS = "";
                                foreach (string item in ss)
                                {
                                    int sIndex = 0;
                                    sIndex = item.IndexOf(" ");
                                    if (sIndex != 0)
                                    {
                                        newS += item.Substring(0, sIndex) + ",";
                                    }
                                }
                                pValue = newS.Substring(0, newS.Length - 1);
                            }
                            else
                            {
                                int spaceIndex = 0;
                                if (pValue.Contains(" ")) spaceIndex = pValue.IndexOf(" "); // string[] words = s.Split(new char[] { ' ' });
                                if (spaceIndex != 0 )
                                {
                                    pValue = pValue.Substring(0, spaceIndex);
                                }
                            }

                        }
                        try
                        {
                            using (Transaction tr = new Transaction(doc, "Set parameter M1"))
                            {
                                tr.Start();
                                el.LookupParameter("М1_MEP система").Set(pValue);
                                tr.Commit();
                            }

                        }
                        catch { };
                    }
                }

            }

            var mepSystemNames = GetDistinctStringList(messStrings);

            var mepSystemNames_FirstPart = GetFirstPartStringList(mepSystemNames);

            mepSystemNames = GetDistinctStringList(mepSystemNames_FirstPart);

            var viewList = new List<View3D>();
            foreach (var item in mepSystemNames)
            {
                View3D view = CreateView("Схема вентиляции " + item, doc, doc.ActiveView);
                if (view != null)
                    viewList.Add(view);
            }

            var elementIdsToHide = GetDistinctElementIdList(GetAllElementIds(doc));

            foreach (var view in viewList)
            {
                try
                {
                    SetView(view, doc, elementIdsToHide);
                    CreateViewFilter(doc, view, view.Name.Replace("Схема вентиляции ", ""));
                }
                catch { };
            }

            return Result.Succeeded;
        }
        private List<string> GetFirstPartStringList(List<string> inputList)
        {
            List<string> outputList = new List<string>();
            foreach (string s in inputList)
            {
                if (s != null)
                {
                    if (s.Contains(","))
                    {
                        string[] ss = s.Split(',');
                        string newS = "";
                        foreach (string item in ss)
                        {
                            int sIndex = 0;
                            sIndex = item.IndexOf(" ");
                            if (sIndex != 0)
                            {
                                newS += item.Substring(0, sIndex) + ",";
                            }
                        }
                        outputList.Add(newS.Substring(0, newS.Length - 1));
                    }
                    else
                    {
                        int spaceIndex = 0;
                        if (s.Contains(" ")) spaceIndex = s.IndexOf(" ");
                        if (spaceIndex == 0)
                        {
                            outputList.Add(s);
                        }
                        else
                        {
                            outputList.Add(s.Substring(0, spaceIndex));
                        }
                    }
                }
            }

            return outputList;
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
        private View3D CreateView(string viewName, Document doc, View activeView)
        {
            View3D view = null;

            var eyePos = new XYZ(0.707106781186548, 0.707106781186548, 0); //(-1, -1, 0); 
            var upDir = new XYZ(-0.408248290463863, 0.408248290463863, 0.816496580927726); //(0, 0, 1);  
            var forwardDir = new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626);  //(1, 1, 0);  

            try
            {
                var activeView3D = (View3D)activeView;
                eyePos = activeView3D.GetOrientation().EyePosition;
                upDir = activeView3D.GetOrientation().UpDirection;
                forwardDir = activeView3D.GetOrientation().ForwardDirection;
            }
            catch { };

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
        private void SetView(View3D view3D, Document doc, List<ElementId> hideList)
        {
            if (view3D != null)
            {
                using (Transaction setView = new Transaction(doc, "Set view"))
                {
                    setView.Start();

                    try
                    {
                        view3D.LookupParameter("ADSK_Назначение вида").Set("Схемы вентиляции");
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                    }

                    view3D.Discipline = ViewDiscipline.Mechanical;
                    view3D.DisplayStyle = DisplayStyle.HLR;
                    view3D.DetailLevel = ViewDetailLevel.Medium;

                    try
                    {
                        view3D.DisplayStyle = doc.ActiveView.DisplayStyle;
                        view3D.DetailLevel = doc.ActiveView.DetailLevel;
                    }
                    catch { };

                    view3D.SaveOrientationAndLock();

                    IList<Parameter> p1List = view3D.GetParameters(name: "М1_Группа вида");
                    IList<Parameter> p2List = view3D.GetParameters(name: "М1_Подгруппа вида");
                    IList<Parameter> p3List = view3D.GetParameters(name: "ADSK_Назначение вида");

                    if (p1List != null || p1List.Count != 0)
                        foreach (var p in p1List)
                            if (!p.IsReadOnly)
                                p.Set("▲ Схемы");

                    if (p2List != null || p2List.Count != 0)
                        foreach (var p in p2List)
                            if (!p.IsReadOnly)
                                p.Set("Схемы вентиляции");

                    if (p3List != null || p3List.Count != 0)
                        foreach (var p in p2List)
                            if (!p.IsReadOnly)
                                p.Set("Схемы вентиляции");

                    view3D.AreImportCategoriesHidden = true;
                    view3D.AreAnalyticalModelCategoriesHidden = true;
                    //activeView3D.AreAnnotationCategoriesHidden = true;

                    foreach (var elId in hideList)
                    {
                        view3D.HideCategoryTemporary(elId);
                    }

                    // переводит временно скрытые категории в постоянные
                    view3D.ConvertTemporaryHideIsolateToPermanent();

                    FilteredElementCollector pipeCurves = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves);
                    if (pipeCurves.Count() != 0)
                        view3D.SetCategoryHidden(pipeCurves.FirstElement().Category.Id, false);
                    FilteredElementCollector pipeAcc = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctAccessory);
                    if (pipeAcc.Count() != 0)
                        view3D.SetCategoryHidden(pipeAcc.FirstElement().Category.Id, false);
                    FilteredElementCollector pipeFit = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctFitting);
                    if (pipeFit.Count() != 0)
                        view3D.SetCategoryHidden(pipeFit.FirstElement().Category.Id, false);
                    FilteredElementCollector mechEq = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MechanicalEquipment);
                    if (mechEq.Count() != 0)
                        view3D.SetCategoryHidden(mechEq.FirstElement().Category.Id, false);

                    setView.Commit();
                }
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
            string filterName = "Фильтр для Системы вентиляции " + value;
            var filter = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().Where(x => x.Name == filterName).ToList().FirstOrDefault();
            if (filter != null)
            {
                using (Transaction t = new Transaction(doc, "Delete Filter"))
                {
                    t.Start();
                    doc.Delete(filter.Id);
                    t.Commit();
                }
            }
                
            try
            {
                using (Transaction t = new Transaction(doc, "Add view filter"))
                {
                    t.Start();

                    // Create filter element assocated to the input categories
                    ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, filterName, categories);

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

                        List<string> filterNames = new List<string>();
                        string viewName = view.Name.Replace("Схема вентиляции ", "");
                        if (viewName.Contains(","))
                        {
                            filterNames = viewName.Split(',').ToList();
                        }

                        filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(sharedParamId, value, true));
                        if (filterNames != null)
                        {
                            foreach (string f in filterNames)
                            {
                                filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(sharedParamId, f, true));
                            }
                        }
                    }

                    ElementFilter elemFilter = CreateElementFilterFromFilterRules(filterRules);
                    parameterFilterElement.SetElementFilter(elemFilter);

                    // Apply filter to view
                    view.AddFilter(parameterFilterElement.Id);
                    view.SetFilterVisibility(parameterFilterElement.Id, false);
                    t.Commit();
                }
            }
             catch
            {
                try
                {
                    using (Transaction t = new Transaction(doc, "Add Filter"))
                    {
                        t.Start();
                        var parameterFilterElement = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().Where(x => x.Name == filterName).ToList().FirstOrDefault();
                        view.SetFilterVisibility(parameterFilterElement.Id, false);
                        t.Commit();
                    }
                }
                catch { };
            }
        }

        public bool AreTheseSystemsMustBeTogether(string str1, string str2)
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
