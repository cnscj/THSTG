﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class SpriteMenu
    {
        [MenuItem("Assets/Guangyv/精灵菜单/动画&控制器/生成动画及控制器", false, 12)]
        public static void MenuOneKeyAll()
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
                    //处理逻辑
                    SpriteTools.GenerateAnimationClipFromTextureFile(assetPath);
                }
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }

        }
    }
}
