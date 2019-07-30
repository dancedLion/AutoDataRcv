using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoUpdate
{
    public class Class1
    {
    }

    public class AUFileInfo
    {
        public int FileId;
        public string FileName;
        public string FileVersion;
        public string FilePath;
        public int FileSize;
    }
    public class AUSSetting
    {
        public int Id;
        public Dictionary<string, string> Settings;
    }
    public class AutoUpdater
    {

    }
}
