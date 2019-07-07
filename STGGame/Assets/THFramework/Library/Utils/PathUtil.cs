using System;
using System.IO;

namespace THGame
{
    public static class PathUtil
    {
       public static string Combine(string p1,string p2)
        {
            string combinePath = Path.Combine(p1, p2);
            return combinePath.Replace("\\", "/");
        }
    }

}

