using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Linq;
using System.Windows.Forms;

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class Command02 : IExternalCommand
    {
        // Распределяет связи по рабочим наборам
        Result IExternalCommand.Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;
            if (doc.IsWorkshared == false)
            {
                TaskDialog.Show("Worksets", "Документ не для совместной работы");
            }
            if (doc.IsWorkshared == true)
            {
                var ws_set = new List<ChangingElement>();
                var rvtLinks = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkInstance)).WhereElementIsNotElementType().ToList();
                foreach (var element in rvtLinks)
                {
                    WSNames razdel = WSNames.Unknown;
                    var name = element.Name;
                    if (name.Contains("AP_") || name.Contains("AP_") || name.Contains("AР_") || name.Contains("АP_")) razdel = WSNames.AR;
                    if (name.Contains("AК_") || name.Contains("AK_") || name.Contains("AК_") || name.Contains("АК_")) razdel = WSNames.AK;
                    if (name.Contains("КP_") || name.Contains("KP_") || name.Contains("KР_") || name.Contains("КP_")) razdel = WSNames.KR;
                    if (name.Contains("ОВ_") || name.Contains("ОВ1_") || name.Contains("ОВ2_") || name.Contains("ОВ3_") || name.Contains("ОВ4_")) razdel = WSNames.OV;
                    if (name.Contains("КФ_")) razdel = WSNames.KF;
                    if (name.Contains("ВК_")) razdel = WSNames.VK;
                    if (name.Contains("МГ_")) razdel = WSNames.MG;
                    if (name.Contains("ТХ_")) razdel = WSNames.TH;
                    if (name.Contains("ЭОМ_")) razdel = WSNames.EOM;
                    if (name.Contains("СС_")) razdel = WSNames.CC;

                    var w = new ChangingElement()
                    {
                        WSName = razdel,
                        ElementLink = element
                    };
                    ws_set.Add(w);
                }
                using (Transaction tr = new Transaction(doc, "Work Set Naming"))
                {
                    tr.Start();
                    foreach (var w in ws_set)
                    {
                        try
                        {
                            var ws = CreateWorkset(doc, w.Name);
                        }
                        catch { };
                        try
                        {
                            //var rvtLink = (RevitLinkInstance)w.ElementLink;
                            w.ElementLink.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(GetIdInt(doc, w.Name));
                        }
                        catch { };

                    }
                    tr.Commit();
                }

                PlaceInWS(doc, typeof(ImportInstance), WSNames.DWG);

            }
            return Result.Succeeded;
        }
        private Workset CreateWorkset(Document document, String wsname)
        {
            Workset newWorkset = null;
            // Worksets can only be created in a document with worksharing enabled
            if (document.IsWorkshared)
            {
                string worksetName = wsname;
                // Workset name must not be in use by another workset
                if (WorksetTable.IsWorksetNameUnique(document, worksetName))
                {
                    //using (Transaction worksetTransaction = new Transaction(document, "Set preview view id"))
                    //{
                    //    worksetTransaction.Start();
                    newWorkset = Workset.Create(document, worksetName);
                    //worksetTransaction.Commit();
                    //}
                }
            }

            return newWorkset;
        }
        private int GetIdInt(Document doc, string wsname)
        {
            int i = 0;
            IList<Workset> worksetList = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets();
            foreach (Workset workset in worksetList)
            {
                if (workset.Name.Contains(wsname))
                {
                    i = workset.Id.IntegerValue;
                }

            }
            return i;
        }
        private void PlaceInWS(Document doc, Type type, WSNames wsName)
        {
            var ws_set = new List<ChangingElement>();
            var links = new FilteredElementCollector(doc).OfClass(type).WhereElementIsNotElementType().ToList();
            foreach (var element in links)
            {
                var w = new ChangingElement()
                {
                    WSName = wsName,
                    ElementLink = (Element)element
                };
                ws_set.Add(w);
            }
            using (Transaction tr = new Transaction(doc, "Work Set Naming"))
            {
                tr.Start();
                foreach (var w in ws_set)
                {
                    try
                    {
                        var ws = CreateWorkset(doc, w.Name);
                    }
                    catch { };
                    try
                    {
                        //var dwgLink = (ImportInstance)w.ElementLink;
                        w.ElementLink.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(GetIdInt(doc, w.Name));
                    }
                    catch { };

                }
                tr.Commit();
            }
        }
        private void PlaceInWS(Document doc, BuiltInCategory builtInCategory, WSNames wsName)
        {
            var ws_set = new List<ChangingElement>();
            var links = new FilteredElementCollector(doc).OfCategory(builtInCategory).WhereElementIsNotElementType().ToList();
            foreach (var element in links)
            {
                var w = new ChangingElement()
                {
                    WSName = wsName,
                    ElementLink = (Element)element
                };
                ws_set.Add(w);
            }
            using (Transaction tr = new Transaction(doc, "Work Set Naming"))
            {
                tr.Start();
                foreach (var w in ws_set)
                {
                    try
                    {
                        var ws = CreateWorkset(doc, w.Name);
                    }
                    catch { };
                    try
                    {
                        //var dwgLink = (ImportInstance)w.ElementLink;
                        w.ElementLink.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(GetIdInt(doc, w.Name));
                    }
                    catch { };

                }
                tr.Commit();
            }
        }
        private void PlacePipesInWS(Document doc)
        {
            var ws_set = new List<ChangingElement>();

            var pipes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsNotElementType().ToList();
            var fittings = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeFitting).WhereElementIsNotElementType().ToList();
            var accessories = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_PipeAccessory).WhereElementIsNotElementType().ToList();
            foreach (var item in fittings)
            {
                pipes.Add(item);
            };
            foreach (var item in accessories)
            {
                pipes.Add(item);
            };


            foreach (var element in pipes)
            {
                var map = element.ParametersMap;
                foreach (Parameter p in map)
                {
                    try
                    {
                        Definition def = p.Definition;
                        InternalDefinition iDef = (InternalDefinition)def;
                        if (iDef.BuiltInParameter == BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM)
                        {
                            if (p.AsValueString().ToLower().Contains("отопление"))
                            {
                                var w = new ChangingElement()
                                {
                                    WSName = WSNames.OTOP,
                                    ElementLink = (Element)element
                                };
                                ws_set.Add(w);
                            }
                            if (p.AsValueString().ToLower().Contains("холодоснабжение"))
                            {
                                var w = new ChangingElement()
                                {
                                    WSName = WSNames.HOLOD,
                                    ElementLink = (Element)element
                                };
                                ws_set.Add(w);
                            }
                            if (p.AsValueString().ToLower().Contains("водоснабжение"))
                            {
                                var w = new ChangingElement()
                                {
                                    WSName = WSNames.VODOSNAB,
                                    ElementLink = (Element)element
                                };
                                ws_set.Add(w);
                            }
                            if (p.AsValueString().ToLower().Contains("водоотведение"))
                            {
                                var w = new ChangingElement()
                                {
                                    WSName = WSNames.KANAL,
                                    ElementLink = (Element)element
                                };
                                ws_set.Add(w);
                            }
                        }
                    }
                    catch { }
                }
            }
            using (Transaction tr = new Transaction(doc, "Work Set Naming"))
            {
                tr.Start();
                foreach (var w in ws_set)
                {
                    try
                    {
                        var ws = CreateWorkset(doc, w.Name);
                    }
                    catch { };
                    try
                    {
                        w.ElementLink.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(GetIdInt(doc, w.Name));
                    }
                    catch { };

                }
                tr.Commit();
            }
        }
    }
    public class ChangingElement
    {
        public string Name 
        { 
            get
            {
                switch (WSName)
                {
                    case WSNames.AR:
                        return "#Модель АР";
                    case WSNames.KR:
                        return "#Модель КР";
                    case WSNames.KF:
                        return "#Модель КФ";
                    case WSNames.OV:
                        return "#Модель ОВ";
                    case WSNames.VK:
                        return "#Модель ВК";
                    case WSNames.MG:
                        return "#Модель МГ";
                    case WSNames.TH:
                        return "#Модель ТХ";
                    case WSNames.EOM:
                        return "#Модель ЭОМ";
                    case WSNames.IFC:
                        return "#IFC";
                    case WSNames.DWG:
                        return "#DWG";
                    case WSNames.TRUBO:
                        return "Трубопроводы";
                    case WSNames.OTOP:
                        return "Отопление";
                    case WSNames.HOLOD:
                        return "Холодоснабжение";
                    case WSNames.VODOSNAB:
                        return "Водоснабжение";
                    case WSNames.KANAL:
                        return "Водоотведение";
                        case WSNames.VENT:
                        return "Вентиляция";
                    case WSNames.Unknown:
                        return "Неизвестное";

                        default: return "Unknown";
                }
            }
        }
        public WSNames WSName { get; set; }
        public Element ElementLink { get; set; }
    }
    public enum WSNames
    {
        Unknown = 0,
        AR = 1,
        KR = 2,
        KF = 3,
        TH = 4,
        OV = 5,
        MG = 6,
        EOM = 7,
        VK = 8,
        AK = 9,
        CC = 10,
        IFC = 11,
        DWG = 12,
        TRUBO = 13,
        OTOP = 14,
        HOLOD = 15,
        VENT = 16,
        VODOSNAB = 17,
        KANAL = 18
    }
}
