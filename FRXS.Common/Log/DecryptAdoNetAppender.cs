using System;

using log4net.Appender;
using System.Configuration;

namespace FRXS.Common.Log
{
    /// <summary>
    /// log4net连接字符串解密
    /// </summary>
    public class DecryptAdoNetAppender : AdoNetAppender
    {
        ///// <summary>
        ///// 此方法只能使用log4net1.2.13才能重写，但是当前Memcached使用的log4net版本为1.2.10，无法向后兼容，只能屏蔽该方法
        ///// </summary>
        ///// <param name="connectionType"></param>
        ///// <param name="connectionString"></param>
        ///// <returns></returns>
        //protected override System.Data.IDbConnection CreateConnection(Type connectionType, string connectionString)
        //{
        //    //TODO 补充加密算法
        //    base.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        //    connectionString = "";// DESHelper.Decrypt(base.ConnectionString, "hpdai168");
        //    return base.CreateConnection(connectionType, connectionString);
        //}
    }
}
