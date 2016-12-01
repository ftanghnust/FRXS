using System;

namespace FRXS.Common.Log
{
    #region 普通日志实体类
    /// <summary>
    /// 普通日志实体类
    /// </summary>
    public class NormalLog
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string LogType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string LogContent { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string LogUserCode { get; set; }

        /// <summary>
        /// 源
        /// </summary>
        public string LogSource { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string LogOperation { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string LogIp { get; set; }
    }
    #endregion
}
