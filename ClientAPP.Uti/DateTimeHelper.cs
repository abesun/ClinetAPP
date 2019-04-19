using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAPP.Uti
{
    /// <summary>
    /// 日期时间相关
    /// </summary>
    public class DateTimeHelper
    {
        /// <summary>
        /// Datetime转CTime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int DateTimeToCTime(DateTime time)
        {
            return (int)(time - DateTime.Parse("1970-01-01 08:00:00")).TotalSeconds;
        }

        /// <summary>
        /// CTime转Datetime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime CTimeToDateTime(int time)
        {
            return DateTime.Parse("1970-01-01 08:00:00").AddSeconds(time);
        }
    }
}
