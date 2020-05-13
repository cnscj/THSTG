/*****************
* Lua端数据缓存器
* Lua端更新完或者服务器重启断线后，一般需要重启游戏，同时会重建新的Lua虚拟机，
* 为方便保存部分有用的数据，以便在重启完之后能再次拿回这些数据做对应的逻辑处理，
* 以前游戏的做法是比较低效的读写本地文件
*****************/
using System.Collections.Generic;

namespace SEGame
{
	public static class LuaDataCache
    {
        private static Dictionary<string, string> s_Dict = new Dictionary<string, string>();

        public static void Clear()
        {
            s_Dict.Clear();
        }

        public static string GetValueByKey(string key)
        {
			s_Dict.TryGetValue(key, out string ret);
			return ret;
        }

        public static void SetValueByKey(string key, string value)
        {
            s_Dict[key] = value;
        }

        public static void RemoveValueByKey(string key)
        {
            s_Dict.Remove(key);
        }
    }
}