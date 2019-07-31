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
        /// <summary>
        /// 服务器端文件信息表
        /// </summary>
        List<AUFileInfo> FileInfos { get; set; }
        /// <summary>
        /// 服务器状态
        /// </summary>
        int ServerStatus { get; set; }
        /// <summary>
        /// 获取指定ID的文件信息
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        AUFileInfo GetFileInfo(int fileId);
        /// <summary>
        /// 获取所有服务器端文件信息
        /// </summary>
        /// <returns></returns>
        List<AUFileInfo> GetAllFileInfos();
        /// <summary>
        /// 获取服务器端指定ID的文件
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns>字节流</returns>
        byte[] PrepareFile(int fileId);
        /// <summary>
        /// 设置服务器文件，保存上传的文件
        /// </summary>
        /// <param name="info">文件信息</param>
        /// <param name="fs">字节流</param>
        /// <returns></returns>
        int SetServerFile(AUFileInfo info, byte[] fs);
        /// <summary>
        /// 加载设置
        /// 写服务器状态
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        int LoadingSetting(int serverId);
    }
    public interface IAutoUpdateClient
    {
        /// <summary>
        /// 自动更新
        /// </summary>
        /// <returns></returns>
        int AutoRunUpdate();
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileInfo">服务器文件信息</param>
        /// <returns></returns>
        int DownloadFile(AUFileInfo fileInfo);
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileInfo">文件信息</param>
        /// <param name="fileStream">字节流</param>
        /// <returns></returns>
        int UploadFile(AUFileInfo fileInfo, byte[] fileStream);
        /// <summary>
        /// 获取需要更新的文件清单
        /// </summary>
        /// <returns></returns>
        List<AUFileInfo> GetDownloadList();
        /// <summary>
        /// 自动更新服务器
        /// </summary>
        IAutoUpdateServer AUServer { get; set; }
        /// <summary>
        /// 获取客户端指定ID的文件信息
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        AUFileInfo GetFileInfo(int fileId);
        /// <summary>
        /// 获取客户端所有的文件信息
        /// </summary>
        /// <returns></returns>
        List<AUFileInfo> GetAllFileInfos();
        /// <summary>
        /// 客户端文件信息列表
        /// </summary>
        List<AUFileInfo> FileInfos { get; set; }
    }
}
