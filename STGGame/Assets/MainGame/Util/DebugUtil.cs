﻿#define STG_LOG

using System.Diagnostics;
using System.Text;
using XLibGame.Debug;

namespace STGGame
{
    public static class DebugUtil
    {
        public static int MY_FLAG = 15;

        //[Conditional("STG_LOG")]
        public static void Format(int flag, string format, params object[] args)
        {
            if (flag <= 0 || flag == MY_FLAG)
            {
                Consular.GetInstance().Format(format, args);
            }
        }

        //[Conditional("STG_LOG")]
        public static void Print(int flag, params object[] objs)
        {
            if (flag <= 0 || flag == MY_FLAG)
            {
                Consular.GetInstance().Print(objs);
            }
        }

        public static void Dump(int flag, object obj)
        {
            if (flag <= 0 || flag == MY_FLAG)
            {
                Consular.GetInstance().Dump(obj); 
            }
        }
    }
}

