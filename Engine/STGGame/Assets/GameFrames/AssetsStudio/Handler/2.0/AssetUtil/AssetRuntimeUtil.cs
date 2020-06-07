using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public static class AssetRuntimeUtil
    {
        //全局常量
        public static readonly string[] TEXTURE_FILE_SUFFIXS = { "tga", "png", "jpg" };                           //常用图像文件后缀

        public enum AssetPathType
        {
            Unknow,
            Local,
            Url,
            AssetBundle,
        }

        public static void TryCreateFolder(string path)
        {
            if (!XFolderTools.Exists(path))
            {
                XFolderTools.CreateDirectory(path);
            }
        }

        public static AssetPathType GetAssetPathType(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                //Bundle路径
                if (path.IndexOf("|") > 0)
                {
                    return AssetPathType.AssetBundle;
                }
                else
                {
                    //第一个冒号之前的,如果是http,https,ftp等,则为网络协议
                    int index = path.IndexOf(":");
                    if (index > 0)
                    {
                        string protocol = path.Substring(0, index).ToLower();
                        if (protocol == "http" || protocol == "https")
                        {
                            return AssetPathType.Url;
                        }
                        else
                        {
                            return AssetPathType.Local;
                        }
                    }
                    else
                    {
                        return AssetPathType.Local;
                    }
                }
            }
            return AssetPathType.Unknow;
        }

        /// <summary>
        /// 判断是否为图片文件
        /// </summary>
        /// <param name="assetPath">文件路径</param>
        /// <returns>是否</returns>
        public static bool IsImageFile(string assetPath)
        {
            string fileExt = Path.GetExtension(assetPath);
            foreach (var imgExt in TEXTURE_FILE_SUFFIXS)
            {
                if (fileExt.Contains(imgExt))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
