using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Application = Autodesk.Revit.ApplicationServices.Application;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using Autodesk.Revit.DB.Mechanical;
using System.IO;
using Autodesk.Revit.DB.Plumbing;
using Point = Autodesk.Revit.DB.Point;
using ProjectTools;
using Microsoft.WindowsAPICodePack.Dialogs;
using Autodesk.Revit.DB.Electrical;

namespace ProjectTools
{
    public class Command13ViewModel : INotifyPropertyChanged
    {

        #region private vars
        private CreateVoidsEventHandler _createVoidsEventHandler;
        private ExternalEvent _externalEvent;
        private ExternalCommandData _commandData;
        private string _wallGap;
        private string _floorGap;
        private string _wallIndent;
        private string _floorIndent;
        private ICommand _createVoidsCommand;
        private bool _roundedDucts;
        private bool _rectangDucts;
        private bool _roundedPipes;
        private bool _rectangCableTray;
        private bool _inWalls;
        private bool _inFloors;
        private string _minDiameter;
        private string _maxDiameter;
        private string _maxStopDiameter;
        private string _startAngle;
        private string _statusik;

        private string _family_name_hole_walls_round;
        private string _family_name_hole_walls_rectang;
        private string _family_name_hole_floors_round;
        private string _family_name_hole_floors_rectang;
        private string _path_hole_walls_round;
        private string _path_hole_walls_rectang;
        private string _path_hole_floors_round;
        private string _path_hole_floors_rectang;


        #endregion

        #region public vars
        public CreateVoidsEventHandler CreateVoidsEventHandler
        {
            get { return _createVoidsEventHandler; }
            set
            {
                if (value == _createVoidsEventHandler) return;
                _createVoidsEventHandler = value;
                OnPropertyChanged();
            }
        }
        public ExternalEvent CreateVoidsExternalEvent
        {
            get { return _externalEvent; }
            set
            {
                if (value == _externalEvent) return;
                _externalEvent = value;
                OnPropertyChanged();
            }
        }
        public ExternalCommandData CommandData
        {
            get { return _commandData; }
            set
            {
                if (value == _commandData) return;
                _commandData = value;
                OnPropertyChanged();
            }
        }
        public string WallGap
        {
            get { return _wallGap; }
            set
            {
                if (value == _wallGap) return;
                _wallGap = value;
                OnPropertyChanged();
            }
        }
        public string FloorGap
        {
            get { return _floorGap; }
            set
            {
                if (value == _floorGap) return;
                _floorGap = value;
                OnPropertyChanged();
            }
        }
        public string WallIndent
        {
            get { return _wallIndent; }
            set
            {
                if (value == _wallIndent) return;
                _wallIndent = value;
                OnPropertyChanged();
            }
        }
        public string FloorIndent
        {
            get { return _floorIndent; }
            set
            {
                if (value == _floorIndent) return;
                _floorIndent = value;
                OnPropertyChanged();
            }
        }
        public string MinDiameter
        {
            get { return _minDiameter; }
            set
            {
                if (value == _minDiameter) return;
                _minDiameter = value;
                OnPropertyChanged();
            }
        }
        public string MaxDiameter
        {
            get { return _maxDiameter; }
            set
            {
                if (value == _maxDiameter) return;
                _maxDiameter = value;
                OnPropertyChanged();
            }
        }
        public string MaxStopDiameter
        {
            get { return _maxStopDiameter; }
            set
            {
                if (value == _maxStopDiameter) return;
                _maxStopDiameter = value;
                OnPropertyChanged();
            }
        }
        public string StartAngle
        {
            get { return _startAngle; }
            set
            {
                if (value == _startAngle) return;
                _startAngle = value;
                OnPropertyChanged();
            }
        }
        public ICommand CreateVoidsCommand
        {
            get
            {
                if (_createVoidsCommand == null)
                {
                    _createVoidsCommand = new RelayCommand(CreateVoids);
                }

                return _createVoidsCommand;
            }
        }

        public bool RoundedDucts
        {
            get { return _roundedDucts; }
            set
            {
                if (value == _roundedDucts) return;
                _roundedDucts = value;
                OnPropertyChanged();
            }
        }
        public bool RectangDucts
        {
            get { return _rectangDucts; }
            set
            {
                if (value == _rectangDucts) return;
                _rectangDucts = value;
                OnPropertyChanged();
            }
        }
        public bool RoundedPipes
        {
            get { return _roundedPipes; }
            set
            {
                if (value == _roundedPipes) return;
                _roundedPipes = value;
                OnPropertyChanged();
            }
        }
        public bool RectangCableTray
        {
            get { return _rectangCableTray; }
            set
            {
                if (value == _rectangCableTray) return;
                _rectangCableTray = value;
                OnPropertyChanged();
            }
        }
        public bool InWalls
        {
            get { return _inWalls; }
            set
            {
                if (value == _inWalls) return;
                _inWalls = value;
                OnPropertyChanged();
            }
        }
        public bool InFloors
        {
            get { return _inFloors; }
            set
            {
                if (value == _inFloors) return;
                _inFloors = value;
                OnPropertyChanged();
            }
        }
        public string Statusik
        {
            get { return _statusik; }
            set
            {
                if (value == _statusik) return;
                _statusik = value;
                OnPropertyChanged();
            }
        }

        public string family_name_hole_walls_round
        {
            get { return _family_name_hole_walls_round; }
            set
            {
                if (value == _family_name_hole_walls_round) return;
                _family_name_hole_walls_round = value;
                OnPropertyChanged();
            }
        }
        public string family_name_hole_walls_rectang
        {
            get { return _family_name_hole_walls_rectang; }
            set
            {
                if (value == _family_name_hole_walls_rectang) return;
                _family_name_hole_walls_rectang = value;
                OnPropertyChanged();
            }
        }
        public string family_name_hole_floors_round
        {
            get { return _family_name_hole_floors_round; }
            set
            {
                if (value == _family_name_hole_floors_round) return;
                _family_name_hole_floors_round = value;
                OnPropertyChanged();
            }
        }
        public string family_name_hole_floors_rectang
        {
            get { return _family_name_hole_floors_rectang; }
            set
            {
                if (value == _family_name_hole_floors_rectang) return;
                _family_name_hole_floors_rectang = value;
                OnPropertyChanged();
            }
        }
        public string path_hole_walls_round
        {
            get { return _path_hole_walls_round; }
            set
            {
                if (value == _path_hole_walls_round) return;
                _path_hole_walls_round = value;
                OnPropertyChanged();
            }
        }
        public string path_hole_walls_rectang
        {
            get { return _path_hole_walls_rectang; }
            set
            {
                if (value == _path_hole_walls_rectang) return;
                _path_hole_walls_rectang = value;
                OnPropertyChanged();
            }
        }
        public string path_hole_floors_round
        {
            get { return _path_hole_floors_round; }
            set
            {
                if (value == _path_hole_floors_round) return;
                _path_hole_floors_round = value;
                OnPropertyChanged();
            }
        }
        public string path_hole_floors_rectang
        {
            get { return _path_hole_floors_rectang; }
            set
            {
                if (value == _path_hole_floors_rectang) return;
                _path_hole_floors_rectang = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructor
        public Command13ViewModel()
        {
            InFloors = true;
            InWalls = true;
            RoundedPipes = true;
            RoundedDucts = true;
            RectangDucts = true;
            RectangCableTray = true;

            WallIndent = 0.ToString();
            WallGap = 50.ToString();
            FloorGap = 50.ToString();
            FloorIndent = 0.ToString();
            MinDiameter = 80.ToString();
            MaxDiameter = 300.ToString();
            StartAngle = 0.ToString();
            MaxStopDiameter = 1000.ToString();

            family_name_hole_walls_round = @"М1_ОтверстиеКруглое_Стена";
            family_name_hole_walls_rectang = @"М1_ОтверстиеПрямоугольное_Стена";
            family_name_hole_floors_round = @"М1_ОтверстиеКруглое_Перекрытие";
            family_name_hole_floors_rectang = @"М1_ОтверстиеПрямоугольное_Перекрытие";

            string extension = ".rfa";

            string directory = Main.DllFolderLocation + @"\res\";

            path_hole_walls_round = directory + family_name_hole_walls_round + extension;
            path_hole_walls_rectang = directory + family_name_hole_walls_rectang + extension;
            path_hole_floors_round = directory + family_name_hole_floors_round + extension;
            path_hole_floors_rectang = directory + family_name_hole_floors_rectang + extension;

        }
        #endregion

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void CreateVoids(object obj)
        {
            throw new NotImplementedException();
        }

        private ICommand chooseFamily;

        public ICommand ChooseFamily
        {
            get
            {
                if (chooseFamily == null)
                {
                    chooseFamily = new RelayCommand(PerformChooseFamily);
                }

                return chooseFamily;
            }
        }

        private void PerformChooseFamily(object commandParameter)
        {
            string path = string.Empty;
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = false;
                dialog.Multiselect = false;
                dialog.DefaultDirectory = @"C:\Users\" + Environment.UserName + @"\Downloads";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    path = dialog.FileName;
                    
                    if (File.Exists(path) && path.Contains(".rfa"))
                    {
                        if ((string)commandParameter == "RectangWall")
                        {
                            string fileName = Path.GetFileName(path);
                            path_hole_walls_rectang = path;
                            family_name_hole_walls_rectang = fileName.Replace(".rfa", "");
                            //MessageBox.Show((string)commandParameter + "\n" + path + "\n" + fileName.Replace(".rfa", ""));
                        }
                        if ((string)commandParameter == "RoundWall")
                        {
                            string fileName = Path.GetFileName(path);
                            path_hole_walls_round = path;
                            family_name_hole_walls_round = fileName.Replace(".rfa", "");
                            //MessageBox.Show((string)commandParameter + "\n" + path + "\n" + fileName.Replace(".rfa", ""));
                        }
                        if ((string)commandParameter == "RectangFloor")
                        {
                            string fileName = Path.GetFileName(path);
                            path_hole_floors_rectang = path;
                            family_name_hole_floors_rectang = fileName.Replace(".rfa", "");
                            //MessageBox.Show((string)commandParameter + "\n" + path + "\n" + fileName.Replace(".rfa", ""));
                        }
                        if ((string)commandParameter == "RoundFloor")
                        {
                            string fileName = Path.GetFileName(path);
                            path_hole_floors_round = path;
                            family_name_hole_floors_round = fileName.Replace(".rfa", "");
                            //MessageBox.Show((string)commandParameter + "\n" + path + "\n" + fileName.Replace(".rfa", ""));
                        }

                    }
                }
            }
        }
    }

    public class CreateVoidsEventHandler : IExternalEventHandler
    {
        #region public vars
        public ExternalCommandData CommandData { get; set; }
        public double WallGap { get; set; }
        public double FloorGap { get; set; }
        public double WallIndent { get; set; }
        public double FloorIndent { get; set; }
        public double MinDiameter { get; set; }
        public double MaxDiameter { get; set; }
        public double MaxStopDiameter { get; set; }
        public double StartAngle { get; set; }
        public Document Doc { get; set; }
        public Command13ViewModel ViewModel { get; set; }
        public Window Window { get; set; }

        public string family_name_hole_walls_round { get; set; }
        public string family_name_hole_walls_rectang { get; set; }
        public string family_name_hole_floors_round { get; set; }
        public string family_name_hole_floors_rectang { get; set; }
        public string path_hole_walls_round { get; set; }
        public string path_hole_walls_rectang { get; set; }
        public string path_hole_floors_round { get; set; }
        public string path_hole_floors_rectang { get; set; }


        #endregion
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument; //CommandData.Application.ActiveUIDocument;
            Doc = uidoc.Document;

            ViewModel.Statusik = "Привет! Отдельный привет бим бойцу Фариду! Начинаем..";

            //MessageBox.Show(family_name_hole_walls_round + path_hole_walls_round + Environment.NewLine);

            List<string> paramNames = new List<string>(new string[] { "M1_ForElements", "M1_ElementMask", "M1_SystemName", "M1_СreationDate" });

            ViewModel.Statusik = $"добавляем параметры в {family_name_hole_walls_round}";
            path_hole_walls_round = AddFamilyParametersInFamilyDocument(uiapp, path_hole_walls_round, paramNames);

            ViewModel.Statusik = $"добавляем параметры в {family_name_hole_walls_rectang}";
            path_hole_walls_rectang = AddFamilyParametersInFamilyDocument(uiapp, path_hole_walls_rectang, paramNames);

            ViewModel.Statusik = $"добавляем параметры в {family_name_hole_floors_round}";
            path_hole_floors_round = AddFamilyParametersInFamilyDocument(uiapp, path_hole_floors_round, paramNames);

            ViewModel.Statusik = $"добавляем параметры в {family_name_hole_floors_rectang}";
            path_hole_floors_rectang = AddFamilyParametersInFamilyDocument(uiapp, path_hole_floors_rectang, paramNames);
             
            ViewModel.Statusik = $"загрузка семейства {family_name_hole_walls_round}";
            Family family_hole_walls_round = LoadFamily(Doc, family_name_hole_walls_round, path_hole_walls_round);
            FamilySymbol familySymbol_hole_walls_round = null;
            if (family_hole_walls_round != null) { familySymbol_hole_walls_round = Doc.GetElement(family_hole_walls_round.GetFamilySymbolIds().First()) as FamilySymbol; }
            else
            {
#if DEBUG
                MessageBox.Show("семейство не загружено \n" + family_name_hole_walls_round);
#endif
            }

            ViewModel.Statusik = $"загрузка семейства {family_name_hole_walls_rectang}";
            Family family_hole_walls_rectang = LoadFamily(Doc, family_name_hole_walls_rectang, path_hole_walls_rectang);
            FamilySymbol familySymbol_hole_walls_rectang = null;
            if (family_hole_walls_rectang != null) { familySymbol_hole_walls_rectang = Doc.GetElement(family_hole_walls_rectang.GetFamilySymbolIds().First()) as FamilySymbol; }
            else
            {
#if DEBUG
                MessageBox.Show("семейство не загружено \n" + family_name_hole_walls_rectang);
#endif
            }

            ViewModel.Statusik = $"загрузка семейства {family_name_hole_floors_round}";
            Family family_hole_floors_round = LoadFamily(Doc, family_name_hole_floors_round, path_hole_floors_round);
            FamilySymbol familySymbol_hole_floors_round = null;
            if (family_hole_floors_round != null) { familySymbol_hole_floors_round = Doc.GetElement(family_hole_floors_round.GetFamilySymbolIds().First()) as FamilySymbol; }
            else
            {
#if DEBUG
                MessageBox.Show("семейство не загружено \n" + family_name_hole_floors_round);
#endif
            }

            ViewModel.Statusik = $"загрузка семейства {family_name_hole_floors_rectang}";
            Family family_hole_floors_rectang = LoadFamily(Doc, family_name_hole_floors_rectang, path_hole_floors_rectang);
            FamilySymbol familySymbol_hole_floors_rectang = null;
            if (family_hole_floors_rectang != null) { familySymbol_hole_floors_rectang = Doc.GetElement(family_hole_floors_rectang.GetFamilySymbolIds().First()) as FamilySymbol; }
            else
            {
#if DEBUG
                MessageBox.Show("семейство не загружено \n" + family_name_hole_floors_rectang);
#endif
            }

            //MessageBox.Show(Doc.IsModifiable.ToString() + Doc.IsReadOnly.ToString());


            var rvtLinksList = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>().ToList();

            ViewModel.Statusik = $"выбор связи ...";
            MessageBox.Show("Выберите связь АР или КР");
           
            Selection sel = uidoc.Selection;

            Reference pickedRef = sel.PickObject(ObjectType.Element, new RevitLinkSelectionFilter(Doc));

            List<Wall> walls = new List<Wall>();
            List<Floor> floors = new List<Floor>();

            if (pickedRef != null)
            {
                var revitLinkDoc = Doc.GetElement(pickedRef.ElementId) as RevitLinkInstance;
                ViewModel.Statusik = $"связь {revitLinkDoc.Name}";

                var durtyWalls = new FilteredElementCollector(revitLinkDoc.GetLinkDocument()).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements().ToList();
                var durtyFloors = new FilteredElementCollector(revitLinkDoc.GetLinkDocument()).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType().ToElements().ToList();
#if DEBUG 
                MessageBox.Show($"гр. стен: {durtyWalls.Count}\nгр. перекр.: {durtyFloors.Count}");
#endif
                foreach (var dwall in durtyWalls)
                {
                    try
                    {
                        Wall myWall = (Wall)dwall;
                        walls.Add(myWall);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        MessageBox.Show($"стена {dwall.Id} на кастомизируется");
#endif
                    }
                }

                foreach (var dfloor in durtyFloors)
                {
                    try
                    {
                        Floor myFloor = (Floor)dfloor;
                        floors.Add(myFloor);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        MessageBox.Show($"перекр. {dfloor.Id} на кастомизируется");
#endif
                    }
                }
#if DEBUG
                MessageBox.Show($"стен: {walls.Count}\nперекр.: {floors.Count}");
#endif
            }

            List<Duct> ducts = new List<Duct>();
            List<Pipe> pipes = new List<Pipe>();
            List<CableTray> cableTrays = new List<CableTray>();

            var dirtyDucts = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_DuctCurves).WhereElementIsNotElementType().ToElements().ToList();
            var dirtyPipes = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_PipeCurves).WhereElementIsNotElementType().ToElements().ToList();
            var dirtyCableTrays = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_CableTray).WhereElementIsNotElementType().ToElements().ToList();
#if DEBUG
            MessageBox.Show($"гр. возд.: {dirtyDucts.Count}\nгр. труб: {dirtyPipes.Count}\nгр. лотков: {dirtyCableTrays.Count}");
#endif
            foreach (var dduct in dirtyDucts)
            {
                try
                {
                    Duct myDuct = (Duct)dduct;
                    ducts.Add(myDuct);
                }
                catch (Exception ex)
                {
#if DEBUG
                    MessageBox.Show($"возд. {dduct.Id} не кастомизируется");
#endif
                }
            }

            foreach (var dpipe in dirtyPipes)
            {
                try
                {
                    Pipe myPipe = (Pipe)dpipe;
                    pipes.Add(myPipe);
                }
                catch (Exception ex)
                {
#if DEBUG
                    MessageBox.Show($"труба {dpipe.Id} не кастомизируется");
#endif
                }
            }
#if DEBUG
            MessageBox.Show($"возд.: {ducts.Count}\nтруб: {pipes.Count}");
#endif
            
            foreach (var dcabtray in dirtyCableTrays)
            {
                try
                {
                    CableTray myCableTray = (CableTray)dcabtray;
                    cableTrays.Add(myCableTray);
                }
                catch (Exception ex)
                {
#if DEBUG
                    MessageBox.Show($"лоток {dcabtray.Id} не кастомизируется");
#endif
                }
            }
#if DEBUG
            MessageBox.Show($"возд.: {ducts.Count}\nтруб: {pipes.Count}\nлотков: {cableTrays.Count}");
#endif

            List<Duct> roundedDucts = new List<Duct>();
            List<Duct> rectangularDucts = new List<Duct>();
            List<Pipe> roundedPipes = new List<Pipe>();
            List<CableTray> rectangularCableTrays = new List<CableTray>();

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
            foreach (Pipe p in pipes)
            {
                roundedPipes.Add(p);
            }
            foreach (CableTray ct in cableTrays)
            {
                rectangularCableTrays.Add(ct);
            }

            List<XYZ> intersections = new List<XYZ>();
            XYZ intersection = null;

            List<FamilyInstance> familyCollection;
            bool familyIsInPoint = false;

#if DEBUG
            MessageBox.Show($"мин диам {MinDiameter}, \nмакс диам {MaxDiameter},\nмакс stop диам {MaxStopDiameter}");
#endif

            #region round ducts walls

            if (ViewModel.RoundedDucts && ViewModel.InWalls)
            {
                familyCollection = new FilteredElementCollector(Doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Family.Name == family_name_hole_walls_round).ToList();
                //MessageBox.Show($"ro.du.{familyCollection.Count}");
                foreach (Duct duct in roundedDucts)
                {
                    foreach (Wall wall in walls)
                    {
                        ViewModel.Statusik = $"установка кругл. отверстий в стенах..";
                        Curve ductCurve = FindDuctCurve(duct, out XYZ vector, out Line line);

                        intersections.Clear();
                        intersection = null;

                        List<Face> wallFaces = FindWallFace(wall, out double alfa);

                        foreach (Face face in wallFaces)
                        {
                            intersection = FindFaceCurve(ductCurve, line, face);
                            if (intersection != null) intersections.Add(intersection);
                        }

                        XYZ middlePoint = XYZ.Zero;

                        familyIsInPoint = false;

                        if (intersections.Count == 2)
                        {
                            middlePoint = (intersections[0] + intersections[1]) / 2;

                            familyIsInPoint = false;

                            try
                            {
                                foreach (FamilyInstance _f in familyCollection)
                                {
                                    Options options = new Options();
                                    options.ComputeReferences = true;
                                    options.DetailLevel = ViewDetailLevel.Undefined;
                                    options.IncludeNonVisibleObjects = true;
                                    GeometryElement geometryElement = _f.get_Geometry(options);
                                    BoundingBoxXYZ bb = geometryElement.GetBoundingBox();
                                    XYZ centerBB = (bb.Min + bb.Max) / 2;
                                    if (Math.Abs(middlePoint.DistanceTo(centerBB)) < 0.000001) familyIsInPoint = true;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }

                            FamilyInstance familyInstance = null;
                            double kIncrease = 1;

                            if (!familyIsInPoint && duct.Diameter >= MinDiameter && duct.Diameter <= MaxDiameter)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_walls_round.IsActive) familySymbol_hole_walls_round.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(middlePoint, familySymbol_hole_walls_round, (Level)Doc.GetElement(duct.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);

                                        XYZ orientation = wall.Orientation;
                                        double angle = 0;
                                        //if (orientation.X == 0 && orientation.Y == 0) angle = 0;
                                        if (orientation.X <= 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X < 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);

                                        LocationCurve lcurve = (LocationCurve)duct.Location;
                                        Curve curve = lcurve.Curve;
                                        Line curveline = (Line)curve;
                                        double delta = Math.Sin(Math.Abs(Math.Abs(curveline.Direction.X) - Math.Abs(orientation.X)));
                                        if (delta > 0.00001) kIncrease = 1 + delta;

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); // vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Диаметр").Set(duct.Diameter * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(WallIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Отверстие_Отметка от этажа").Set(middlePoint.Z - ((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(duct.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(duct.Id.IntegerValue.ToString() + " in " + wall.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Round_Wall"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(duct.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }
                            else if (!familyIsInPoint && duct.Diameter >= MinDiameter && duct.Diameter > MaxDiameter)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_walls_rectang.IsActive) familySymbol_hole_walls_rectang.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(middlePoint, familySymbol_hole_walls_rectang, (Level)Doc.GetElement(duct.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);

                                        XYZ orientation = wall.Orientation;
                                        double angle = 0;
                                        //if (orientation.X == 0 && orientation.Y == 0) angle = 0;
                                        if (orientation.X <= 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X < 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);

                                        LocationCurve lcurve = (LocationCurve)duct.Location;
                                        Curve curve = lcurve.Curve;
                                        Line curveline = (Line)curve;
                                        double delta = Math.Sin(Math.Abs(Math.Abs(curveline.Direction.X) - Math.Abs(orientation.X)));
                                        if (delta > 0.00001) kIncrease = 1 + delta;

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); // vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(duct.Diameter * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Высота").Set(duct.Diameter * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(WallIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Отверстие_Отметка от этажа").Set(middlePoint.Z - ((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(duct.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(duct.Id.IntegerValue.ToString() + " in " + wall.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Rectangular_Wall"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(duct.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }

                        }
                    }
                }
                ViewModel.Statusik = $"установка кругл. отверстий в стенах завершена";
            }
            
            #endregion

            #region rectang ducts walls
            
            if (ViewModel.RectangDucts && ViewModel.InWalls)
            {
                familyCollection = new FilteredElementCollector(Doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Family.Name == family_name_hole_walls_rectang).ToList();
                //MessageBox.Show($"re.du.{familyCollection.Count}");
                foreach (Duct duct in rectangularDucts)
                {
                    foreach (Wall wall in walls)
                    {
                        ViewModel.Statusik = $"установка прям. отверстий в стенах..";
                        Curve ductCurve = FindDuctCurve(duct, out XYZ vector, out Line line);

                        intersections.Clear();
                        intersection = null;

                        List<Face> wallFaces = FindWallFace(wall, out double alfa);

                        foreach (Face face in wallFaces)
                        {
                            intersection = FindFaceCurve(ductCurve, line, face);
                            if (intersection != null) intersections.Add(intersection);
                        }

                        XYZ middlePoint = XYZ.Zero;

                        familyIsInPoint = false;

                        if (intersections.Count == 2)
                        {
                            middlePoint = (intersections[0] + intersections[1]) / 2;

                            try
                            {
                                foreach (FamilyInstance _f in familyCollection)
                                {
                                    Options options = new Options();
                                    options.ComputeReferences = true;
                                    options.DetailLevel = ViewDetailLevel.Undefined;
                                    options.IncludeNonVisibleObjects = true;
                                    GeometryElement geometryElement = _f.get_Geometry(options);
                                    BoundingBoxXYZ bb = geometryElement.GetBoundingBox();
                                    XYZ centerBB = (bb.Min + bb.Max) / 2;
                                    if (Math.Abs(middlePoint.DistanceTo(centerBB)) < 0.000001) familyIsInPoint = true;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }

                            FamilyInstance familyInstance = null;
                            double kIncrease = 1;
                            if (!familyIsInPoint)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_walls_rectang.IsActive) familySymbol_hole_walls_rectang.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(middlePoint, familySymbol_hole_walls_rectang, (Level)Doc.GetElement(duct.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);

                                        XYZ orientation = wall.Orientation;
                                        double angle = 0;
                                        //if (orientation.X == 0 && orientation.Y == 0) angle = 0;
                                        if(orientation.X <= 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X < 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);
                                            

                                        
                                        LocationCurve lcurve = (LocationCurve)duct.Location;
                                        Curve curve = lcurve.Curve;
                                        Line curveline = (Line)curve;
                                        double delta = Math.Sin(Math.Abs(Math.Abs(curveline.Direction.X) - Math.Abs(orientation.X)));
                                        if (delta > 0.00001) kIncrease = 1 + delta;

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); // vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(duct.Width * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Высота").Set(duct.Height + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(WallIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Отверстие_Отметка от этажа").Set(middlePoint.Z - ((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation - duct.Height / 2 - WallGap / 2); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(Math.Max(duct.Width, duct.Height)); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(duct.Id.IntegerValue.ToString() + " in " + wall.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Rectangular_Wall"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(duct.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }
                                
                        }
                    }
                }
                ViewModel.Statusik = $"установка прям. отверстий в стенах завершена";
            }
            
            #endregion

            #region round pipes walls
            
            if (ViewModel.RoundedPipes && ViewModel.InWalls)
            {
                familyCollection = new FilteredElementCollector(Doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Family.Name == family_name_hole_walls_round).ToList();
                //MessageBox.Show($"re.pi.{familyCollection.Count}");
                foreach (Pipe pipe in roundedPipes)
                {
                    foreach (Wall wall in walls)
                    {
                        ViewModel.Statusik = $"установка кругл. отверстий в стенах..";
                        Curve ductCurve = FindPipeCurve(pipe, out XYZ vector, out Line line);

                        intersections.Clear();
                        intersection = null;

                        List<Face> wallFaces = FindWallFace(wall, out double alfa);

                        foreach (Face face in wallFaces)
                        {
                            intersection = FindFaceCurve(ductCurve, line, face);
                            if (intersection != null) intersections.Add(intersection);
                        }

                        XYZ middlePoint = XYZ.Zero;

                        if (intersections.Count == 2)
                        {
                            middlePoint = (intersections[0] + intersections[1]) / 2;

                            familyIsInPoint = false;

                            try
                            {
                                foreach (FamilyInstance _f in familyCollection)
                                {
                                    Options options = new Options();
                                    options.ComputeReferences = true;
                                    options.DetailLevel = ViewDetailLevel.Undefined;
                                    options.IncludeNonVisibleObjects = true;
                                    GeometryElement geometryElement = _f.get_Geometry(options);
                                    BoundingBoxXYZ bb = geometryElement.GetBoundingBox();
                                    XYZ centerBB = (bb.Min + bb.Max) / 2;
                                    if (Math.Abs(middlePoint.DistanceTo(centerBB)) < 0.000001) familyIsInPoint = true;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }

                            FamilyInstance familyInstance = null;
                            double kIncrease = 1;

                            if (!familyIsInPoint && pipe.Diameter >= MinDiameter && pipe.Diameter < MaxStopDiameter && pipe.Diameter <= MaxDiameter)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_walls_round.IsActive) familySymbol_hole_walls_round.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(middlePoint, familySymbol_hole_walls_round, (Level)Doc.GetElement(pipe.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);

                                        XYZ orientation = wall.Orientation;
                                        double angle = 0;
                                        //if (orientation.X == 0 && orientation.Y == 0) angle = 0;
                                        if (orientation.X <= 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X < 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);

                                        LocationCurve lcurve = (LocationCurve)pipe.Location;
                                        Curve curve = lcurve.Curve;
                                        Line curveline = (Line)curve;
                                        double delta = Math.Sin(Math.Abs(Math.Abs(curveline.Direction.X) - Math.Abs(orientation.X)));
                                        if (delta > 0.00001) kIncrease = 1 + delta;

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); // vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(pipe.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Диаметр").Set(pipe.Diameter * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(WallIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Отверстие_Отметка от этажа").Set(middlePoint.Z - ((Level)Doc.GetElement(pipe.ReferenceLevel.Id)).Elevation); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(pipe.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(pipe.Id.IntegerValue.ToString() + " in " + wall.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Round_Wall"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(pipe.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }
                            else if (!familyIsInPoint && pipe.Diameter >= MinDiameter && pipe.Diameter < MaxStopDiameter && pipe.Diameter > MaxDiameter)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_walls_rectang.IsActive) familySymbol_hole_walls_rectang.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(middlePoint, familySymbol_hole_walls_rectang, (Level)Doc.GetElement(pipe.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);

                                        XYZ orientation = wall.Orientation;
                                        double angle = 0;
                                        //if (orientation.X == 0 && orientation.Y == 0) angle = 0;
                                        if (orientation.X <= 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X < 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);

                                        LocationCurve lcurve = (LocationCurve)pipe.Location;
                                        Curve curve = lcurve.Curve;
                                        Line curveline = (Line)curve;
                                        double delta = Math.Sin(Math.Abs(Math.Abs(curveline.Direction.X) - Math.Abs(orientation.X)));
                                        if (delta > 0.00001) kIncrease = 1 + delta;

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); // vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(pipe.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(pipe.Diameter * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Высота").Set(pipe.Diameter * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(WallIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Отверстие_Отметка от этажа").Set(middlePoint.Z - ((Level)Doc.GetElement(pipe.ReferenceLevel.Id)).Elevation); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(pipe.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(pipe.Id.IntegerValue.ToString() + " in " + wall.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Rectangular_Wall"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(pipe.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }
                        }
                    }
                }
                ViewModel.Statusik = $"установка кругл. отверстий в стенах завершена";
            }

            #endregion

            #region rectang cable trays

            if (ViewModel.RectangCableTray && ViewModel.InWalls)
            {
                familyCollection = new FilteredElementCollector(Doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Family.Name == family_name_hole_walls_rectang).ToList();
                //MessageBox.Show($"re.du.{familyCollection.Count}");
                foreach (CableTray cabTray in rectangularCableTrays)
                {
                    foreach (Wall wall in walls)
                    {
                        ViewModel.Statusik = $"установка прям. отверстий в стенах..";
                        Curve cableTrayCurve = FindCabTrayCurve(cabTray, out XYZ vector, out Line line);

                        intersections.Clear();
                        intersection = null;

                        List<Face> wallFaces = FindWallFace(wall, out double alfa);

                        foreach (Face face in wallFaces)
                        {
                            intersection = FindFaceCurve(cableTrayCurve, line, face);
                            if (intersection != null) intersections.Add(intersection);
                        }

                        XYZ middlePoint = XYZ.Zero;

                        familyIsInPoint = false;

                        if (intersections.Count == 2)
                        {
                            middlePoint = (intersections[0] + intersections[1]) / 2;

                            try
                            {
                                foreach (FamilyInstance _f in familyCollection)
                                {
                                    Options options = new Options();
                                    options.ComputeReferences = true;
                                    options.DetailLevel = ViewDetailLevel.Undefined;
                                    options.IncludeNonVisibleObjects = true;
                                    GeometryElement geometryElement = _f.get_Geometry(options);
                                    BoundingBoxXYZ bb = geometryElement.GetBoundingBox();
                                    XYZ centerBB = (bb.Min + bb.Max) / 2;
                                    if (Math.Abs(middlePoint.DistanceTo(centerBB)) < 0.000001) familyIsInPoint = true;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }

                            FamilyInstance familyInstance = null;
                            double kIncrease = 1;
                            if (!familyIsInPoint)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_walls_rectang.IsActive) familySymbol_hole_walls_rectang.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(middlePoint, familySymbol_hole_walls_rectang, (Level)Doc.GetElement(cabTray.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);

                                        XYZ orientation = wall.Orientation;
                                        double angle = 0;
                                        //if (orientation.X == 0 && orientation.Y == 0) angle = 0;
                                        if (orientation.X <= 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X < 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y <= 0) angle = Math.Asin(orientation.X);
                                        else if (orientation.X > 0 && orientation.Y > 0) angle = -Math.Asin(orientation.X);



                                        LocationCurve lcurve = (LocationCurve)cabTray.Location;
                                        Curve curve = lcurve.Curve;
                                        Line curveline = (Line)curve;
                                        double delta = Math.Sin(Math.Abs(Math.Abs(curveline.Direction.X) - Math.Abs(orientation.X)));
                                        if (delta > 0.00001) kIncrease = 1 + delta;

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); // vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(cabTray.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(cabTray.Width * kIncrease * kIncrease + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Высота").Set(cabTray.Height + WallGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(WallIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Отверстие_Отметка от этажа").Set(middlePoint.Z - ((Level)Doc.GetElement(cabTray.ReferenceLevel.Id)).Elevation - cabTray.Height / 2 - WallGap / 2); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(wall.Width); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(Math.Max(cabTray.Width, cabTray.Height)); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(cabTray.Id.IntegerValue.ToString() + " in " + wall.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Rectangular_Wall"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(cabTray.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }

                        }
                    }
                }
                ViewModel.Statusik = $"установка прям. отверстий в стенах завершена";
            }

            #endregion

            #region round ducts floors

            if (ViewModel.RoundedDucts && ViewModel.InFloors)
            {
                familyCollection = new FilteredElementCollector(Doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Family.Name == family_name_hole_floors_round).ToList();
                //MessageBox.Show($"ro.du.floor {familyCollection.Count}");
                foreach (Duct duct in roundedDucts)
                {
                    foreach (Floor floor in floors)
                    {
                        ViewModel.Statusik = $"установка кругл. отверстий в перекр..";
                        Curve ductCurve = FindDuctCurve(duct, out XYZ vector, out Line line);

                        intersections.Clear();
                        intersection = null;

                        List<Face> floorFaces = FindFloorFace(floor);

                        foreach (Face face in floorFaces)
                        {
                            intersection = FindFaceCurve(ductCurve, line, face);
                            if (intersection != null) intersections.Add(intersection);
                        }

                        XYZ middlePoint = XYZ.Zero;
                        XYZ maxZPoint = XYZ.Zero;

                        familyIsInPoint = false;

                        if (intersections.Count == 2)
                        {
                            middlePoint = (intersections[0] + intersections[1]) / 2;

                            if (intersections[0].Z > intersections[1].Z) maxZPoint = new XYZ(intersections[0].X, intersections[0].Y, intersections[0].Z);
                            else maxZPoint = new XYZ(intersections[1].X, intersections[1].Y, intersections[1].Z);

                            familyIsInPoint = false;

                            try
                            {
                                foreach (FamilyInstance _f in familyCollection)
                                {
                                    Options options = new Options();
                                    options.ComputeReferences = true;
                                    options.DetailLevel = ViewDetailLevel.Undefined;
                                    options.IncludeNonVisibleObjects = true;
                                    GeometryElement geometryElement = _f.get_Geometry(options);
                                    BoundingBoxXYZ bb = geometryElement.GetBoundingBox();
                                    XYZ centerBB = (bb.Min + bb.Max) / 2;
                                    if (Math.Abs(middlePoint.DistanceTo(centerBB)) < 0.000001) familyIsInPoint = true;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }

                            FamilyInstance familyInstance = null;

                            if (!familyIsInPoint && duct.Diameter >= MinDiameter && duct.Diameter <= MaxDiameter)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_floors_round.IsActive) familySymbol_hole_floors_round.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(maxZPoint, familySymbol_hole_floors_round, (Level)Doc.GetElement(duct.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);
                                        double angle = 0;
                                        ConnectorSet cSet = duct.ConnectorManager.Connectors;
                                        foreach (Connector cn in cSet)
                                        {
                                            Transform transform = cn.CoordinateSystem;
                                            if (transform.BasisY.X * transform.BasisY.Y <= 0) angle = Math.Asin(Math.Abs(transform.BasisY.X));
                                            else angle = -Math.Asin(Math.Abs(transform.BasisY.X));
                                        }

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); //vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }

                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Диаметр").Set(duct.Diameter + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(FloorIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(duct.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(duct.Id.IntegerValue.ToString() + " in " + floor.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Round_Floor"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(duct.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }
                            else if (!familyIsInPoint && duct.Diameter >= MinDiameter && duct.Diameter > MaxDiameter)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_floors_rectang.IsActive) familySymbol_hole_floors_rectang.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(maxZPoint, familySymbol_hole_floors_rectang, (Level)Doc.GetElement(duct.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);
                                        double angle = 0;
                                        ConnectorSet cSet = duct.ConnectorManager.Connectors;
                                        foreach (Connector cn in cSet)
                                        {
                                            Transform transform = cn.CoordinateSystem;
                                            if (transform.BasisY.X * transform.BasisY.Y <= 0) angle = Math.Asin(Math.Abs(transform.BasisY.X));
                                            else angle = -Math.Asin(Math.Abs(transform.BasisY.X));
                                        }

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); //vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(duct.Diameter + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Высота").Set(duct.Diameter + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(FloorIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(duct.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(duct.Id.IntegerValue.ToString() + " in " + floor.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Rectangular_Floor"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(duct.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }

                        }
                    }
                }
                ViewModel.Statusik = $"установка кругл. отверстий в перекр завершена";
            }
            
            #endregion

            #region rectang ducts floors

            if (ViewModel.RectangDucts && ViewModel.InFloors)
            {
                familyCollection = new FilteredElementCollector(Doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Family.Name == family_name_hole_floors_rectang).ToList();
                //MessageBox.Show($"ro.du.floor {familyCollection.Count}");
                foreach (Duct duct in rectangularDucts)
                {
                    foreach (Floor floor in floors)
                    {
                        ViewModel.Statusik = $"установка прям. отверстий в перекр..";
                        Curve ductCurve = FindDuctCurve(duct, out XYZ vector, out Line line);

                        intersections.Clear();
                        intersection = null;

                        List<Face> floorFaces = FindFloorFace(floor);

                        foreach (Face face in floorFaces)
                        {
                            intersection = FindFaceCurve(ductCurve, line, face);
                            if (intersection != null) intersections.Add(intersection);
                        }

                        XYZ middlePoint = XYZ.Zero;
                        XYZ maxZPoint = XYZ.Zero;

                        familyIsInPoint = false;

                        if (intersections.Count == 2)
                        {
                            middlePoint = (intersections[0] + intersections[1]) / 2;

                            if (intersections[0].Z > intersections[1].Z) maxZPoint = new XYZ(intersections[0].X, intersections[0].Y, intersections[0].Z);
                            else maxZPoint = new XYZ(intersections[1].X, intersections[1].Y, intersections[1].Z);

                            familyIsInPoint = false;

                            try
                            {
                                foreach (FamilyInstance _f in familyCollection)
                                {
                                    Options options = new Options();
                                    options.ComputeReferences = true;
                                    options.DetailLevel = ViewDetailLevel.Undefined;
                                    options.IncludeNonVisibleObjects = true;
                                    GeometryElement geometryElement = _f.get_Geometry(options);
                                    BoundingBoxXYZ bb = geometryElement.GetBoundingBox();
                                    XYZ centerBB = (bb.Min + bb.Max) / 2;
                                    if (Math.Abs(middlePoint.DistanceTo(centerBB)) < 0.000001) familyIsInPoint = true;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }

                            FamilyInstance familyInstance = null;

                            if (!familyIsInPoint)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_floors_rectang.IsActive) familySymbol_hole_floors_rectang.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(maxZPoint, familySymbol_hole_floors_rectang, (Level)Doc.GetElement(duct.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);
                                        double angle = 0;
                                        ConnectorSet cSet = duct.ConnectorManager.Connectors;
                                        foreach (Connector cn in cSet)
                                        {
                                            Transform transform = cn.CoordinateSystem;
                                            if (transform.BasisY.X * transform.BasisY.Y <= 0) angle = Math.Asin(Math.Abs(transform.BasisY.X));
                                            else angle = - Math.Asin(Math.Abs(transform.BasisY.X));
                                        }

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); //vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(duct.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(duct.Width + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Высота").Set(duct.Height + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(FloorIndent); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(duct.Id.IntegerValue.ToString() + " in " + floor.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(Math.Max(duct.Width, duct.Height)); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Rectangular_Floor"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(duct.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }

                        }
                    }
                }
                ViewModel.Statusik = $"установка прям. отверстий в перекр. завершена";
            }

            #endregion

            #region round pipes floors

            if (ViewModel.RoundedPipes && ViewModel.InFloors)
            {
                familyCollection = new FilteredElementCollector(Doc).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().Where(x => x.Symbol.Family.Name == family_name_hole_floors_round).ToList();

                foreach (Pipe pipe in roundedPipes)
                {
                    foreach (Floor floor in floors)
                    {
                        ViewModel.Statusik = $"установка кругл. отверстий в перекр..";
                        Curve pipeCurve = FindPipeCurve(pipe, out XYZ vector, out Line line);

                        intersections.Clear();
                        intersection = null;

                        List<Face> floorFaces = FindFloorFace(floor);

                        foreach (Face face in floorFaces)
                        {
                            intersection = FindFaceCurve(pipeCurve, line, face);
                            if (intersection != null) intersections.Add(intersection);
                        }

                        XYZ middlePoint = XYZ.Zero;
                        XYZ maxZPoint = XYZ.Zero;

                        familyIsInPoint = false;

                        if (intersections.Count == 2)
                        {
                            middlePoint = (intersections[0] + intersections[1]) / 2;

                            if (intersections[0].Z > intersections[1].Z) maxZPoint = new XYZ(intersections[0].X, intersections[0].Y, intersections[0].Z);
                            else maxZPoint = new XYZ(intersections[1].X, intersections[1].Y, intersections[1].Z);

                            familyIsInPoint = false;

                            try
                            {
                                foreach (FamilyInstance _f in familyCollection)
                                {
                                    Options options = new Options();
                                    options.ComputeReferences = true;
                                    options.DetailLevel = ViewDetailLevel.Undefined;
                                    options.IncludeNonVisibleObjects = true;
                                    GeometryElement geometryElement = _f.get_Geometry(options);
                                    BoundingBoxXYZ bb = geometryElement.GetBoundingBox();
                                    XYZ centerBB = (bb.Min + bb.Max) / 2;
                                    if (Math.Abs(middlePoint.DistanceTo(centerBB)) < 0.000001) familyIsInPoint = true;
                                }
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }

                            FamilyInstance familyInstance = null;

                            if (!familyIsInPoint && pipe.Diameter >= MinDiameter && pipe.Diameter < MaxStopDiameter && pipe.Diameter <= MaxDiameter) 
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_floors_round.IsActive) familySymbol_hole_floors_round.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(maxZPoint, familySymbol_hole_floors_round, (Level)Doc.GetElement(pipe.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);
                                        double angle = 0;
                                        ConnectorSet cSet = pipe.ConnectorManager.Connectors;
                                        foreach (Connector cn in cSet)
                                        {
                                            Transform transform = cn.CoordinateSystem;
                                            if (transform.BasisY.X * transform.BasisY.Y <= 0) angle = Math.Asin(Math.Abs(transform.BasisY.X));
                                            else angle = -Math.Asin(Math.Abs(transform.BasisY.X));
                                        }

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); //vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(pipe.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Диаметр").Set(pipe.Diameter + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(FloorIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(pipe.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(pipe.Id.IntegerValue.ToString() + " in " + floor.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Round_Floor"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(pipe.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }
                            else if (!familyIsInPoint && pipe.Diameter >= MinDiameter && pipe.Diameter < MaxStopDiameter && pipe.Diameter > MaxDiameter)
                            {
                                using (Transaction tr = new Transaction(Doc, " create fi "))
                                {
                                    tr.Start();
                                    try
                                    {
                                        if (!familySymbol_hole_floors_rectang.IsActive) familySymbol_hole_floors_rectang.Activate();
                                        familyInstance = Doc.Create.NewFamilyInstance(maxZPoint, familySymbol_hole_floors_rectang, (Level)Doc.GetElement(pipe.ReferenceLevel.Id), Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                                        XYZ point1 = new XYZ(middlePoint.X, middlePoint.Y, 0);
                                        XYZ point2 = new XYZ(middlePoint.X, middlePoint.Y, 10);
                                        Line axis = Line.CreateBound(point1, point2);
                                        double angle = 0;
                                        ConnectorSet cSet = pipe.ConnectorManager.Connectors;
                                        foreach (Connector cn in cSet)
                                        {
                                            Transform transform = cn.CoordinateSystem;
                                            if (transform.BasisY.X * transform.BasisY.Y <= 0) angle = Math.Asin(Math.Abs(transform.BasisY.X));
                                            else angle = -Math.Asin(Math.Abs(transform.BasisY.X));
                                        }

                                        ElementTransformUtils.RotateElement(Doc, familyInstance.Id, axis, angle + StartAngle); //vector.AngleTo(XYZ.BasisY));
                                        ElementTransformUtils.MoveElement(Doc, familyInstance.Id, new XYZ(0, 0, -((Level)Doc.GetElement(pipe.ReferenceLevel.Id)).Elevation));
                                    }
                                    catch (Exception ex)
                                    {
#if DEBUG
                                        MessageBox.Show(ex.ToString());
#endif
                                    }
                                    tr.Commit();
                                }

                                FamilyManager fm = Doc.EditFamily(familyInstance.Symbol.Family).FamilyManager;
                                var familyParameters = fm.GetParameters();
                                using (Transaction tr = new Transaction(Doc, " set parameters fi "))
                                {
                                    tr.Start();
                                    try { familyInstance.LookupParameter("ADSK_Размер_Ширина").Set(pipe.Diameter + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Высота").Set(pipe.Diameter + FloorGap); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Глубина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("М1_Отверстие_Выступ").Set(FloorIndent); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_Длина").Set(((FloorType)Doc.GetElement(floor.GetTypeId())).GetCompoundStructure().GetWidth()); } catch { };
                                    try { familyInstance.LookupParameter("ADSK_Размер_ДиаметрИзделия").Set(pipe.Diameter); } catch { };
                                    try { familyInstance.LookupParameter("M1_ForElements").Set(pipe.Id.IntegerValue.ToString() + " in " + floor.Id.IntegerValue.ToString()); } catch { };
                                    try { familyInstance.LookupParameter("M1_ElementMask").Set("M1_Void_Rectangular_Floor"); } catch { };
                                    try { familyInstance.LookupParameter("M1_SystemName").Set(pipe.MEPSystem.Name); } catch { };
                                    try { familyInstance.LookupParameter("M1_СreationDate").Set($"{DateTime.Now.ToString("dd/MM/yyyy")}"); } catch { };
                                    tr.Commit();
                                }
                            }

                        }
                    }
                }
                ViewModel.Statusik = $"установка кругл. отверстий в перекр завершена";
            }

            #endregion



            ViewModel.Statusik = $" - установка всех отверстий завершена - ";
            Window.Close();
        }

        public string GetName()
        {
            return "CreateVoids";
        }

        private string AddFamilyParametersInFamilyDocument(UIApplication uiapp, string path, List<string> paramNames)
        {
            Document familyDocument = uiapp.Application.OpenDocumentFile(path);

            using (Transaction docTrans = new Transaction(familyDocument, $"Add family parameters in {familyDocument.Title}"))
            {
                docTrans.Start();

                MessageBox.Show("редактируем " + familyDocument.Title);

                if (familyDocument.IsModifiable)
                {

                    FamilyManager familyManager = familyDocument.FamilyManager;

                    var fParameters = familyManager.GetParameters();
                    foreach (string pName in paramNames)
                    {
                        bool parameterExists = false;
                        foreach (FamilyParameter fp in fParameters)
                        {
                            if (fp.Definition.Name == pName) parameterExists = true;
                        }
                        if (!parameterExists)
                        {
                            try
                            {
                                familyManager.AddParameter(pName, BuiltInParameterGroup.INVALID, ParameterType.Text, true);
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                MessageBox.Show(ex.ToString());
#endif
                            }
                        }
                    }


                }
                else
                {
#if DEBUG
                    MessageBox.Show("Семейство не изменено IsModifiable == false");
#endif
                }

                docTrans.Commit();
            }
            string newPath = path.Replace(".rfa", "_v1.rfa");
            familyDocument.SaveAs(newPath, new SaveAsOptions() { OverwriteExistingFile = true });
            familyDocument.Close(false);
            return newPath;
        }
        private Family LoadFamily(Document doc, string name, string path)
        {
            var families = new FilteredElementCollector(doc).OfClass(typeof(Family));

            Family family = families.FirstOrDefault<Element>(e => e.Name.Equals(name)) as Family;

            if (null == family)
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show("Нет семейства для загрузки \n" + name);
                }

                using (Transaction tx = new Transaction(doc, "Load Family"))
                {
                    tx.Start();
                    doc.LoadFamily(path, new JtFamilyLoadOptions(), out family);
                    tx.Commit();
                }
            }
            return family;
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
            else vector = (list.ElementAt(1) - list.ElementAt(0)).Normalize();

            curve.MakeUnbound();

            return curve;
        }
        public Curve FindCabTrayCurve(CableTray cabTray, out XYZ vector, out Line line)
        {
            //The wind pipe curve
            IList<XYZ> list = new List<XYZ>();
            ConnectorSetIterator csi = cabTray.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                list.Add(conn.Origin);
            }

            if (list.ElementAt(0).X < list.ElementAt(1).X) line = Line.CreateBound(list.ElementAt(0), list.ElementAt(1));
            else line = Line.CreateBound(list.ElementAt(1), list.ElementAt(0));

            Curve curve = Line.CreateBound(list.ElementAt(0), list.ElementAt(1)) as Curve;

            if (list.ElementAt(0).X < list.ElementAt(1).X) vector = (list.ElementAt(0) - list.ElementAt(1)).Normalize();
            else vector = (list.ElementAt(1) - list.ElementAt(0)).Normalize();

            curve.MakeUnbound();

            return curve;
        }
        public Curve FindPipeCurve(Pipe pipe, out XYZ vector, out Line line)
        {
            //The wind pipe curve
            IList<XYZ> list = new List<XYZ>();
            ConnectorSetIterator csi = pipe.ConnectorManager.Connectors.ForwardIterator();
            while (csi.MoveNext())
            {
                Connector conn = csi.Current as Connector;
                list.Add(conn.Origin);
            }

            if (list.ElementAt(0).X < list.ElementAt(1).X) line = Line.CreateBound(list.ElementAt(0), list.ElementAt(1));
            else line = Line.CreateBound(list.ElementAt(1), list.ElementAt(0));

            Curve curve = Line.CreateBound(list.ElementAt(0), list.ElementAt(1)) as Curve;

            if (list.ElementAt(0).X < list.ElementAt(1).X) vector = (list.ElementAt(0) - list.ElementAt(1)).Normalize();
            else vector = (list.ElementAt(1) - list.ElementAt(0)).Normalize();

            curve.MakeUnbound();

            return curve;
        }
        public List<Face> FindWallFace(Wall wall, out double alfa)
        {
            List<Face> normalFaces = new List<Face>();

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

            if (Math.Abs(end.X - start.X) < 0.00001)
            {
                alfa = Math.PI / 2;
            }
            else if (Math.Abs(end.Y - start.Y) < 0.00001)
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
                }
            }


            return normalFaces;
        }
        public List<Face> FindFloorFace(Floor floor)
        {
            List<Face> normalFaces = new List<Face>();

            //LocationCurve locCurve = floor.Location as LocationCurve;

            //XYZ start;
            //XYZ end;

            //if (locCurve.Curve.GetEndPoint(0).X <= locCurve.Curve.GetEndPoint(1).X)
            //{
            //    start = locCurve.Curve.GetEndPoint(0);
            //    end = locCurve.Curve.GetEndPoint(1);
            //}
            //else
            //{
            //    start = locCurve.Curve.GetEndPoint(1);
            //    end = locCurve.Curve.GetEndPoint(0);
            //}

            //XYZ locCurveCenter = (start + end) / 2;

            //if (Math.Abs(end.X - start.X) < 0.00001)
            //{
            //    alfa = Math.PI / 2;
            //}
            //else if (Math.Abs(end.Y - start.Y) < 0.00001)
            //{
            //    alfa = Math.PI / 2;
            //}
            //else
            //{
            //    alfa = Math.Atan((end.Y - start.Y) / (end.X - start.X));
            //}

            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Fine;

            GeometryElement e = floor.get_Geometry(opt);

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
                }
            }

            return normalFaces;
        }
        public XYZ FindFaceCurve(Curve curve, Line line, Face WallFace)
        {
            //The intersection point
            IntersectionResultArray intersectionR = new IntersectionResultArray();//Intersection point set

            SetComparisonResult results;//Results of Comparison

            results = WallFace.Intersect(curve, out intersectionR);

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
        public XYZ FindCenterOfFace(Face face)
        {
            double CurvePoints_Umin = double.MaxValue;
            double CurvePoints_Umax = double.MinValue;
            double CurvePoints_Vmin = double.MaxValue;
            double CurvePoints_Vmax = double.MinValue;

            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    foreach (UV uv in edge.TessellateOnFace(face))
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
        public void AddParams(string prm, CategorySet catSet, Application app, Document doc)
        {
            try
            {
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Add Shared Parameters");
                    DefinitionFile sharedParameterFile = app.OpenSharedParameterFile();
                    DefinitionGroup sharedParameterGroup = sharedParameterFile.Groups.get_Item("ADG");
                    Definition sharedParameterDefinition = sharedParameterGroup.Definitions.get_Item(prm);
                    ExternalDefinition externalDefinition =
                        sharedParameterGroup.Definitions.get_Item(prm) as ExternalDefinition;
                    Guid guid = externalDefinition.GUID;
                    InstanceBinding newIB = app.Create.NewInstanceBinding(catSet);
                    doc.ParameterBindings.Insert(externalDefinition, newIB, BuiltInParameterGroup.INVALID);
                    //SharedParameterElement sp = SharedParameterElement.Lookup(doc, guid);
                    // InternalDefinition def = sp.GetDefinition();
                    // def.SetAllowVaryBetweenGroups(doc, true);
                    t.Commit();
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
    public static class HereExtensions
    {
        static readonly double EPSILON = Math.Pow(2, -52);
        public static bool Contains(this Line line, XYZ point)
        {
            XYZ a = line.GetEndPoint(0); // Line start point
            XYZ b = line.GetEndPoint(1); // Line end point
            XYZ p = point;
            return (Math.Abs(a.DistanceTo(b) - (a.DistanceTo(p) + p.DistanceTo(b))) < EPSILON * 1000);
        }
        public static double ToMM(this double input)
        {
            return input * 304.8;
        }
        public static double ToFeet(this double input)
        {
            return input / 304.8;
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
            if (elem.Category != null)
            {
                if (elem.Category.Id.IntegerValue == -2001352) //(int)BuiltInCategory.OST_RvtLinks)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
            return false;
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
