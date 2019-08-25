﻿
using System.IO;

namespace THGame
{
    public static class ResourceLoaderUtil 
    {
        public static bool SplitBundlePath(string fullpath, out string abPath, out string assetName)
        {
            if (fullpath != null && fullpath != "")
            {
                string[] strArray = fullpath.Split('|');
                if (strArray.Length >= 1)
                {
                    abPath = strArray[0];
                    assetName = strArray[1];
                    if (assetName == "")
                    {
                        assetName = Path.GetFileNameWithoutExtension(abPath);
                    }
                }
                else
                {
                    abPath = fullpath;
                    assetName = null;
                }

                return true;
            }
            abPath = fullpath;
            assetName = fullpath;

            return false;
        }

        public static string CombineBundlePath(string abPath, string assetName)
        {
            if (assetName == null)
            {
                return string.Format("{0}", abPath);
            }
            return string.Format("{0}|{1}", abPath, assetName);
        }


    }
}
