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

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class Command11 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            var wallTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType().ToList();
            var floorTypes = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsElementType().ToList();


            ElementId eiM1_Material = new ElementId(24);
            List<ElementId> eisWhatNeedToChange = new List<ElementId>();

            string matName = Interaction.InputBox("Содержится в названии материала, который нужно заменить:", "Замена материала стен", "Кирпич");

            var materials = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).ToList();

            bool isMaterialInProject = false;
            string listOfMaterials = "";
            foreach (Material m in materials)
            {
                if (m.Name.ToString() == "M1_Material")
                {
                    isMaterialInProject = true;
                }
                if (m.Name.ToString().Contains(matName))
                {
                    eisWhatNeedToChange.Add(m.Id);
                    listOfMaterials += $"{m.Name}, ";
                }
            }
            if (listOfMaterials.Count() > 2) listOfMaterials = listOfMaterials.Substring(0,listOfMaterials.Length - 2);
            if (listOfMaterials.Length > 256) listOfMaterials = listOfMaterials.Substring(0, 255) + "... ";

            if (!isMaterialInProject)
            {
                using (var transaction = new Transaction(doc))
                {
                    transaction.Start("CreateMaterial");

                    // Create new material
                    ElementId newMaterial = Material.Create(doc, "M1_Material");
                    Material material = doc.GetElement(newMaterial) as Material;
                    Autodesk.Revit.DB.Color color = new Color(200, 250, 200);
                    material.Color = color;

                    transaction.Commit();
                }
            }

            materials = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Materials).ToList();
            //MessageBox.Show($"walltypes {wallTypes.Count}\nfloortypes {floorTypes.Count}\nmaterials {materials.Count}");

            foreach (Material m in materials)
            {
                if (m.Name.ToString() == "M1_Material")
                {
                    eiM1_Material = m.Id;
                    //MessageBox.Show($"material: {m.Name}\nid: {m.Id}");
                }
            }
            

            const string caption = "Замена материалов";
            var result = MessageBox.Show($"Заменить эти материалы:\n{listOfMaterials}?", caption,
                                 System.Windows.Forms.MessageBoxButtons.YesNo,
                                 System.Windows.Forms.MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (var element in wallTypes)
                {
                    try
                    {
                        WallType wallType = element as WallType;

                        if (wallType != null)
                        {
                            var structure = wallType.GetCompoundStructure();

                            if (structure != null)
                            {
                                if (structure.LayerCount != 0)
                                {
                                    var ls = structure.GetLayers();

                                    if (ls.Count != 0)
                                    {
                                        using (Transaction t = new Transaction(doc, "Wall Material Change"))
                                        {
                                            t.Start();

                                            List<CompoundStructureLayer> layers = new List<CompoundStructureLayer>();
                                            foreach (CompoundStructureLayer sl in ls)
                                            {
                                                bool flag = false;
                                                foreach (ElementId ei in eisWhatNeedToChange)
                                                {
                                                    if (ei.IntegerValue == sl.MaterialId.IntegerValue) flag = true;
                                                }

                                                if (flag)
                                                {
                                                    CompoundStructureLayer newLayer = new CompoundStructureLayer(sl.Width, sl.Function, eiM1_Material);
                                                    layers.Add(newLayer);
                                                }
                                                else
                                                {
                                                    CompoundStructureLayer newLayer = new CompoundStructureLayer(sl.Width, sl.Function, sl.MaterialId);
                                                    layers.Add(newLayer);
                                                }
                                            }
                                            try
                                            {
                                                structure.SetLayers(layers);
                                                wallType.SetCompoundStructure(structure);
                                            }
                                            catch { };

                                            t.Commit();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch { };
                }
            }
            else 
            {
                
            };

            return Result.Succeeded;
        }
    }
}
