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
using System.Data;
using System.Collections.ObjectModel;

namespace ProjectTools
{
    public class ParameterViewModel
    {
        public string Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string ParameterGroup { get; set; }
        public string ParameterType { get; set; }
        public string FamilyParameterGroup { get; set; }
        public string FamilyParameterType { get; set; }
        public string FamilyValue { get; set; }

        public ParameterViewModel()
        {

        }

        private List<string> GetGroupsOfFOP(Application app)
        {
            List<string> output = new List<string>();
            app.SharedParametersFilename = FOP.PathToFOP;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (var item in definitionGroups)
            {
                output.Add(item.Name);
            }
            return output;
        }
        private List<List<string>> GetGroupItemsOfFOP(Application app)
        {
            List<List<string>> output = new List<List<string>>();
            app.SharedParametersFilename = FOP.PathToFOP;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                List<string> items = new List<string>();
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    items.Add(definition.Name);
                }
                output.Add(items);
            }
            return output;
        }
        public ParameterViewModel[] AllParameters(Application app)
        {
            var output = new List<ParameterViewModel>();
            app.SharedParametersFilename = FOP.PathToFOP;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    ExternalDefinition externalDefinition = definition as ExternalDefinition;
                    var parameterProperties = new ParameterViewModel()
                    {
                        Id = "",
                        Guid = externalDefinition.GUID.ToString(),
                        Name = externalDefinition.Name,
                        ParameterGroup = externalDefinition.OwnerGroup.Name,
                        ParameterType = definition.ParameterType.ToString(),
                        FamilyParameterGroup = "Прочее",
                        FamilyParameterType = "Тип",
                        FamilyValue = ""
                    };
                    output.Add(parameterProperties);
                }
            }

            return output.ToArray();
        }
        public bool IsInstance()
        {
            if (FamilyParameterType == "Экземпляр") return true;
            else return false;
        }
        public List<string> GroupsList(Application app)
        {
            var output = new List<string>();
            app.SharedParametersFilename = FOP.PathToFOP;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                output.Add(definitionGroup.Name);
            }

            return output;
        }
        public ParameterViewModel GetParameterByName(Application app, string name)
        {
            var output = new ParameterViewModel();
            app.SharedParametersFilename = FOP.PathToFOP;
            DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                List<string> items = new List<string>();
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    if (definition.Name == name)
                    {
                        ExternalDefinition externalDefinition = definition as ExternalDefinition;
                        output.Id = "";
                        output.Guid = externalDefinition.GUID.ToString();
                        output.Name = externalDefinition.Name;
                        output.ParameterGroup = externalDefinition.OwnerGroup.Name;
                        output.ParameterType = definition.ParameterType.ToString();
                        output.FamilyParameterGroup = "Прочее";
                        output.FamilyParameterType = "Тип";
                        output.FamilyValue = "";
                    }
                }
            }
            return output;
        }
        public BuiltInParameterGroup PGGroup()
        {
            if (FamilyParameterGroup == "Моменты") return BuiltInParameterGroup.PG_MOMENTS;
            if (FamilyParameterGroup == "Силы") return BuiltInParameterGroup.PG_FORCES;
            if (FamilyParameterGroup == "Геометрия разделения") return BuiltInParameterGroup.PG_DIVISION_GEOMETRY;
            if (FamilyParameterGroup == "Сегменты и соединительные детали") return BuiltInParameterGroup.PG_SEGMENTS_FITTINGS;
            if (FamilyParameterGroup == "Общая легенда") return BuiltInParameterGroup.PG_OVERALL_LEGEND;
            if (FamilyParameterGroup == "Видимость") return BuiltInParameterGroup.PG_VISIBILITY;
            if (FamilyParameterGroup == "Данные") return BuiltInParameterGroup.PG_DATA;
            if (FamilyParameterGroup == "Электросети - Создание цепей") return BuiltInParameterGroup.PG_ELECTRICAL_CIRCUITING;
            if (FamilyParameterGroup == "Общие") return BuiltInParameterGroup.PG_GENERAL;
            if (FamilyParameterGroup == "Свойства модели") return BuiltInParameterGroup.PG_ADSK_MODEL_PROPERTIES;
            if (FamilyParameterGroup == "Результаты анализа") return BuiltInParameterGroup.PG_ANALYSIS_RESULTS;
            if (FamilyParameterGroup == "Редактирование формы перекрытия") return BuiltInParameterGroup.PG_SLAB_SHAPE_EDIT;
            if (FamilyParameterGroup == "Фотометрические") return BuiltInParameterGroup.PG_LIGHT_PHOTOMETRICS;
            if (FamilyParameterGroup == "Свойства экологически чистого здания") return BuiltInParameterGroup.PG_GREEN_BUILDING;
            if (FamilyParameterGroup == "Шрифт заголовков") return BuiltInParameterGroup.PG_TITLE;
            if (FamilyParameterGroup == "Система пожаротушения") return BuiltInParameterGroup.PG_FIRE_PROTECTION;
            if (FamilyParameterGroup == "Аналитическая модель") return BuiltInParameterGroup.PG_ANALYTICAL_MODEL;
            if (FamilyParameterGroup == "Набор арматурных стержней") return BuiltInParameterGroup.PG_REBAR_ARRAY;
            if (FamilyParameterGroup == "Слои") return BuiltInParameterGroup.PG_REBAR_SYSTEM_LAYERS;
            if (FamilyParameterGroup == "Параметры IFC") return BuiltInParameterGroup.PG_IFC;
            if (FamilyParameterGroup == "Электросети (А)") return BuiltInParameterGroup.PG_AELECTRICAL;
            if (FamilyParameterGroup == "Рачет энергопотребления") return BuiltInParameterGroup.PG_ENERGY_ANALYSIS;
            if (FamilyParameterGroup == "Расчет несущих конструкций") return BuiltInParameterGroup.PG_STRUCTURAL_ANALYSIS;
            if (FamilyParameterGroup == "Механизмы - Расход") return BuiltInParameterGroup.PG_MECHANICAL_AIRFLOW;
            if (FamilyParameterGroup == "Механизмы - Нагрузки") return BuiltInParameterGroup.PG_MECHANICAL_LOADS;
            if (FamilyParameterGroup == "Электросети - Нагрузки") return BuiltInParameterGroup.PG_ELECTRICAL_LOADS;
            if (FamilyParameterGroup == "Электросети - Освещение") return BuiltInParameterGroup.PG_ELECTRICAL_LIGHTING;
            if (FamilyParameterGroup == "Текст") return BuiltInParameterGroup.PG_TEXT;
            if (FamilyParameterGroup == "Зависимости") return BuiltInParameterGroup.PG_CONSTRAINTS;
            if (FamilyParameterGroup == "Стадии") return BuiltInParameterGroup.PG_PHASING;
            if (FamilyParameterGroup == "Механизмы") return BuiltInParameterGroup.PG_MECHANICAL;
            if (FamilyParameterGroup == "Несущие конструкции") return BuiltInParameterGroup.PG_STRUCTURAL;
            if (FamilyParameterGroup == "Сантехника") return BuiltInParameterGroup.PG_PLUMBING;
            if (FamilyParameterGroup == "Электросети") return BuiltInParameterGroup.PG_ELECTRICAL;
            if (FamilyParameterGroup == "Материалы и отделка") return BuiltInParameterGroup.PG_MATERIALS;
            if (FamilyParameterGroup == "Графика") return BuiltInParameterGroup.PG_GRAPHICS;
            if (FamilyParameterGroup == "Строительство") return BuiltInParameterGroup.PG_CONSTRUCTION;
            if (FamilyParameterGroup == "Размеры") return BuiltInParameterGroup.PG_GEOMETRY;
            if (FamilyParameterGroup == "Идентификация") return BuiltInParameterGroup.PG_IDENTITY_DATA;
            if (FamilyParameterGroup == "Прочее") return BuiltInParameterGroup.INVALID;

            return BuiltInParameterGroup.INVALID;
        }

        private string GroupNameBySharedParameterName(ExternalCommandData commandData)
        {
            string outputGroupName = "";

            DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
            DefinitionGroups definitionGroups = sharedParametersFile.Groups;
            foreach (DefinitionGroup definitionGroup in definitionGroups)
            {
                foreach (Definition definition in definitionGroup.Definitions)
                {
                    if (definition.Name == Name)
                        outputGroupName = definitionGroup.Name;
                }
            }

            return outputGroupName;
        }

        public string AddSharedParameterIntoFamily(ExternalCommandData commandData, Document doc)
        {
            string str = "";

            FamilyManager familyManager = doc.FamilyManager;
            FamilyType familyType = familyManager.CurrentType;
            FamilyTypeSet types = familyManager.Types;

            #region check if family has no type
            if (familyType == null)
            {
                using (Transaction t = new Transaction(doc, "change"))
                {
                    t.Start();
                    familyType = familyManager.NewType("Тип 1");
                    familyManager.CurrentType = familyType;
                    t.Commit();
                }
            }
            #endregion

            FamilyParameterSet parametersList = familyManager.Parameters;

            #region check tha parameter already in doc
            foreach (FamilyParameter p in parametersList)
            {
                if (p.Definition.Name == Name)
                {
                    string addedToStr = "";
                    var docName = doc.Title + ".rfa";
                    //try
                    //{
                    //    addedToStr += CM.CloseDocSimple(doc);
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw ex;
                    //}

                    return ":: " + "Параметр " + Name + " существует в семействе " + docName + addedToStr;
                }

            }
            #endregion

            #region add parameter
            try
            {
                //app.SharedParametersFilename = CommandForAddingParameters.FOPPath;
                using (Transaction t = new Transaction(doc, "Add paramter"))
                {
                    t.Start();
                    DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
                    DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(GroupNameBySharedParameterName(commandData));
                    Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(Name);
                    ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;
                    FamilyParameter familyParameter = familyManager.AddParameter(externalDefinition, PGGroup(), IsInstance());
                    str = "+ " + familyParameter.Definition.Name + " был успешно добавлен в семейство " + doc.Title + ".rfa";
                    t.Commit();
                }
                using (Transaction t2 = new Transaction(doc, "Add Value"))
                {
                    t2.Start();
                    foreach (FamilyType ft in types)
                    {
                        familyManager.CurrentType = ft;
                        var p = familyManager.get_Parameter(Name);
                        if (ParameterType == "Text" || ParameterType == "URL")
                        {
                            familyManager.Set(p, FamilyValue);
                            str += $"\n  ... присвоено в типе {ft.Name} текстовое значение: " + FamilyValue;
                        }
                        else if (ParameterType == "Number" || ParameterType == "Length" || ParameterType == "Area" || ParameterType == "Volume" || ParameterType == "Angle")
                        {
                            if (double.TryParse(FamilyValue, out double doubleValue))
                            {
                                familyManager.Set(p, doubleValue);
                                str += $"\n  ... присвоено в типе {ft.Name} числовое значение: " + FamilyValue;
                            };

                        }
                        else if (ParameterType == "Integer")
                        {
                            if (int.TryParse(FamilyValue, out int intValue))
                            {
                                familyManager.Set(p, intValue);
                                str += $"\n  ... присвоено в типе {ft.Name} целочисленное значение: " + FamilyValue;
                            };

                        }
                        else if (ParameterType == "YesNo")
                        {
                            if (FamilyValue.ToLower().Contains("да") || FamilyValue.ToLower().Contains("yes") || FamilyValue.ToLower().Contains("true"))
                            {
                                familyManager.Set(p, (int)1);
                                str += $"\n  ... присвоено в типе {ft.Name} логическое значение: true";
                            }
                            else
                            {
                                familyManager.Set(p, (int)0);
                                str += $"\n  ... присвоено в типе {ft.Name} логическое значение: false";
                            }

                        }
                        else
                        {

                        }

                    }
                    t2.Commit();
                }

            }
            catch (Exception e)
            {
                str = "! " + Name + " не удалось добавить в семейство " + doc.Title + ".rfa" + e.ToString();
            }
            #endregion

            // str += CM.SaveAndCloseDocSimple(doc);

            return str;

        }

        public string AddSharedParameterIntoProject(ExternalCommandData commandData, Document doc, CategorySet catSet, BuiltInParameterGroup builtInParameterGroup) //(string prm, CategorySet catSet, Application app, Document doc)
        {
            Application app = commandData.Application.Application;
            app.SharedParametersFilename = FOP.PathToFOP;
            #region add parameter
            try
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Add Shared Parameter");
                    DefinitionFile sharedParametersFile = app.OpenSharedParameterFile();
                    DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(GroupNameBySharedParameterName(commandData));
                    Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(Name);
                    ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;
                    Guid guid = externalDefinition.GUID;
                    InstanceBinding newIB = app.Create.NewInstanceBinding(catSet);
                    doc.ParameterBindings.Insert(externalDefinition, newIB, builtInParameterGroup);
                    //SharedParameterElement sp = SharedParameterElement.Lookup(doc, guid);
                    // InternalDefinition def = sp.GetDefinition();
                    // def.SetAllowVaryBetweenGroups(doc, true);
                    t.Commit();
                }

            }
            catch (Exception e)
            {
                TaskDialog.Show("Warning", e.ToString());
            }


            #endregion
            return "ok";
        }

        public string DeleteSharedParameterFromFamily(ExternalCommandData commandData, Document doc)
        {
            string str = "";

            FamilyManager familyManager = doc.FamilyManager;
            FamilyType familyType = familyManager.CurrentType;
            FamilyTypeSet types = familyManager.Types;

            #region check if family has no type
            if (familyType == null)
            {
                using (Transaction t = new Transaction(doc, "change"))
                {
                    t.Start();
                    familyType = familyManager.NewType("Тип 1");
                    familyManager.CurrentType = familyType;
                    t.Commit();
                }
            }
            #endregion

            FamilyParameterSet parametersList = familyManager.Parameters;

            #region clear family from parameter 
            try
            {
                commandData.Application.Application.SharedParametersFilename = FOP.PathToFOP;
                using (Transaction t = new Transaction(doc, "Clear"))
                {
                    t.Start();

                    try
                    {
                        DefinitionFile sharedParametersFile = commandData.Application.Application.OpenSharedParameterFile();
                        DefinitionGroup sharedParametersGroup = sharedParametersFile.Groups.get_Item(GroupNameBySharedParameterName(commandData));
                        Definition sharedParameterDefinition = sharedParametersGroup.Definitions.get_Item(Name);
                        ExternalDefinition externalDefinition = sharedParameterDefinition as ExternalDefinition;

                        var p = familyManager.get_Parameter(externalDefinition.GUID);
                        familyManager.RemoveParameter(p);

                        str = "- " + Name + " был успешно удален из семейства " + doc.Title + ".rfa";
                    }
                    catch
                    {
                        str = "! " + Name + " отсутсвует в семействе " + doc.Title + ".rfa";
                    }

                    t.Commit();
                }
            }
            catch (Exception e)
            {
                TaskDialog.Show("1", e.ToString());
            }

            #endregion

            //str += CM.SaveAndCloseDocSimple(doc);

            return str;
        }


    }


}
