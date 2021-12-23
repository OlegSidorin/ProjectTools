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
        // Готовит виды схемы для труб по количеству систем в проекте
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            FOP.AddSharedParameter_M1_MEP_System(commandData);
            

            List<Element> allElements = new List<Element>();
            var pipes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).ToList();
            pipes.ForEach(x => allElements.Add(x));
            var pipeAccessories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory).ToList();
            pipeAccessories.ForEach(x => allElements.Add(x));
            var pipeFittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting).ToList();
            pipeFittings.ForEach(x => allElements.Add(x));
            var plumbingFixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlumbingFixtures).ToList();
            plumbingFixtures.ForEach(x => allElements.Add(x));
            var mechanicalEquipment = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_MechanicalEquipment).ToList();
            mechanicalEquipment.ForEach(x => allElements.Add(x));
            var pipesAxes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlaceHolderPipes).ToList();
            pipesAxes.ForEach(x => allElements.Add(x));

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
                                if (spaceIndex != 0)
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

            //var messList = new FilteredElementCollector(doc)
            //    .OfCategory(BuiltInCategory.OST_PipeCurves)
            //    .WhereElementIsNotElementType()
            //    .Cast<MEPCurve>()
            //    .ToList();

            var mepSystemNames = GetDistinctStringList(messStrings);

            var mepSystemNames_FirstPart = GetFirstPartStringList(mepSystemNames);

            mepSystemNames = GetDistinctStringList(mepSystemNames_FirstPart);

            var viewList = new List<View3D>();
            foreach (var item in mepSystemNames)
            {
                View3D view = CreateView("Схема труб " + item, doc, doc.ActiveView);
                if (view != null)
                    viewList.Add(view);
            }

            //var viewFor2List = new List<View3D>();

            //var mepTwoSystemsNames = new List<string>();
            //foreach (var sn1 in mepSystemNames)
            //{
            //    foreach (var sn2 in mepSystemNames)
            //    {
            //        if (AreTheseSystemsSame2(sn1, sn2, out string resultString))
            //        {
            //            mepTwoSystemsNames.Add(resultString);
            //        }
            //    }
            //}

            //foreach (var item in mepTwoSystemsNames)
            //{
            //    View3D view = CreateView(item, doc);
            //    if (view != null)
            //        viewFor2List.Add(view);
            //}

            var elementIdsToHide = GetDistinctElementIdList(GetAllElementIds(doc));
            
            foreach (var view in viewList)
            {
                try
                {
                    SetView(view, doc, elementIdsToHide);
                    CreateViewFilterForOneSystem(doc, view, view.Name.Replace("Схема труб ", ""));
                }
                catch { };
            }

            //foreach (var view in viewFor2List)
            //{
            //    SetView(view, doc, elementIdsToHide, viewDetailLevel, displayStyle);
            //    CreateViewFilterForTwoSystems(doc, view, viewNameFromTwo(view.Name, 0), viewNameFromTwo(view.Name, 1));
            //}

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
        private List<string> GetDistinctStringList(List<Element> elementList)
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
            if (elementList != null)
            {
                foreach (var element in elementList)
                {
                    if (element != null)
                    {
                        try
                        {
                            MEPCurve mepCurve = (MEPCurve)element;
                            if (!isStringInAList(mepCurve.MEPSystem.Name, outputList))
                            {
                                outputList.Add(mepCurve.MEPSystem.Name);
                            }
                        }
                        catch { };
                    }

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
            if (messList != null)
            {
                foreach (var element in messList)
                {
                    if (element != null)
                    {
                        try
                        {
                            if (!isStringInAList(element.MEPSystem.Name, outputList))
                            {
                                outputList.Add(element.MEPSystem.Name);
                            }
                        }
                        catch { };
                    }

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
                        view3D.LookupParameter("ADSK_Назначение вида").Set("Схемы труб");
                        //activeView3D.LookupParameter("ADSK_Штамп Раздел проекта").Set("Схемы труб");

                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                    }

                    view3D.Discipline = ViewDiscipline.Mechanical;
                    view3D.DisplayStyle = DisplayStyle.HLR;
                    view3D.DetailLevel = ViewDetailLevel.Coarse;

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
                                p.Set("Схемы труб");

                    if (p3List != null || p3List.Count != 0)
                        foreach (var p in p2List)
                            if (!p.IsReadOnly)
                                p.Set("Схемы труб");

                    view3D.AreImportCategoriesHidden = true;
                    view3D.AreAnalyticalModelCategoriesHidden = true;
                    //activeView3D.AreAnnotationCategoriesHidden = true;

                    foreach (var elId in hideList)
                    {
                        view3D.HideCategoryTemporary(elId);
                    }

                    // переводит временно скрытые категории в постоянные
                    view3D.ConvertTemporaryHideIsolateToPermanent();

                    FilteredElementCollector pipeCurves = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves);
                    if (pipeCurves.Count() != 0)
                        view3D.SetCategoryHidden(pipeCurves.FirstElement().Category.Id, false);
                    FilteredElementCollector pipeAcc = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory);
                    if (pipeAcc.Count() != 0)
                        view3D.SetCategoryHidden(pipeAcc.FirstElement().Category.Id, false);
                    FilteredElementCollector pipeFit = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting);
                    if (pipeFit.Count() != 0)
                        view3D.SetCategoryHidden(pipeFit.FirstElement().Category.Id, false);
                    FilteredElementCollector plumFix = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlumbingFixtures);
                    if (plumFix.Count() != 0)
                        view3D.SetCategoryHidden(plumFix.FirstElement().Category.Id, false);
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

        public static void CreateViewFilterForOneSystem(Document doc, View view, string value)
        {
            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
            categories.Add(new ElementId(BuiltInCategory.OST_PlumbingFixtures));
            categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
            List<FilterRule> filterRules = new List<FilterRule>();
            string filterName = "Фильтр для Системы труб " + value;
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
                using (Transaction t = new Transaction(doc, "Add view filter for one system"))
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


                    var pipes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).ToList();
                    //var pipeAccessories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory).ToList();
                    //var pipeFittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting).ToList();
                    //var plumbingFixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlumbingFixtures).ToList();

                    Element el = pipes.First();

                    if (el != null)
                    {
                        Parameter sharedParam = el.get_Parameter(spGuid);
                        ElementId sharedParamId = sharedParam.Id;

                        List<string> filterNames = new List<string>();
                        string viewName = view.Name.Replace("Схема труб ", "");
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
        public static void CreateViewFilterForTwoSystems(Document doc, View view, string value1, string value2)
        {
            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
            categories.Add(new ElementId(BuiltInCategory.OST_PlumbingFixtures));
            categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
            List<FilterRule> filterRules = new List<FilterRule>();
            

            using (Transaction t = new Transaction(doc, "Add view filter for two systems"))
            {
                t.Start();

                // Create filter element assocated to the input categories
                ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Фильтр для Систем " + value1 + ", " + value2, categories);

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


                var pipes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).ToList();
                //var pipeAccessories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory).ToList();
                //var pipeFittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting).ToList();
                //var plumbingFixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlumbingFixtures).ToList();

                Element el = pipes.First();

                if (el != null)
                {
                    Parameter sharedParam = el.get_Parameter(spGuid);
                    ElementId sharedParamId = sharedParam.Id;
                    
                    filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(sharedParamId, value1, true));
                    filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(sharedParamId, value2, true));
                    
                }

                ElementFilter elemFilter = CreateElementFilterFromFilterRules(filterRules);
                parameterFilterElement.SetElementFilter(elemFilter);

                // Apply filter to view
                view.AddFilter(parameterFilterElement.Id);
                view.SetFilterVisibility(parameterFilterElement.Id, false);
                t.Commit();
            }
        }

        public static void CreateViewFilterSystems(Document doc, View view, List<string> list)
        {
            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_PipeCurves));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeAccessory));
            categories.Add(new ElementId(BuiltInCategory.OST_PipeFitting));
            categories.Add(new ElementId(BuiltInCategory.OST_PlumbingFixtures));
            categories.Add(new ElementId(BuiltInCategory.OST_MechanicalEquipment));
            List<FilterRule> filterRules = new List<FilterRule>();

            using (Transaction t = new Transaction(doc, "Add view filter for two systems"))
            {
                t.Start();

                // Create filter element assocated to the input categories
                string names = "";
                foreach (var s in list)
                {
                    names += s + ", ";
                }
                names = names.Substring(0, names.Length - 2);
                ParameterFilterElement parameterFilterElement = ParameterFilterElement.Create(doc, "Фильтр для Систем " + names, categories);

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


                var pipes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).ToList();
                //var pipeAccessories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory).ToList();
                //var pipeFittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting).ToList();
                //var plumbingFixtures = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PlumbingFixtures).ToList();

                Element el = pipes.First();

                if (el != null)
                {
                    Parameter sharedParam = el.get_Parameter(spGuid);
                    ElementId sharedParamId = sharedParam.Id;

                    //filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(sharedParamId, value1, true));
                    //filterRules.Add(ParameterFilterRuleFactory.CreateNotContainsRule(sharedParamId, value2, true));

                }

                ElementFilter elemFilter = CreateElementFilterFromFilterRules(filterRules);
                parameterFilterElement.SetElementFilter(elemFilter);

                // Apply filter to view
                view.AddFilter(parameterFilterElement.Id);
                view.SetFilterVisibility(parameterFilterElement.Id, false);
                t.Commit();
            }
        }
        public bool AreTheseSystemsSame(string str1, string str2, out string stringSum)
        {
            stringSum = "";
            int len1 = str1.Length;
            int len2 = str2.Length;
            int difference = 0;
            if (len1 != len2)
            {
                return false;
            }
            else
            {
                stringSum = $"{str1},{str2}";

                if (str1.Contains("T1 1") && str2.Contains("T2 1") || str1.Contains("Т1 1") && str2.Contains("Т2 1"))
                    return true;
                if (str1.Contains("T1 2") && str2.Contains("T2 2") || str1.Contains("Т1 2") && str2.Contains("Т2 2"))
                    return true;
                if (str1.Contains("T1 3") && str2.Contains("T2 3") || str1.Contains("Т1 3") && str2.Contains("Т2 3"))
                    return true;
                if (str1.Contains("T1 4") && str2.Contains("T2 4") || str1.Contains("Т1 4") && str2.Contains("Т2 4"))
                    return true;
                if (str1.Contains("T1 5") && str2.Contains("T2 5") || str1.Contains("Т1 5") && str2.Contains("Т2 5"))
                    return true;
                if (str1.Contains("T1 6") && str2.Contains("T2 6") || str1.Contains("Т1 6") && str2.Contains("Т2 6"))
                    return true;
                if (str1.Contains("T1 7") && str2.Contains("T2 7") || str1.Contains("Т1 7") && str2.Contains("Т2 7"))
                    return true;
                if (str1.Contains("T1 8") && str2.Contains("T2 8") || str1.Contains("Т1 8") && str2.Contains("Т2 8"))
                    return true;
                if (str1.Contains("T1 9") && str2.Contains("T2 9") || str1.Contains("Т1 9") && str2.Contains("Т2 9"))
                    return true;

                if (str1.Contains("X1 1") && str2.Contains("X2 1") || str1.Contains("Х1 1") && str2.Contains("Х2 1"))
                    return true;
                if (str1.Contains("X1 2") && str2.Contains("X2 2") || str1.Contains("Х1 2") && str2.Contains("Х2 2"))
                    return true;
                if (str1.Contains("X1 3") && str2.Contains("X2 3") || str1.Contains("Х1 3") && str2.Contains("Х2 3"))
                    return true;
                if (str1.Contains("X1 4") && str2.Contains("X2 4") || str1.Contains("Х1 4") && str2.Contains("Х2 4"))
                    return true;
                if (str1.Contains("X1 5") && str2.Contains("X2 5") || str1.Contains("Х1 5") && str2.Contains("Х2 5"))
                    return true;
                if (str1.Contains("X1 6") && str2.Contains("X2 6") || str1.Contains("Х1 6") && str2.Contains("Х2 6"))
                    return true;
                if (str1.Contains("X1 7") && str2.Contains("X2 7") || str1.Contains("Х1 7") && str2.Contains("Х2 7"))
                    return true;
                if (str1.Contains("X1 8") && str2.Contains("X2 8") || str1.Contains("Х1 8") && str2.Contains("Х2 8"))
                    return true;
                if (str1.Contains("X1 9") && str2.Contains("X2 9") || str1.Contains("Х1 9") && str2.Contains("Х2 9"))
                    return true;

                if (str1.Contains("T11 1") && str2.Contains("T21 1") || str1.Contains("Т11 1") && str2.Contains("Т21 1"))
                    return true;
                if (str1.Contains("T11 2") && str2.Contains("T21 2") || str1.Contains("Т11 2") && str2.Contains("Т21 2"))
                    return true;
                if (str1.Contains("T11 3") && str2.Contains("T21 3") || str1.Contains("Т11 3") && str2.Contains("Т21 3"))
                    return true;
                if (str1.Contains("T11 4") && str2.Contains("T21 4") || str1.Contains("Т11 4") && str2.Contains("Т21 4"))
                    return true;
                if (str1.Contains("T11 5") && str2.Contains("T21 5") || str1.Contains("Т11 5") && str2.Contains("Т21 5"))
                    return true;
                if (str1.Contains("T11 6") && str2.Contains("T21 6") || str1.Contains("Т11 6") && str2.Contains("Т21 6"))
                    return true;
                if (str1.Contains("T11 7") && str2.Contains("T21 7") || str1.Contains("Т11 7") && str2.Contains("Т21 7"))
                    return true;
                if (str1.Contains("T11 8") && str2.Contains("T21 8") || str1.Contains("Т11 8") && str2.Contains("Т21 8"))
                    return true;
                if (str1.Contains("T11 9") && str2.Contains("T21 9") || str1.Contains("Т11 9") && str2.Contains("Т21 9"))
                    return true;

                if (str1.Contains("T3 1") && str2.Contains("T4 1") || str1.Contains("Т3 1") && str2.Contains("Т4 1"))
                    return true;

                if (str1.Contains("T5 1") && str2.Contains("T6 1") || str1.Contains("Т5 1") && str2.Contains("Т6 1"))
                    return true;


            }
            return false;
        }
        public bool AreTheseSystemsSame2(string str1, string str2, out string stringSum)
        {
            stringSum = "";
            int len1 = str1.Length;
            int len2 = str2.Length;
            int difference = 0;
            if (len1 != len2)
            {
                return false;
            }
            else
            {
                stringSum = $"{str1},{str2}";

                if (str1.Contains("T1") && str2.Contains("T2") || str1.Contains("Т1") && str2.Contains("Т2"))
                    return true;

                if (str1.Contains("X1") && str2.Contains("X2") || str1.Contains("Х1") && str2.Contains("Х2"))
                    return true;
               

                if (str1.Contains("T11") && str2.Contains("T21") || str1.Contains("Т11") && str2.Contains("Т21"))
                    return true;
                

                if (str1.Contains("T3") && str2.Contains("T4") || str1.Contains("Т3") && str2.Contains("Т4"))
                    return true;

                if (str1.Contains("T5") && str2.Contains("T6") || str1.Contains("Т5") && str2.Contains("Т6"))
                    return true;

            }
            return false;
        }

        public List<string> UnicSystems(List<string> list)
        {
            List<(string, string)> dividedSystemNames = new List<(string, string)>();
            foreach (string s in list)
            {
                int spaceIndex = 0;
                spaceIndex = s.IndexOf(" "); // string[] words = s.Split(new char[] { ' ' });
                if (spaceIndex == 0)
                {
                    dividedSystemNames.Add((s, ""));
                }
                else 
                {
                    dividedSystemNames.Add((s.Substring(0, spaceIndex), s.Substring(spaceIndex + 1)));
                }
            }

            
            foreach (var item in dividedSystemNames)
            {

            }
            return null;
        }

        public string viewNameFromTwo(string str, int number)
        {
            string output = "";
            string[] words = str.Split(',');
            if (words[number] != null)
                output = words[number];
            return output;
        }

    }
    public class OurMEPSystem
    {
        public string Name { get; set; }
        public List<string> SameSystemNames { get; set; }
    }
}
