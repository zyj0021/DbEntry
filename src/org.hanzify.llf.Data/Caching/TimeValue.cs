
using System;
using System.Collections.Generic;
using System.Text;
using Lephone.Data.Common;
using Lephone.Util;

namespace Lephone.Data.Caching
{
    public class TimeValue
    {
        public DateTime ExpiredOn;
        public object Value;

        public TimeValue(DateTime ExpiredOn, object Value)
        {
            this.ExpiredOn = ExpiredOn;
            this.Value = Value;
        }

        public static DateTime GetExpiredOn()
        {
            return NowProvider.Instance.Now.AddMinutes(DataSetting.CacheMinutes);
        }

        public static TimeValue CreateTimeValue(object Value)
        {
            return new TimeValue(GetExpiredOn(), Value);
        }
    }
}
