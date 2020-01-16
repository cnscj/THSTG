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
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds/1000);
        }
    }
}