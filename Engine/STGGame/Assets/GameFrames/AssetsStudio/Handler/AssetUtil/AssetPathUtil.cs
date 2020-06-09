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
