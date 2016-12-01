using System;

using log4net.Layout;

namespace FRXS.Common.Log
{
    public class ReflectionLayout : PatternLayout
    {
        public ReflectionLayout()
        {
            this.AddConverter("property", typeof(ReflectionPatternConverter));
        }
    }
}
