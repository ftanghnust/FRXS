using System;

using log4net;
using System.Web;
using System.IO;
using log4net.Config;

namespace FRXS.Common.Log
{
    #region 封装log4net处理类
    /// <summary>
    /// 封装log4net处理类
    /// </summary>
    public class Logger
    {
        #region 单例对象
        static Logger instance = null;
        static readonly object lockObj = new object();

        Logger()
        {
            string path = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("/Log4Net.config") : (System.AppDomain.CurrentDomain.BaseDirectory + "Log4Net.config");
            if (File.Exists(path))
            {
                XmlConfigurator.Configure(new FileInfo(path));
            }
            else
            {
                XmlConfigurator.Configure();
            }
        }

        public static Logger GetInstance()
        {
            if (instance == null)
            {
                lock (lockObj)
                {
                    instance = new Logger();
                }
            }
            return instance;
        }
        #endregion

        #region 通用方法
        /// <summary>
        /// 调试信息 记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。
        /// </summary>
        /// <param name="message">错误字符串</param>
        /// <param name="exception">异常对象</param>
        public void Debug(object message, Exception exception)
        {
            LogManager.GetLogger("NormalLogger").Debug(message, exception);
        }

        /// <summary>
        /// 调试信息 记录系统用于调试的一切信息，内容或者是一些关键数据内容的输出。
        /// </summary>
        /// <param name="message">错误字符串</param>
        public void Debug(object message)
        {
            LogManager.GetLogger("NormalLogger").Debug(message);
        }

        /// <summary>
        /// 一般信息 记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message">错误字符串</param>
        /// <param name="exception">异常对象</param>
        public void Info(object message, Exception exception)
        {
            LogManager.GetLogger("NormalLogger").Info(message, exception);
        }

        /// <summary>
        /// 一般信息 记录系统运行中应该让用户知道的基本信息。例如，服务开始运行，功能已经开户等。
        /// </summary>
        /// <param name="message">错误字符串</param>
        public void Info(object message)
        {
            LogManager.GetLogger("NormalLogger").Info(message);
        }

        #region 根据配置文件名称加载对象并写入信息
        /// <summary>
        /// 根据配置文件名称加载对象并写入信息
        /// </summary>
        /// <param name="logName">配置文件名称</param>
        /// <param name="model">普通日志实体对象</param>
        public void Info(string logName, NormalLog model)
        {
            LogManager.GetLogger(logName).Info(model);
        }
        #endregion

        /// <summary>
        /// 警告 记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。
        /// </summary>
        /// <param name="message">错误字符串</param>
        /// <param name="exception">异常对象</param>
        public void Warn(object message, Exception exception)
        {
            LogManager.GetLogger("NormalLogger").Warn(message, exception);
        }

        /// <summary>
        /// 警告 记录系统中不影响系统继续运行，但不符合系统运行正常条件，有可能引起系统错误的信息。例如，记录内容为空，数据内容不正确等。
        /// </summary>
        /// <param name="message">错误字符串</param>
        public void Warn(object message)
        {
            LogManager.GetLogger("NormalLogger").Warn(message);
        }

        /// <summary>
        /// 一般错误 记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message">错误字符串</param>
        /// <param name="exception">异常对象</param>
        public void Error(object message, Exception exception)
        {
            LogManager.GetLogger("NormalLogger").Error(message, exception);
        }

        /// <summary>
        /// 一般错误 记录系统中出现的导致系统不稳定，部分功能出现混乱或部分功能失效一类的错误。例如，数据字段为空，数据操作不可完成，操作出现异常等。
        /// </summary>
        /// <param name="message">错误字符串</param>
        public void Error(object message)
        {
            LogManager.GetLogger("NormalLogger").Error(message);
        }

        /// <summary>
        /// 致命错误 记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。
        /// </summary>
        /// <param name="message">错误字符串</param>
        /// <param name="exception">异常对象</param>
        public void Fatal(object message, Exception exception)
        {
            LogManager.GetLogger("NormalLogger").Fatal(message, exception);
        }

        /// <summary>
        /// 致命错误 记录系统中出现的能使用系统完全失去功能，服务停止，系统崩溃等使系统无法继续运行下去的错误。例如，数据库无法连接，系统出现死循环。
        /// </summary>
        /// <param name="message">错误字符串</param>
        public void Fatal(object message)
        {
            LogManager.GetLogger("NormalLogger").Fatal(message);
        }
        #endregion

        #region 调试日志
        /// <summary>
        /// 调试日志记录
        /// </summary>
        /// <param name="model">普通日志实体对象</param>
        public void DebugLog(NormalLog model)
        {
            LogManager.GetLogger("DebugLogger").Info(model);
        }
        #endregion

        #region 异常日志
        /// <summary>
        /// 异常日志记录
        /// </summary>
        /// <param name="model">普通日志实体对象</param>
        public void ExceptionLog(NormalLog model)
        {
            LogManager.GetLogger("ExceptionLogger").Error(model);
        }

        /// <summary>
        /// 异常日志记录
        /// </summary>
        /// <param name="model">普通日志实体对象</param>
        /// <param name="exception">异常对象</param>
        public void ExceptionLog(NormalLog model, Exception exception)
        {
            LogManager.GetLogger("ExceptionLogger").Error(model, exception);
        }

        #endregion

        #region 数据操作日志
        /// <summary>
        /// 数据操作日志记录
        /// </summary>
        /// <param name="model">普通日志实体对象</param>
        public void DBOperatingLog(NormalLog model)
        {
            LogManager.GetLogger("DBOperatingLogger").Error(model);
        }

        /// <summary>
        /// 数据操作日志记录
        /// </summary>
        /// <param name="model">普通日志实体对象</param>
        /// <param name="exception">异常对象</param>
        public void DBOperatingLog(NormalLog model, Exception exception)
        {
            LogManager.GetLogger("DBOperatingLogger").Error(model, exception);
        }

        #endregion

    }
    #endregion
}
