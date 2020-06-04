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
    }
}
