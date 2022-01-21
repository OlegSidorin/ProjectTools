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
        private static ExternalCommandData _cachedCmdData;

        Document doc { get; set; }

        public Result Execute(ExternalCommandData cmdData, ref string message, ElementSet elements)
        {
            _cachedCmdData = cmdData;

            Application app = cmdData.Application.Application;
            UIDocument uidoc = cmdData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            string family_name_hole_walls_round = "М1_ОтверстиеКруглое_Стена";
            string family_name_hole_walls_square = "М1_ОтверстиеПрямоугольное_Стена";
            string family_name_hole_floor_round = "М1_ОтверстиеКруглое_Перекрытие";
            string family_name_hole_floor_square = "М1_ОтверстиеПрямоугольное_Перекрытие";

            string extension = ".rfa";

            string directory = Main.DllFolderLocation + @"\res\";

            string path_hole_walls_round = directory + family_name_hole_walls_round + extension;
            string path_hole_walls_square = directory + family_name_hole_walls_square + extension;
            string path_hole_floor_round = directory + family_name_hole_floor_round + extension;
            string path_hole_floor_square = directory + family_name_hole_floor_square + extension;

            LoadFamily(doc, family_name_hole_walls_round, path_hole_walls_round);
            LoadFamily(doc, family_name_hole_walls_square, path_hole_walls_square);
            LoadFamily(doc, family_name_hole_floor_round, path_hole_floor_round);
            LoadFamily(doc, family_name_hole_floor_square, path_hole_floor_square);

            var rvtLinksList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>().ToList();

            Selection sel = uidoc.Selection;

            Reference  pickedRef = sel.PickObject(ObjectType.Element, new RevitLinkSelectionFilter(doc));

            List<Wall> walls = new List<Wall>();

            if (pickedRef != null)
            {
                var revitLinkDoc = doc.GetElement(pickedRef.ElementId) as RevitLinkInstance;
                walls = new FilteredElementCollector(revitLinkDoc.GetLinkDocument()).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().Cast<Wall>().ToList();
                foreach(Wall wall in walls)
                {
                    
                }
                //MessageBox.Show(walls.Count.ToString());
            }

            var ducts = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_DuctCurves).WhereElementIsNotElementType().Cast<Duct>().ToList();

            List<Duct> roundedDucts = new List<Duct>();
            List<Duct> rectangularDucts = new List<Duct>();

            foreach (Duct mp in ducts)
            {
                ConnectorManager cm = mp.ConnectorManager;
                foreach (Connector c in cm.Connectors)
                {
                    if (c.Shape == ConnectorProfileType.Round)
                    {
                        roundedDucts.Add(mp);
                    }
                    if (c.Shape == ConnectorProfileType.Rectangular)
                    {
                        rectangularDucts.Add(mp);
                    }
                    break;
                }
            }

            MessageBox.Show("rounded: " + roundedDucts.Count.ToString() + ",\n" + "rect: " + rectangularDucts.Count.ToString());

            var family_hole_walls_round = new FilteredElementCollector(doc).OfClass(typeof(Family)).Where(x => x.Name == family_name_hole_walls_round).Cast<Family>().ToList().First();
            var familySymbol_hole_walls_round = doc.GetElement(family_hole_walls_round.GetFamilySymbolIds().First()) as FamilySymbol;

            var family_hole_walls_square = new FilteredElementCollector(doc).OfClass(typeof(Family)).Where(x => x.Name == family_name_hole_walls_square).Cast<Family>().ToList().First();
            var familySymbol_hole_walls_square = doc.GetElement(family_hole_walls_square.GetFamilySymbolIds().First()) as FamilySymbol;

            List<XYZ> intersections = new List<XYZ>();
            foreach (Duct duct in roundedDucts)
            {
                foreach (Wall wall in walls)
                {
                    Curve ductCurve = FindDuctCurve(duct, out XYZ vector, out Line line);

                    XYZ intersection = null;

                    List<Face> wallFaces = FindWallFace(wall, out double alfa, out XYZ myXYZ, out XYZ rightXYZ, out string oEdges);

                    foreach (Face face in wallFaces)
                    {
                        intersection = FindFaceCurve(ductCurve, line, face);
                        if (null != intersection)
                        {
                            FamilyInstance familyInstance = null;
                            using (Transaction tr = new Transaction(doc, " we "))
                            {
                                tr.Start();
                                if (!familySymbol_hole_walls_round.IsActive) familySymbol_hole_walls_round.Activate();
                                familyInstance = doc.Create.NewFamilyInstance(intersection, familySymbol_hole_walls_round, (Level)doc.GetElement(duct.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                XYZ point1 = new XYZ(intersection.X, intersection.Y, 0);
                                XYZ point2 = new XYZ(intersection.X, intersection.Y, 10);
                                Line axis = Line.CreateBound(point1, point2);

                                ElementTransformUtils.RotateElement(doc, familyInstance.Id, axis, vector.AngleTo(XYZ.BasisY));
                                
                                tr.Commit();
                            }

                            FamilyManager fm = doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                            var familyParameters = fm.GetParameters();
                            using (Transaction tr = new Transaction(doc, "asa1"))
                            {
                                tr.Start();
                                familyInstance.LookupParameter("ADSK_Размер_Диаметр").Set(duct.Diameter * 1.2);
                                familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width * 1.2);
                                familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width * 1.2);
                                familyInstance.LookupParameter("Data").Set((alfa * 180 / Math.PI).ToString() + " : " + $"({myXYZ.X * 304.8}, {myXYZ.Y * 304.8})" + " : " + $"({rightXYZ.X * 304.8}, {rightXYZ.Y * 304.8})\n:::\n" + oEdges);
                                tr.Commit();
                            }

                            intersections.Add(intersection);
                        }

                    }
                }
            }

            #region rectangular
            //foreach (Duct duct in rectangularDucts)
            //{
            //    foreach (Wall wall in walls)
            //    {
            //        Curve ductCurve = FindDuctCurve(duct, out XYZ vector, out Line line);

            //        XYZ intersection = null;

            //        List<Face> wallFaces = FindWallFace(wall);

            //        foreach (Face face in wallFaces)
            //        {
            //            intersection = FindFaceCurve(ductCurve, line, face);
            //            if (null != intersection)
            //            {
            //                FamilyInstance familyInstance = null;
            //                using (Transaction tr = new Transaction(doc, " we2 "))
            //                {
            //                    tr.Start();
            //                    if (!familySymbol_hole_walls_square.IsActive) familySymbol_hole_walls_square.Activate();
            //                    familyInstance = doc.Create.NewFamilyInstance(intersection, familySymbol_hole_walls_square, duct.ReferenceLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            //                    XYZ point1 = new XYZ(intersection.X, intersection.Y, 0);
            //                    XYZ point2 = new XYZ(intersection.X, intersection.Y, 10);
            //                    Line axis = Line.CreateBound(point1, point2);

            //                    ElementTransformUtils.RotateElement(doc, familyInstance.Id, axis, vector.AngleTo(XYZ.BasisY));
            //                    tr.Commit();
            //                }

            //                FamilyManager fm = doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
            //                var familyParameters = fm.GetParameters();
            //                using (Transaction tr = new Transaction(doc, "asa2"))
            //                {
            //                    tr.Start();
            //                    familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(duct.Width * 1.2);
            //                    familyInstance.LookupParameter("ADSK_Размер_Высота").Set(duct.Height * 1.2);
            //                    familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width * 1.2);
            //                    tr.Commit();
            //                }

            //                intersections.Add(intersection);
            //            }

            //        }
            //    }
            //}
            #endregion

            

            string output = "";
            //foreach (XYZ xyz in intersections)
            //{
            //    output += xyz.ToString() + Environment.NewLine;
            //}
            //MessageBox.Show(output);
            return Result.Succeeded;
        }
        private void LoadFamily(Document doc, string name, string path)
        {
            var families = new FilteredElementCollector(doc).OfClass(typeof(Family));

            Family family = families.FirstOrDefault<Element>(e => e.Name.Equals(name)) as Family;

            if (null == family)
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show("Нет семейства \n" + name);
                }

                using (Transaction tx = new Transaction(doc, "Load Family"))
                {
                    tx.Start();
                    doc.LoadFamily(path, out family);
                    tx.Commit();
                }
            }
        }
        public Curve FindDuctCurve(Duct duct, out XYZ vector, out Line line)
        {
            //The wind pipe curve
            IList<XYZ> list = new List<XYZ>();
            ConnectorSetIterator csi = duct.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                list.Add(conn.Origin);
            }

            if (list.ElementAt(0).X < list.ElementAt(1).X) line = Line.CreateBound(list.ElementAt(0), list.ElementAt(1));
            else line = Line.CreateBound(list.ElementAt(1), list.ElementAt(0));

            Curve curve = Line.CreateBound(list.ElementAt(0), list.ElementAt(1)) as Curve;

            if (list.ElementAt(0).X < list.ElementAt(1).X) vector = (list.ElementAt(0) - list.ElementAt(1)).Normalize();
            else  vector = (list.ElementAt(1) - list.ElementAt(0)).Normalize();

            curve.MakeUnbound();

            return curve;
        }
        public List<Face> FindWallFace(Wall wall, out double alfa, out XYZ myXYZ, out XYZ locCurveRight, out string oEdges)
        {
            XYZ faceCenter = XYZ.Zero;

            myXYZ = new XYZ(-1000000000, -1000000000, 1);

            //face1 = XYZ.Zero; 
            List<Face> normalFaces = new List<Face>();
            List<Face> myFaces = new List<Face>();

            LocationCurve locCurve = wall.Location as LocationCurve;

            XYZ start;
            XYZ end;

            if (locCurve.Curve.GetEndPoint(0).X <= locCurve.Curve.GetEndPoint(1).X)
            {
                start = locCurve.Curve.GetEndPoint(0);
                end = locCurve.Curve.GetEndPoint(1);
            }
            else
            {
                start = locCurve.Curve.GetEndPoint(1);
                end = locCurve.Curve.GetEndPoint(0);
            }

            XYZ locCurveCenter = (start + end) / 2;
            locCurveRight = end;

            if (end.X - start.X < Math.Pow(2, -40))
            {
                alfa = Math.PI / 2;
            }
            else
            {
                alfa = Math.Atan((end.Y - start.Y) / (end.X - start.X));
            }

            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;

            GeometryElement e = wall.get_Geometry(opt);

            Face myFace = null;
            Edge myEdge = null;
            oEdges = "";
            foreach (GeometryObject obj in e)
            {
                Solid solid = obj as Solid;
                if (solid != null && solid.Faces.Size > 0)
                {
                    foreach (Face face in solid.Faces)
                    {
                        PlanarFace pf = face as PlanarFace;
                        if (pf != null)
                        {
                            normalFaces.Add(pf);
                        }
                    }

                    myFace = solid.Edges.get_Item(0).GetFace(0);

                    //myEdge = solid.Edges.get_Item(0);

                    List<Edge> myEdges = new List<Edge>();

                    foreach (Edge edge in solid.Edges)
                    {
                        myEdges.Add(edge);
                        oEdges += $"{edge.AsCurve().GetEndPoint(0).X * 304.8}\t{edge.AsCurve().GetEndPoint(0).Y * 304.8}\t{edge.AsCurve().GetEndPoint(0).Z * 304.8}\t{edge.AsCurve().GetEndPoint(1).X * 304.8}\t{edge.AsCurve().GetEndPoint(1).Y * 304.8}\t{edge.AsCurve().GetEndPoint(1).Z * 304.8}\n";

                        //if (Math.Abs(edge.AsCurve().GetEndPoint(0).X - edge.AsCurve().GetEndPoint(1).X) < 0.0000001)
                        //{
                        //    if (Math.Abs(edge.AsCurve().GetEndPoint(0).Y - edge.AsCurve().GetEndPoint(1).Y) < 0.0000001)
                        //    {
                        //        myEdges.Add(edge);
                        //        oEdges += $"{edge.AsCurve().GetEndPoint(0)}:{edge.AsCurve().GetEndPoint(1)}\n";
                        //    }
                        //}
                        
                    }

                    myEdge = solid.Edges.get_Item(0);

                    if (alfa + 0.001 > 0)
                    {
                        foreach (Edge ed in myEdges)
                        {
                            if (ed.AsCurve().GetEndPoint(0).Y > myEdge.AsCurve().GetEndPoint(0).Y)
                            {
                                myEdge = ed;
                            }
                        }
                    }
                    else
                    {
                        foreach (Edge ed in myEdges)
                        {
                            if (ed.AsCurve().GetEndPoint(0).Y < myEdge.AsCurve().GetEndPoint(0).Y)
                            {
                                myEdge = ed;
                            }
                        }
                    }
                    
                        



                    if (myEdge.GetFace(0).Area >= myFace.Area - 0.001)
                    {
                        PlanarFace pf = myEdge.GetFace(0) as PlanarFace;
                        if (pf != null)
                        {
                            myFace = pf;
                            myXYZ = new XYZ(myEdge.AsCurve().GetEndPoint(0).X, myEdge.AsCurve().GetEndPoint(0).Y, 0);
                        }
                    }
                    if (myEdge.GetFace(1).Area >= myFace.Area - 0.001)
                    {
                        PlanarFace pf = myEdge.GetFace(1) as PlanarFace;
                        if (pf != null)
                        {
                            myFace = pf;
                            myXYZ = new XYZ(myEdge.AsCurve().GetEndPoint(0).X, myEdge.AsCurve().GetEndPoint(0).Y, 0);
                        }
                    }



                }
            }


            //List<Face> specFaces = new List<Face>();
            //Face mostAreaFace = null;

            //if (normalFaces.Count > 2)
            //{
            //    if (alfa >= 0)
            //    {
            //        foreach (var face in normalFaces)
            //        {
            //            var curveLoops = face.GetEdgesAsCurveLoops();
            //            foreach(var curveLoop in curveLoops)
            //            {
            //                List<XYZ> pts = new List<XYZ>();
            //                foreach (Curve c in curveLoop)
            //                {
            //                    pts.AddRange(c.Tessellate());
            //                }
            //                BoundingBoxXYZ bb = new BoundingBoxXYZ();
            //                bb.set_Bounds()
            //            }
                        //faceCenter = GetCenterOfFace(face);
                        //face = GetRightXYZ(face);
                        //if (faceCenter.Y - locCurveCenter.Y > Math.Pow(2, -40))
                        //{
                        //    specFaces.Add(face);
                        //}
                        //if (specFaces.Count != 0) mostAreaFace = specFaces.FirstOrDefault();
                        //foreach (Face faceSpec in specFaces)
                        //{
                        //    if (faceSpec.Area >= mostAreaFace.Area)
                        //    {
                        //        xyz = XYZ.Zero;
                        //        mostAreaFace = faceSpec;
                        //    }
                        //}
            //        }
            //    }
            //    else
            //    {
            //        foreach (var face in normalFaces)
            //        {
            //            //faceCenter = GetCenterOfFace(face);
            //            //face = GetRightXYZ(face);
            //            //if (faceCenter.Y - locCurveCenter.Y < Math.Pow(2, -40))
            //            //{
            //            //    specFaces.Add(face);
            //            //}
            //            //if (specFaces.Count != 0) mostAreaFace = specFaces.FirstOrDefault();
            //            //foreach (Face faceSpec in specFaces)
            //            //{
            //            //    if (faceSpec.Area >= mostAreaFace.Area)
            //            //    {
            //            //        xyz = XYZ.Zero;
            //            //        mostAreaFace = faceSpec;
            //            //    }
            //            //}
            //        }
            //    }

            //}

            //if (mostAreaFace != null)
            //{

            //}

            normalFaces.Clear();
            normalFaces.Add(myFace);

            //foreach (Face f in specFaces)
            //{
            //    normalFaces.Add(f);
            //}


            return normalFaces;
        }
        public XYZ FindFaceCurve(Curve DuctCurve, Line line, Face WallFace)
        {
            //The intersection point
            IntersectionResultArray intersectionR = new IntersectionResultArray();//Intersection point set

            SetComparisonResult results;//Results of Comparison

            results = WallFace.Intersect(DuctCurve, out intersectionR);

            XYZ intersectionResult = null;//Intersection coordinate

            if (SetComparisonResult.Disjoint != results)
            {
                if (intersectionR != null)
                {
                    if (!intersectionR.IsEmpty)
                    {
                        if (line.Contains(intersectionR.get_Item(0).XYZPoint))
                        {
                            intersectionResult = intersectionR.get_Item(0).XYZPoint;
                        }
                    }
                }
            }
            return intersectionResult;
        }

        public XYZ GetCenterOfFace(Face face)
        {
            double CurvePoints_Umin = double.MaxValue;
            double CurvePoints_Umax = double.MinValue;
            double CurvePoints_Vmin = double.MaxValue;
            double CurvePoints_Vmax = double.MinValue;

            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach(Edge edge in edgeArray)
                {
                    foreach(UV uv in edge.TessellateOnFace(face))
                    {
                        CurvePoints_Umin = Math.Min(CurvePoints_Umin, uv.U);
                        CurvePoints_Umax = Math.Max(CurvePoints_Umax, uv.U);
                        CurvePoints_Vmin = Math.Min(CurvePoints_Vmin, uv.V);
                        CurvePoints_Vmax = Math.Max(CurvePoints_Vmax, uv.V);
                    }
                }
            }

            UV uvCenter = new UV(CurvePoints_Umax - CurvePoints_Umin, CurvePoints_Vmax - CurvePoints_Vmin);

            return face.Evaluate(uvCenter);
        }

        public XYZ GetRightXYZ(Face face)
        {
            XYZ xyz = new XYZ(double.MinValue, double.MinValue, double.MinValue);

            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    if (edge.AsCurve().GetEndPoint(0).X > xyz.X) xyz = new XYZ(edge.AsCurve().GetEndPoint(0).X, edge.AsCurve().GetEndPoint(0).Y, edge.AsCurve().GetEndPoint(0).Z);
                    if (edge.AsCurve().GetEndPoint(1).X > xyz.X) xyz = new XYZ(edge.AsCurve().GetEndPoint(1).X, edge.AsCurve().GetEndPoint(1).Y, edge.AsCurve().GetEndPoint(1).Z);
                }
            }


            return xyz;
        }

    }

    public static class LineExtension
    {
        static readonly double EPSILON = Math.Pow(2, -52);
        public static bool Contains(this Line line, XYZ point)
        {
            XYZ a = line.GetEndPoint(0); // Line start point
            XYZ b = line.GetEndPoint(1); // Line end point
            XYZ p = point;
            return (Math.Abs(a.DistanceTo(b) - (a.DistanceTo(p) + p.DistanceTo(b))) < EPSILON * 1000);
        }

    }

    public class RevitLinkSelectionFilter : ISelectionFilter
    {
        Document doc = null;

        public RevitLinkSelectionFilter(Document document)
        {
            doc = document;
        }

        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_RvtLinks)
            {
                return true;
            }
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
            //RevitLinkInstance revitlinkinstance = doc.GetElement(reference) as RevitLinkInstance;
            //Autodesk.Revit.DB.Document docLink = revitlinkinstance.GetLinkDocument();
            //Element eRoomLink = docLink.GetElement(reference.LinkedElementId);
            //if (eRoomLink.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Rooms)
            //{
            //    return true;
            //}
            //return false;
        }
    }
}
