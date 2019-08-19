using System;
using System.IO;

namespace THGame
{
    public static class PathUtil
    {
        public static string NormalizePath(string path)
        {
            return path.Replace(@"\", "/");
        }

        public static string Combine(params string[] path)
        {
            string combinePath = Path.Combine(path);
            return NormalizePath(combinePath);
        }

        //取根目录
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

        //相对某目录的相对路径
        public static string GetRelativePath(string relaPath,string filePath)
        {
            relaPath = NormalizePath(relaPath);
            filePath = NormalizePath(filePath);
            int startPos = filePath.IndexOf(relaPath, StringComparison.Ordinal);
            if (startPos >= 0)
            {
                return filePath.Substring(startPos + relaPath.Length);
            }
            return filePath;
        }

        //获取上层目录
        public static string GetParentPath(string curPath)
        {
            curPath = NormalizePath(curPath);
            int lastIndex = curPath.LastIndexOf("/", System.StringComparison.Ordinal);
            if (lastIndex >= 0)
            {
                string lastPath = curPath.Substring(0, lastIndex);
                return lastPath;
            }
            else
            {
                return "/";
            }
        }

        //遍历并获取一个唯一路径
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

