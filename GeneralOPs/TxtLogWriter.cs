using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
namespace GeneralOPs
{
    public class TxtLogWriter
    {
        public static string DefaultLogFilename = AppDomain.CurrentDomain.BaseDirectory + "logs\\program.log";
        public static string DefaultErrorFilename = AppDomain.CurrentDomain.BaseDirectory + "logs\\error.log";
        /// <summary>
        /// 如果指定文件不存在就创建文件
        /// 注意：目录必须是logs
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static bool isFileExists(string filename)
        {
            try
            {
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "logs");
                }
                if (!File.Exists(filename))
                {
                    File.Create(filename).Close();  //文件创建后默认是打开的，一定要关闭
                }
                //如果文件大于50M
                FileInfo fileinfo = new FileInfo(filename);
                if (fileinfo.Length > 10 * 1024 * 1024)
                {
                    File.Move(filename,
                        AppDomain.CurrentDomain.BaseDirectory + "logs\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + "log.txt");
                    //然后创建:
                    if (!File.Exists(filename))
                    {
                        File.Create(filename).Close();  //文件创建后默认是打开的，一定要关闭
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 写入默认文件
        /// </summary>
        /// <param name="message"></param>
        public static void WriteMessage(string message)
        {
            WriteMessage(DefaultLogFilename, message);
        }
        public static void WriteErrorMessage(string message)
        {
            WriteMessage(DefaultErrorFilename, message);
        }
        /// <summary>
        /// 写入指定文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="message"></param>
        public static void WriteMessage(string filename, string message)
        {
            try
            {
                if (isFileExists(filename))
                {
                    FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write);
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(DateTime.Now.ToString() + "::" + message + "\r\n");
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
            catch
            {
            }
        }
    }
}
