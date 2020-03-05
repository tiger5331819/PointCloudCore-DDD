using System;
using System.IO;
using System.Reflection;

namespace PointCloudCore.DomainCore
{
    /// <summary>
    /// 框架：异常消息处理器
    /// </summary>
    public class ErrorMessage
    {
        static string logPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/日志文件";
        static StreamWriter streamWriter;
        /// <summary>
        /// 获取错误并且将错误抛出与写入本地
        /// </summary>
        /// <param name="ex">异常</param>
        public static Exception GetError(ErrorException ex)
        {
            //创建日志文件夹
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            streamWriter = new StreamWriter(logPath + "/" + DateTime.Now.ToString("yyyy-MM-dd") + "日志.txt", true);
            streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss     ") + ex.Message);
            Console.WriteLine(ex.Message);
            streamWriter.WriteLine(ex.Source + ":" + ex.TargetSite);
            Console.WriteLine(ex.Source + ":" + ex.TargetSite);
            streamWriter.WriteLine(ex.StackTrace);
            Console.WriteLine(ex.StackTrace);
            streamWriter.Dispose();
            return ex;
        }
    }
    /// <summary>
    /// 框架：异常类
    /// </summary>
    public class ErrorException:Exception
    {
       public ErrorException(string message) : base(message)
       {

       }
    }
}
