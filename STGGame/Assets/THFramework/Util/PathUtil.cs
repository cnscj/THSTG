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

        public static string GetFileRootPath(string assetPath)
        {
            if (Directory.Exists(assetPath))
            {
                return assetPath;
            }
            else
            {
                string fileRootPath = Path.GetDirectoryName(assetPath);
                return fileRootPath;
            }
        }

        public static string GetUniquePath(string savePath)
        {
            string fileNameNotEx = Path.GetFileNameWithoutExtension(savePath);
            string fileEx = Path.GetExtension(savePath);
            string fileRootPath = Path.GetDirectoryName(savePath);

            string finalPath = savePath;
            for(int i = 0; XFileTools.Exists(finalPath);i++)
            {
                finalPath = Combine(fileRootPath, string.Format("{0}({1}){2}", fileNameNotEx, i, fileEx));
            }

            return finalPath;
        }
    }

}

