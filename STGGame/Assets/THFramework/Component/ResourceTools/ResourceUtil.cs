using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEngine;

namespace THGame
{
    public static class ResourceUtil
    {
        //全局常量
        public static readonly string[] textureFileSuffixs = { "tga", "png", "jpg" };                           //常用图像文件后缀

        /// <summary>
        /// 判断是否为图片文件
        /// </summary>
        /// <param name="assetPath">文件路径</param>
        /// <returns>是否</returns>
        public static bool IsImageFile(string assetPath)
        {
            string fileExt = Path.GetExtension(assetPath);
            foreach (var imgExt in textureFileSuffixs)
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
