using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectTools
{
    public class FilePathViewModel
    {
        public string PathString { get; set; }
        public string Name { get; set; }
        public string ImgSource { get; set; }
        public Thickness LeftMargin { get; set; }
        public FileType FileType { get; set; }
    }
    public enum FileType
    {
        Unknown = 0,
        Project = 1,
        Family = 2,
        Template = 3,
        Folder = 4
    }
}
