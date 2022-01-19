using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using System;
using Autodesk.Revit.DB.Events;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    public class Main : IExternalApplication
    {
        public static string DllLocation { get; set; }
        public static string DllFolderLocation { get; set; }
        public static string UserFolder { get; set; }
        public static string TabName { get; set; } = "Надстройки";
        public static string PanelName { get; set; } = "M1 Add-Ins";


        public Result OnStartup(UIControlledApplication application)
        {
            #region Initialize properties

            #endregion

            RibbonPanel panel = application.CreateRibbonPanel(PanelName);
            DllLocation = Assembly.GetExecutingAssembly().Location;
            DllFolderLocation = Path.GetDirectoryName(DllLocation);
            UserFolder = @"C:\Users\" + Environment.UserName;

            #region Buttons

            PulldownButtonData NavisPulldownButtonData = new PulldownButtonData("NavisPulldownButtonData", "Navis");
            PulldownButton NavisPulldownButton = panel.AddItem(NavisPulldownButtonData) as PulldownButton;
            NavisPulldownButton.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\navis-view-plus-3.png", UriKind.Absolute));

            var CreateBIM360ViewBtnData = new PushButtonData("CreateBIM360BtnData", "Создать 3D вид Navisworks", DllLocation, "ProjectTools.Command01")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\navis-view-plus-3.png", UriKind.Absolute)),
                ToolTip = "Создает вид Navisworks, \nвыгружает ссылки,\nготовит набор к публикации в BIM360"
            };
            var CreateBIM360ViewBtn = NavisPulldownButton.AddPushButton(CreateBIM360ViewBtnData) as PushButton;
            CreateBIM360ViewBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\navis-view-plus-3.png", UriKind.Absolute));

            var ShowBIM360ViewBtnData = new PushButtonData("ShowBIM360BtnData", "Показать 3D вид Navisworks", DllLocation, "ProjectTools.Command01a")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\navis-view-3.png", UriKind.Absolute)),
                ToolTip = "Сделает 3D вид Navisworks активным"
            };
            var ShowBIM360ViewBtn = NavisPulldownButton.AddPushButton(ShowBIM360ViewBtnData) as PushButton;
            ShowBIM360ViewBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\navis-view-3.png", UriKind.Absolute));

            var WorkSetRVTLinksBtnData = new PushButtonData("WorkSetRVTLinksBtnData", "Разместить в рабочих наборах", DllLocation, "ProjectTools.Command02")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-layers.png", UriKind.Absolute)),
                ToolTip = "Создает рабочие наборы для RVT связей"
            };
            var WorkSetRVTLinksBtn = NavisPulldownButton.AddPushButton(WorkSetRVTLinksBtnData) as PushButton;
            WorkSetRVTLinksBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-layers.png", UriKind.Absolute));

            panel.AddSeparator();

            PulldownButtonData SchemePulldownButtonData = new PulldownButtonData("SchemePulldownButtonData", "Схемы");
            PulldownButton ShemePulldownButton = panel.AddItem(SchemePulldownButtonData) as PulldownButton;
            ShemePulldownButton.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\duct-icon.png", UriKind.Absolute));


            var NewDuctViewsBtnData = new PushButtonData("NewDuctViewsBtnData", "Создать схемы вентиляции", DllLocation, "ProjectTools.Command04")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\duct-icon.png", UriKind.Absolute)),
                ToolTip = "Создает виды для вент коробов как в АДСК шаблоне"
            };
            var NewDuctViewsBtn = ShemePulldownButton.AddPushButton(NewDuctViewsBtnData) as PushButton;
            NewDuctViewsBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\duct-icon.png", UriKind.Absolute));

            var NewPipeViewsBtnData = new PushButtonData("NewPipeViewsBtnData", "Создать схемы трубопроводов", DllLocation, "ProjectTools.Command03")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\pipe-icon.png", UriKind.Absolute)),
                ToolTip = "Создает виды для труб как в АДСК шаблоне"
            };
            var NewPipeViewsBtn = ShemePulldownButton.AddPushButton(NewPipeViewsBtnData) as PushButton;
            NewPipeViewsBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\pipe-icon.png", UriKind.Absolute));

            //panel.AddStackedItems(PulldownButtonData, NewPipeViewsBtnData);

            panel.AddSeparator();

            PulldownButtonData ZeroPulldownButtonData = new PulldownButtonData("ZeroPulldownButtonData", "Рабочие\nплоскости");
            PulldownButton ZeroPulldownButton = panel.AddItem(ZeroPulldownButtonData) as PulldownButton;
            ZeroPulldownButton.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\section-view.png", UriKind.Absolute));

            var ZeroBtnData = new PushButtonData("ZeroBtnData", "Создать линии центре проекта", DllLocation, "ProjectTools.Command08")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\zero-point.png", UriKind.Absolute)),
                ToolTip = "Создает в нуле линии"
            };
            var ZeroBtn = ZeroPulldownButton.AddPushButton(ZeroBtnData) as PushButton;
            ZeroBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\zero-point.png", UriKind.Absolute));

            var SetViewAngleBtnData = new PushButtonData("SetViewAngleBtnData", "Установить ориентацию активного вида", DllLocation, "ProjectTools.Command06")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\axes-icon.png", UriKind.Absolute)),
                ToolTip = "Создает ориентацию вида в пространстве"
            };
            var SetViewAngleBtn = ZeroPulldownButton.AddPushButton(SetViewAngleBtnData) as PushButton;
            SetViewAngleBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\axes-icon.png", UriKind.Absolute));

            var WorkPlaneBtnData = new PushButtonData("WorkPlaneBtnData", "Создать рабочую плоскость по ориентации на виде", DllLocation, "ProjectTools.Command05")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\section-view.png", UriKind.Absolute)),
                ToolTip = "Создает рабочую плоскость по активному виду"
            };
            var WorkPlaneBtn = ZeroPulldownButton.AddPushButton(WorkPlaneBtnData) as PushButton;
            WorkPlaneBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\section-view.png", UriKind.Absolute));

            panel.AddSeparator();

            var FilesListBtnData = new PushButtonData("FilesListBtnData", "Список\nфайлов", DllLocation, "ProjectTools.Command07")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\folders-icon.png", UriKind.Absolute)),
                ToolTip = "Создает список файлов из указанной папки"
            };
            var FilesListBtn = panel.AddItem(FilesListBtnData) as PushButton;
            FilesListBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\folders-icon.png", UriKind.Absolute));

            panel.AddSeparator();

            PulldownButtonData AddParamsInRoomsPulldownButtonData = new PulldownButtonData("AddParamsInRoomsPulldownButtonData", "Параметры\nТХ");
            PulldownButton AddParamsInRoomsPulldownButton = panel.AddItem(AddParamsInRoomsPulldownButtonData) as PulldownButton;
            AddParamsInRoomsPulldownButton.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-room.png", UriKind.Absolute));

            var AddParamsInRoomsBtnData = new PushButtonData("AddParamsInRoomsBtnData", "Добавить параметры М1_Номер и М1_Имя в помещения", DllLocation, "ProjectTools.Command10")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-room.png", UriKind.Absolute)),
                ToolTip = "Добавляет параметры М1_Номер и М1_Имя в помещения, присваивает текущие значения"
            };
            var AddParamsInRoomsBtn = AddParamsInRoomsPulldownButton.AddPushButton(AddParamsInRoomsBtnData) as PushButton;
            AddParamsInRoomsBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-room.png", UriKind.Absolute));

            var CheckRoomsBtnData = new PushButtonData("CheckRoomsBtnData", "Проверка помещений на соответствие с параметрами ТХ", DllLocation, "ProjectTools.Command09")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-room.png", UriKind.Absolute)),
                ToolTip = "Создает список помещеий, в которых номер помещений не совпадает с параметрами для ТХ"
            };
            var CheckRoomsBtn = AddParamsInRoomsPulldownButton.AddPushButton(CheckRoomsBtnData) as PushButton;
            CheckRoomsBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-room.png", UriKind.Absolute));

            panel.AddSeparator();

            PulldownButtonData MaterialPulldownButtonData = new PulldownButtonData("MaterialPulldownButtonData", "Материал");
            PulldownButton MaterialPulldownButton = panel.AddItem(MaterialPulldownButtonData) as PulldownButton;
            MaterialPulldownButton.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\wall.png", UriKind.Absolute));

            var ChangeMaterialBtnData = new PushButtonData("ChangeMaterialBtnData", "Заменить материал стен", DllLocation, "ProjectTools.Command11")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\wall.png", UriKind.Absolute)),
                ToolTip = "Меняет материал всех стен на материал с названием M1_Material"
            };
            var ChangeMaterialBtn = MaterialPulldownButton.AddPushButton(ChangeMaterialBtnData) as PushButton;
            ChangeMaterialBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\wall.png", UriKind.Absolute));

            var ChangeFloorMaterialBtnData = new PushButtonData("ChangeFloorMaterialBtnData", "Заменить материал пола", DllLocation, "ProjectTools.Command12")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\floor.png", UriKind.Absolute)),
                ToolTip = "Меняет материал всех полов на материал с названием M1_Material"
            };
            var ChangeFloorMaterialBtn = MaterialPulldownButton.AddPushButton(ChangeFloorMaterialBtnData) as PushButton;
            ChangeFloorMaterialBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\floor.png", UriKind.Absolute));

            panel.AddSeparator();

            var AutoHolesBtnData = new PushButtonData("AutoHolesBtnData", "Расставить\nзаглушки отверстий", DllLocation, "ProjectTools.Command13")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\holes.png", UriKind.Absolute)),
                ToolTip = "Автоматическая расстановка отверстий"
            };
            var AutoHolesBtn = panel.AddItem(AutoHolesBtnData) as PushButton;
            AutoHolesBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\holes.png", UriKind.Absolute));


            //PushButtonData ButtonDataQ = new PushButtonData("ButtonDataQ", "Проверить\nпараметры", DllLocation, "FamilyParametersChanger.MainCommandQ")
            //{
            //    ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\q-icon.png", UriKind.Absolute)),
            //    //ToolTipImage = PngImageSource("BatchAddingParameters.res.bap-icon.png"),
            //    ToolTip = "Позволяет проверить параметры в семействе"
            //};
            //PushButton pushButtonQ = panel.AddItem(ButtonDataQ) as PushButton;
            //pushButtonQ.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\q-icon.png", UriKind.Absolute));

            //PushButtonData ButtonData0 = new PushButtonData("ButtonData0", "Пофиксить\nпараметры", DllLocation, "FamilyParametersChanger.MainCommand0")
            //{
            //    ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\sp-icon.png", UriKind.Absolute)),
            //    //ToolTipImage = PngImageSource("BatchAddingParameters.res.bap-icon.png"),
            //    ToolTip = "Если есть возможность, то фиксит параметры"
            //};
            //PushButton pushButton0 = panel.AddItem(ButtonData0) as PushButton;
            //pushButton0.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\sp-icon.png", UriKind.Absolute));

            //PushButtonData ButtonData1 = new PushButtonData("ButtonData1", "Перевести\nв обычные", DllLocation, "FamilyParametersChanger.MainCommand1")
            //{
            //    ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\fp-icon.png", UriKind.Absolute)),
            //    //ToolTipImage = PngImageSource("BatchAddingParameters.res.bap-icon.png"),
            //    ToolTip = "Позволяет перевести неизвестные общие параметры в параметры семейства"
            //};
            //PushButton pushButton1 = panel.AddItem(ButtonData1) as PushButton;
            //pushButton1.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\fp-icon.png", UriKind.Absolute));

            //PushButtonData ButtonData2 = new PushButtonData("ButtonData2", "Перевести\nв общие", DllLocation, "FamilyParametersChanger.MainCommand2")
            //{
            //    ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\sp-icon.png", UriKind.Absolute)),
            //    //ToolTipImage = PngImageSource("BatchAddingParameters.res.bap-icon.png"),
            //    ToolTip = "Позволяет перевести параметры семейства по имени в общие параметры"
            //};
            //PushButton pushButton2 = panel.AddItem(ButtonData2) as PushButton;
            //pushButton2.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\sp-icon.png", UriKind.Absolute));

            #endregion

            return Result.Succeeded;
        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
