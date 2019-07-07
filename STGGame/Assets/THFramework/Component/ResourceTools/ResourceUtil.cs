using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace THEditor
{
    public static class ResourceUtils
    {
        /// <summary>
        /// 判断是否为图片文件
        /// </summary>
        /// <param name="assetPath">文件路径</param>
        /// <returns>是否</returns>
        public static bool IsImageFile(string assetPath)
        {
            string fileExt = Path.GetExtension(assetPath);
            foreach (var imgExt in ResourceConfig.textureFileSuffixs)
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
