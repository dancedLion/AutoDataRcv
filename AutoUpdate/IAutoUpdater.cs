using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AutoUpdate
{
    public interface IAutoUpdater
    {
        IAutoUpdateClient AUClient { get; set; }
        IAutoUpdateServer AUServer { get; set; }
    }
    public interface IAutoUpdateServer
    {
        List<AUFileInfo> FileInfos { get; set; }
        AUFileInfo GetFileInfo(int fileId);
        List<AUFileInfo> GetAllFileInfos();
    }
    public interface IAutoUpdateClient
    {
        int DownloadFile(AUFileInfo fileInfo);
        int UploadFile(AUFileInfo fileInfo, byte[] fileStream);

        AUFileInfo GetFileInfo(int fileId);
        List<AUFileInfo> GetAllFileInfos();
        List<AUFileInfo> FileInfos { get; set; }
    }
}
