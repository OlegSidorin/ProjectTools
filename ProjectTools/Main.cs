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
        public static string PanelName { get; set; } = "В проекте";


        public Result OnStartup(UIControlledApplication application)
        {
            #region Initialize properties

            #endregion

            RibbonPanel panel = application.CreateRibbonPanel(PanelName);
            DllLocation = Assembly.GetExecutingAssembly().Location;
            DllFolderLocation = Path.GetDirectoryName(DllLocation);
            UserFolder = @"C:\Users\" + Environment.UserName;

            #region Buttons

            var CreateBIM360ViewBtnData = new PushButtonData("CreateBIM360BtnData", "Создать вид\nNavisworks", DllLocation, "ProjectTools.Command01")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-home.png", UriKind.Absolute)),
                ToolTip = "Создает вид Navisworks, \nвыгружает ссылки,\nготовит набор к публикации в BIM360"
            };
            var CreateBIM360ViewBtn = panel.AddItem(CreateBIM360ViewBtnData) as PushButton;
            CreateBIM360ViewBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-home.png", UriKind.Absolute));

            var WorkSetRVTLinksBtnData = new PushButtonData("WorkSetRVTLinksBtnData", "Разместить в\nрабочих наб", DllLocation, "ProjectTools.Command02")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-layers.png", UriKind.Absolute)),
                ToolTip = "Создает рабочие наборы для RVT связей"
            };
            var WorkSetRVTLinksBtn = panel.AddItem(WorkSetRVTLinksBtnData) as PushButton;
            WorkSetRVTLinksBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\icons8-layers.png", UriKind.Absolute));

            var NewDuctViewsBtnData = new PushButtonData("NewDuctViewsBtnData", "Создать\nвиды", DllLocation, "ProjectTools.Command04")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\duct-icon.png", UriKind.Absolute)),
                ToolTip = "Создает виды для вент коробов как в АДСК шаблоне"
            };
            var NewDuctViewsBtn = panel.AddItem(NewDuctViewsBtnData) as PushButton;
            NewDuctViewsBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\duct-icon.png", UriKind.Absolute));

            var NewPipeViewsBtnData = new PushButtonData("NewPipeViewsBtnData", "Создать\nвиды", DllLocation, "ProjectTools.Command03")
            {
                ToolTipImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\pipe-icon.png", UriKind.Absolute)),
                ToolTip = "Создает виды для труб как в АДСК шаблоне"
            };
            var NewPipeViewsBtn = panel.AddItem(NewPipeViewsBtnData) as PushButton;
            NewPipeViewsBtn.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(DllLocation) + "\\res\\pipe-icon.png", UriKind.Absolute));

            

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
