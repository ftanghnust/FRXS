using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace FRXS.Common
{
    /// <summary>
    /// Json数据转换封装
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 将时间由"\/Date(1228372204484)\/" 格式转换成 "yyyy-MM-dd HH:mm:ss" 格式的字符串
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string GetDatetimeString(Match m)
        {
            string sRet = "";
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime(); // dt是UniversalTime
            sRet = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return sRet;
        }

        /// <summary>
        /// 将时间由"\/Date(1228372204484)\/" 格式转换成 "yyyy-MM-dd HH:mm:ss" 格式的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string ReplaceDatetimeString(string str)
        {
            //将时间由"\/Date(1228372204484)\/" 格式转换成 "yyyy-MM-dd HH:mm:ss" 格式的字符串
            string sPattern = @"\\/Date\((\d+)\)\\/";
            MatchEvaluator myMatchEvaluator = new MatchEvaluator(GetDatetimeString);
            Regex reg = new Regex(sPattern);
            return reg.Replace(str, myMatchEvaluator);
        }

        /// <summary>
        /// 将时间由 "yyyy-MM-dd HH:mm:ss" 格式的字符串转换成"\/Date(1228372204484)\/" 格式
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string GetDatetimeJson(Match m)
        {
            string sRet = "";
            DateTime dt = DateTime.Parse(m.Groups[1].Value);
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            sRet = string.Format("\\/Date({0})\\/", ts.TotalMilliseconds);
            return sRet;
        }

        /// <summary>
        /// 转化为JSON格式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //解决json对象过长时无法序列化问题
            jsonSerializer.MaxJsonLength = int.MaxValue;
            string sRet = jsonSerializer.Serialize(obj);
            return ReplaceDatetimeString(sRet); ;
        }

        /// <summary>
        /// 转化为JSON格式
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(this T obj) where T : class
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            //解决json对象过长时无法序列化问题
            jsonSerializer.MaxJsonLength = int.MaxValue;
            var str = jsonSerializer.Serialize(obj);
            return ReplaceDatetimeString(str); ;
        }

        /// <summary>
        /// 转化为JSON格式
        /// </summary>
        /// <param name="objArr"></param>
        /// <returns></returns>
        public static string ToJsonString<T>(this T[] objArr) where T : class
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            var str = String.Format("[{0}]", String.Join(",", Array.ConvertAll(objArr, s => { return jsonSerializer.Serialize(s); })));
            return ReplaceDatetimeString(str); ;
        }

        /// <summary>
        /// 转化为JSON格式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="depth">RecursionLimit</param>
        /// <returns></returns>
        public static string ToJson(this object obj, int depth)
        {
            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            jsonSerializer.RecursionLimit = depth;
            string sRet = jsonSerializer.Serialize(obj);

            return ReplaceDatetimeString(sRet); ;
        }

        /// <summary>
        /// 将DataTable转化为JSON格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this System.Data.DataTable data)
        {
            System.Text.StringBuilder sb = new StringBuilder(256);
            sb.Append("[");
            for (int i = 0; i < data.Rows.Count; ++i)
            {
                System.Data.DataRow row = data.Rows[i];
                if (i > 0) sb.Append(",");

                sb.Append("{");
                for (int k = 0; k < data.Columns.Count; ++k)
                {
                    System.Data.DataColumn column = data.Columns[k];
                    if (k > 0) sb.Append(",");

                    sb.AppendFormat("\"{0}\"", column.ColumnName);
                    sb.Append(":");
                    sb.Append(row[column.ColumnName].ToJson());
                }
                sb.Append("}");
            }
            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// 将JSON反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sJasonData"></param>
        /// <returns></returns>
        public static T FromJson<T>(string sJasonData)
        {
            //将时间由 "yyyy-MM-dd HH:mm:ss" 格式的字符串转换成"\/Date(1228372204484)\/" 格式
            string sPattern = @"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2})";
            MatchEvaluator myMatchEvaluator = new MatchEvaluator(GetDatetimeJson);
            Regex reg = new Regex(sPattern);
            string src = reg.Replace(sJasonData, myMatchEvaluator);

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            T obj = jsonSerializer.Deserialize<T>(src);
            return obj;
        }

        #region By Bati

        /// <summary>
        /// 将DataTable转换成Json格式字符串的方法
        /// </summary>
        /// <param name="table">传入的DataTable</param>
        /// <returns>Json格式的字符串</returns>
        public static string GetJsonArr(DataTable table)
        {
            //构建Json对象数组的开始标示
            StringBuilder data = new StringBuilder("[");

            //遍历DataRow
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //标示单个Json实体的开始
                data.Append("{");

                //遍历DataColumn
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    //将DataTable中各列的列名及值追加到data上
                    data.Append("\"" + table.Columns[j].ColumnName + "\":\"" + table.Rows[i][j] + "\"");

                    //如果不是每一行的最后一个列，则添加,
                    if (j < (table.Columns.Count - 1))
                    {
                        data.Append(",");
                    }
                }

                //标示单个Json实体的结束
                data.Append("}");

                //如果不是DataTable中的最后一行，则添加,
                if (i < (table.Rows.Count - 1))
                {
                    data.Append(",");
                }
            }

            //最后添加]，即Json数组构建完成
            data.Append("]");

            //返回Json字符串
            return data.ToString();
        }

        /// <summary>
        /// 将泛型集合转换成Json格式字符串的泛型方法
        /// </summary>
        /// <typeparam name="T">泛型集合的类型</typeparam>
        /// <param name="list">传入的泛型集合</param>
        /// <returns>Json格式的字符串</returns>
        public static string GetJsonArr<T>(IList<T> list)
        {
            //构建Json对象数组的开始标示
            StringBuilder data = new StringBuilder("[");

            if (list.Count > 0)
            {
                //通过反射获取传入的集合的对象的所有公有属性，存入属性数组
                System.Reflection.PropertyInfo[] array = (list[0].GetType()).GetProperties();

                //遍历集合
                for (int i = 0; i < list.Count; i++)
                {
                    //标示单个Json实体的开始
                    data.Append("{");

                    //遍历属性数组
                    for (int j = 0; j < array.Length; j++)
                    {
                        object propertyName = (list[i].GetType()).GetProperties()[j].GetValue(list[i], null);
                        propertyName = propertyName == null ? "" : propertyName.ToString();
                        //将属性数组中元素名及值追加到data上
                        data.Append("\"" + array[j].Name + "\":\"" + propertyName + "\"");

                        //如果不是每一行的最后一个列，则添加,
                        if (j < (array.Length - 1))
                        {
                            data.Append(",");
                        }
                    }

                    //标示单个Json实体的结束
                    data.Append("}");

                    //如果不是对象集合中的最后一行，则添加,
                    if (i < (list.Count - 1))
                    {
                        data.Append(",");
                    }
                }
            }

            //最后添加]，即Json数组构建完成
            data.Append("]");

            //返回Json字符串
            return data.ToString();
        }

        /// <summary>
        /// 将Json字符串反序列化为对象集合
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="jsonStr">json字符串</param>
        /// <returns>实体集合</returns>
        public static IList<T> GetObjectIList<T>(string jsonStr) where T : class, new()
        {
            IList<T> list = new List<T>();
            if (jsonStr != null)
            {
                if (jsonStr.Trim() != "")
                {
                    //将时间由 "yyyy-MM-dd HH:mm:ss" 格式的字符串转换成"\/Date(1228372204484)\/" 格式
                    string sPattern = @"(\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2})";
                    MatchEvaluator myMatchEvaluator = new MatchEvaluator(GetDatetimeJson);
                    Regex reg = new Regex(sPattern);
                    string src = reg.Replace(jsonStr, myMatchEvaluator);
                    //反序列化对象为数组
                    DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(IList<T>));
                    byte[] b = System.Text.Encoding.UTF8.GetBytes(src);
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(b))
                    {
                        ms.Position = 0;
                        list = dcjs.ReadObject(ms) as IList<T>;
                    }
                }
            }
            IList<T> list1 = new List<T>();
            (list1 as List<T>).AddRange(list);
            return list1;
        }

        public static string GetJsonStr<T>(T t) where T : class,new()
        {
            string jsonStr = "[]";
            if (t != null)
            {
                DataContractJsonSerializer json = new DataContractJsonSerializer(t.GetType());
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {

                    json.WriteObject(ms, t);

                    jsonStr = Encoding.UTF8.GetString(ms.ToArray());

                }
            }
            return jsonStr;
        }

        #endregion
    }
}