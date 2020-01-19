/*************************
 * 
 * 时间,日期操作类
 * 
 **************************/
using System;

namespace XLibrary
{
    public static class XTimeTools
    {
        public static readonly DateTime TIME_ORIGIN = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        /// <summary>
        /// 获取时间戳Ms
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampMs()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;

        }
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - TIME_ORIGIN;
            return Convert.ToInt64(ts.TotalMilliseconds / 1000);
        }


        public static DateTime GetDateTime()
        {
            return DateTime.UtcNow;
        }

        public static string GetTimeStr()
        {
            return DateTime.UtcNow.ToString();
        }

        ///
        public static long DataTime2TimeStamp(DateTime dataTime)
        {
            TimeSpan ts = dataTime.ToUniversalTime() - TIME_ORIGIN;
            return Convert.ToInt64(ts.TotalMilliseconds / 1000);
        }

        public static DateTime TimeStamp2DataTime(long timeStamp)
        {
            return new DateTime(timeStamp);
        }

        //%Y-%m-%d %H:%M:%S格式转时间戳
        public static long TimeStr2TimeStamp(string timeStr)
        {
            TimeSpan ts = DateTime.Parse(timeStr).ToUniversalTime() - TIME_ORIGIN;
            return Convert.ToInt64(ts.TotalMilliseconds / 1000);
        }

        //%Y-%m-%d %H:%M:%S格式转时间类
        public static DateTime TimeStr2DataTime(string timeStr)
        {
            return Convert.ToDateTime(timeStr);
        }

        public static string DataTime2TimeStr(DateTime dataTime)
        {
            return dataTime.ToString();
        }

        public static string TimeStamp2TimeStr(long timeStamp)
        {
            return new DateTime(timeStamp).ToString();
        }


        //无格式时间202001192212
        public static string GetUnformattedTimeStr()
        {
            DateTime dt = DateTime.UtcNow;
            return string.Format("{0:yyyyMMddHHmmss}", dt);
        }
    }
}