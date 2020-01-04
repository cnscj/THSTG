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
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static double GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return ts.TotalMilliseconds;
        }
    }
}