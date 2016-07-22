using System;

namespace FRXS.Common
{
    /// <summary>
    /// 结果对象
    /// </summary>
    [Serializable]
    public class ResultData
    {
        /// <summary>
        /// 标记
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
}
