using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public static class AssetPathUtil
    {
        public static string GetCurPlatformName()
        {
            var platform = Application.platform;
            switch (platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    return "pc";
                case RuntimePlatform.Android:
                    return "android";
                case RuntimePlatform.IPhonePlayer:
                    return "ios";
            }
            return "";
        }


        public static string SpliteBundlePath(string abPath, string assetName)
        {
            return string.Format("{0}|{1}", abPath, assetName);
        }

        public static int SpliteBundlePath(string path, out string abPath, out string assetName)
        {
            abPath = path;
            assetName = null;

            if (string.IsNullOrEmpty(path))
                return -1;

            if (path.IndexOf("|") > 0)
            {
                string[] pathPairs = path.Split('|');
                abPath = pathPairs[0];
                assetName = pathPairs[1];

                return pathPairs.Length;
            }

            return 0;
        }

        public static string TrimInvalidCharacter(string str)
        {
            string newStr = str
                //.Replace("/", "_")
                //.Replace("\\", "_")
                //.Replace(" ", "_")
                // 下面这些符号控制台不认
                .Replace("`", "")
                .Replace("~", "")
                .Replace("!", "")
                .Replace("#", "")
                .Replace("$", "")
                .Replace("%", "")
                .Replace("&", "")
                .Replace("*", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("=", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("、", "")
                .Replace("|", "")
                .Replace(";", "")
                .Replace("\"", "")
                .Replace("\'", "")
                .Replace(",", "")
                .Replace("<", "")
                .Replace(">", "");
            // 上面这些符号控制台不认
            return newStr;
        }
    }
}
