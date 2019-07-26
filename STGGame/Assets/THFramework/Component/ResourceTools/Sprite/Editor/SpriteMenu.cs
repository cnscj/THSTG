using System.Collections;
using System.Collections.Generic;
using System.IO;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class SpriteMenu
    {

        [MenuItem("Assets/Guangyv/精灵菜单/一键生成所有", false, 12)]
        public static void MenuGenSpriteOneKey()
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
                    GenSpriteOneKey(assetPath);

                }
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }
        }

       
        [MenuItem("Assets/Guangyv/精灵菜单/图集/生成精灵图集(DB)")]
        public static void MenuGenAtlasSheets()
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
                    GenSpriteFrames(assetPath);
                }
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }

        }

        [MenuItem("Assets/Guangyv/精灵菜单/图集/生成DB精灵Json数据")]
        public static void MenuGenSheetJson()
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
                    GenSheetJson(assetPath);
                }
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }

        }

        [MenuItem("Assets/Guangyv/精灵菜单/动画/生成动画及控制器")]
        public static void MenuGenAnimaAndController()
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
                    GenAnimaAndCtrler(assetPath);
                }
            }
            else
            {
                Debug.LogError("没有选中文件或文件夹");
            }

        }

        public static void GenSpriteOneKey(string assetPath)
        {
            GenSpriteFrames(assetPath);
            GenAnimaAndCtrler(assetPath);
 
        }

        public static void GenAnimaAndCtrler(string assetPath)
        {
            string selectRootPath = Path.GetDirectoryName(assetPath);
            string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
            //处理逻辑
            var ctrlMap = SpriteEditorTools.GenerateAnimationClipFromTextureFile(assetPath,"",(clip) =>
            {
                bool isLoop = SpriteConfig.IsNeedLoop(clip.name);
                if (isLoop)
                {
                    SpriteEditorTools.SetupAnimationClipLoop(clip, isLoop);
                }
            });
            foreach(var groupPair in ctrlMap)
            {
                foreach(var clipPair in groupPair.Value)
                {
                    var clip = clipPair.Value;
                    string clipFilePath = AssetDatabase.GetAssetPath(clip);
                    string clipRootPath = Path.GetDirectoryName(clipFilePath);

                    string ctrlSavePath = PathUtil.Combine(clipRootPath, SpriteEditorTools.controllerName);
                    var ctrl = SpriteEditorTools.GenerateAnimationControllerFromAnimationClipFile("", ctrlSavePath);

                    bool isDefault = SpriteConfig.isDefaultState(clip.name);
                    SpriteEditorTools.SetupAnimationState(ctrl, clip, isDefault);

                }
                string clipSavePath = PathUtil.Combine(selectRootPath, groupPair.Key);
                string ctrlFilePath = PathUtil.Combine(clipSavePath, SpriteEditorTools.controllerName);
                SpriteEditorTools.GeneratePrefabFromAnimationControllerFile(ctrlFilePath);
            }
        }

        public static void GenSpriteFrames(string assetPath)
        {
            string selectRootPath = Path.GetDirectoryName(assetPath);
            string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
            SpriteEditorTools.SetupSpriteFrameFromDBJsonFile(assetPath);
        }

        public static void GenSheetJson(string assetPath)
        {
            string selectRootPath = Path.GetDirectoryName(assetPath);
            string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
            SpriteEditorTools.GenerateDBJsonFromDBTextureFile(assetPath);
        }
    
    }
}
