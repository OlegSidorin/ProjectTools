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

namespace ProjectTools
{
    [Transaction(TransactionMode.Manual), Regeneration(RegenerationOption.Manual)]
    class Command07 : IExternalCommand
    {
        // готовит список файлов в указанной папке
        static string filename { get; set; } = @"C:\Users\" + Environment.UserName + @"\Downloads\\Dirinfo\dirinfo.txt";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            string ourPath = GetOurPath(out bool isCanceled1);
            if (isCanceled1) return Result.Succeeded;

            var dirList = new List<string>();

            var di = (new DirectoryInfo(ourPath)).GetDirectories();

            foreach(var d in di)
            {
                dirList.Add(d.Name);
            }

            filename = FileForReport(out bool isCanceled2);

            if (isCanceled2) return Result.Succeeded;

            if (filename.EndsWith(".txt"))
            {
                MakeHeader();
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.pdf", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.txt", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.doc", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.docx", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.xls", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.xlsx", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.dwg", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.rvt", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.rfa", dirList);
                WalkDirectoryTree(new DirectoryInfo(ourPath), "*.nwd", dirList);
                //WalkDirectoryTree(new DirectoryInfo(ourPath), "*.jpg", dirList);

                MessageBox.Show($"Отчет сохранен в файле:\n{filename}", "Report");

                string strCmdText;
                strCmdText = filename;
                System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\system32\notepad.exe", strCmdText);

            }

            return Result.Succeeded;
        }

        private void MakeHeader()
        {
            WriteToFile(filename, String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", "mask", "Наименование", "Наименование", "Версия", "Дата", "Папка", "Путь"));
        }

        private string GetOurPath(out bool isCanceled)
        {
            string str = @"C:\Users\" + Environment.UserName;
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = str;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                str = dialog.FileName;
                isCanceled = false;
            } else
            {
                isCanceled = true;
            }
            return str;
        }

        private string FileForReport(out bool isCanceled)
        {
            string str = @"C:\Users\" + Environment.UserName + @"\Downloads";
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = str;
            dialog.IsFolderPicker = false;
            dialog.DefaultFileName = "report.txt";
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                str = dialog.FileName;
                if (!str.Contains(".txt")) str += ".txt";
                isCanceled = false;
            } else
            {
                isCanceled = true;
            }
            return str;
        }

        static void WalkDirectoryTree(DirectoryInfo root, string mask, List<string> folders)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;
            try
            {
                files = root.GetFiles(mask);
            }
            catch (Exception e)
            {
                //System.Windows.MessageBox.Show(e.ToString());
            }


            if (files != null)
            {
                foreach (FileInfo fi in files)
                {
                    string name = fi.FullName.ToString();
                    if (name.Contains(@"") && (!name.Contains("putin")))
                    {
                        WriteToFile(filename, String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", mask, fi.Name, GetPartsOf(fi.Name).Item1, GetPartsOf(fi.Name).Item2, fi.LastWriteTime.ToString(), GetInitFolder(fi.FullName.ToString(), folders), fi.Directory));
                    }
                }
                try
                {
                    subDirs = root.GetDirectories();
                    foreach (DirectoryInfo dirInfo in subDirs)
                    {
                        WalkDirectoryTree(dirInfo, mask, folders);
                    }
                }
                catch (Exception ex)
                {
                    //System.Windows.MessageBox.Show(e.ToString());
                }

                
            }
        }

        public void WriteListFilesInFile()
        {
            // Get the current application directory.
            string currentDirName = System.IO.Directory.GetCurrentDirectory();
            WriteToFile(filename, currentDirName);

            // Get an array of file names as strings rather than FileInfo objects.
            // Use this method when storage space is an issue, and when you might
            // hold on to the file name reference for a while before you try to access
            // the file.
            string[] files = System.IO.Directory.GetFiles(currentDirName, "*.rvt");

            foreach (string s in files)
            {
                // Create the FileInfo object only when needed to ensure
                // the information is as current as possible.
                System.IO.FileInfo fi = null;
                try
                {
                    fi = new System.IO.FileInfo(s);
                }
                catch (System.IO.FileNotFoundException e)
                {
                    // To inform the user and continue is
                    // sufficient for this demonstration.
                    // Your application may require different behavior.
                    WriteToFile(filename, e.Message);
                    continue;
                }
                WriteToFile(filename, String.Format("{0} : {1}", fi.Name, fi.Directory));
            }
        }

        public static void WriteToFile(string fileName, string txt)
        {

            if (!File.Exists(fileName))
            {
                try
                {
                    using (FileStream fs = File.Create(fileName))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes("");
                        fs.Write(info, 0, info.Length);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(e.ToString());
                }
            }
            using (StreamWriter writer = new StreamWriter(fileName, true))
            {
                writer.WriteLine(txt);
            }

        }

        static (string, string) GetPartsOf(string input)
        {
            string str1 = "";
            string str2 = "";
            var ft = GetFileType(input);
            int indexOf_v = 0;
            string subString = "_v";
            if (input.Contains(subString)) indexOf_v = input.IndexOf(subString);
            if (indexOf_v != 0)
            {
                if (ft.Item1 != FT.UND)
                {
                    string firstPart = input.Substring(0, indexOf_v);
                    int endIndex = input.IndexOf(ft.Item2);
                    str2 = input.Substring(indexOf_v + 1, endIndex - indexOf_v - 1);
                    str1 = firstPart + ft.Item2;
                    return (str1, str2);
                }

            }
            return ("", "");
        }

        static (FT, string) GetFileType(string input)
        {
            if (input.ToLower().EndsWith(".txt")) return (FT.TXT, ".txt");
            if (input.ToLower().EndsWith(".doc")) return (FT.DOC, ".doc");
            if (input.ToLower().EndsWith(".docx")) return (FT.DOCX, ".docx");
            if (input.ToLower().EndsWith(".xls")) return (FT.XLS, ".xls");
            if (input.ToLower().EndsWith(".xlsx")) return (FT.XLSX, ".xls");
            if (input.ToLower().EndsWith(".pdf")) return (FT.PDF, ".pdf");
            if (input.ToLower().EndsWith(".dwg")) return (FT.DWG, ".dwg");
            if (input.ToLower().EndsWith(".rvt")) return (FT.RVT, ".rvt");
            if (input.ToLower().EndsWith(".rfa")) return (FT.RFA, ".rfa");
            if (input.ToLower().EndsWith(".rfa")) return (FT.NWD, ".nwd");
            if (input.ToLower().EndsWith(".rfa")) return (FT.JPG, ".jpg");
            
            return (FT.UND, "");
        }

        static string GetInitFolder(string path, List<string> list)
        {
            foreach (string st in list)
            {
                if (path.Contains(st))
                {
                    return st;
                }
            }
            return "";
        }

        public static string ReadIniFile(string fileName)
        {
            string output = "";
            using (StreamReader reader = new StreamReader(fileName, true))
            {
                output = reader.ReadLine();
            }
            return output;
        }

        enum FT
        {
            UND, TXT, PDF, DOC, DOCX, XLS, XLSX, DWG, RVT, RFA, NWD, JPG
        }
    }
}