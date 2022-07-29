using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Core.Helpers
{
    public static class LocalTime
    {
        public static DateTime Now()
        {
            var info = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset localTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            return localTime.DateTime;
        }
    }
}
