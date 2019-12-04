using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame
{

    public static class PlatformUtil
    {
        public static string GetCurPlatformName()
        {
            var platform = Application.platform;
            switch(platform)
            {
                case RuntimePlatform.WindowsEditor:
                    return "PC";
                case RuntimePlatform.OSXEditor:
                    return "PC";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
            }
            return "";     
        }
    }


}
