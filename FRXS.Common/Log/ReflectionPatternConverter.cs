using System;

using log4net.Layout.Pattern;
using log4net.Core;
using System.IO;
using System.Reflection;

namespace FRXS.Common.Log
{
    public class ReflectionPatternConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {

            if (Option != null)
            {
                // 写入指定键的值
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // 写入所有关键值对
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
        }

        /// <summary>
        /// 通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private object LookupProperty(string property, LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            
            PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (propertyInfo != null)
            {
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            }
            //如果为枚举值，则转换为int
            if (propertyInfo.PropertyType.BaseType == typeof(Enum))
            {
                propertyValue = (int)propertyValue;
            }
            return propertyValue;
        }
    }
}
