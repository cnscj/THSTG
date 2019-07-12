using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public static class ResourceUtil
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

        public static void DisposeSelected(Action<string> execFunc)
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);
            if (selectPath != "")
            {
                List<string> assetsList = new List<string>();
                if (Directory.Exists(selectPath))    //是文件夹
                {
                    //对所有操作
                    XFolderTools.TraverseFiles(selectPath, (fullPath) =>
                    {
                        string fileNameNExt = Path.GetFileNameWithoutExtension(fullPath).ToLower();
                        string fileExt = Path.GetExtension(fullPath).ToLower();

                        //TODO:
                        if (!ResourceUtil.IsImageFile(fullPath))
                            return;

                        string assetPath = XFileTools.GetFileRelativePath(fullPath);
                        assetsList.Add(assetPath);
                    });
                }
                else
                {
                    assetsList.Add(selectPath);
                }
                foreach (var assetPath in assetsList)
                {
                    execFunc(assetPath);
                }
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }
    }
}
