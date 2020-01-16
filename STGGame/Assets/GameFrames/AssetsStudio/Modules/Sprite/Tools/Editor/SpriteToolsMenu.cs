using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public class SpriteToolsMenu
    {

        [MenuItem("Assets/ASEditor/资源工具/精灵菜单/一键生成所有", false, 12)]
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


        [MenuItem("Assets/ASEditor/资源工具/精灵菜单/图集/生成精灵图集(DB)")]
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

        [MenuItem("Assets/ASEditor/资源工具/精灵菜单/图集/生成DB精灵Json数据")]
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

        [MenuItem("Assets/ASEditor/资源工具/精灵菜单/动画/生成动画及控制器")]
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
            List<string> rootList = new List<string>();
            GenSpriteFrames(assetPath);
            GenAnimaAndCtrler(assetPath, rootList);
            GenPrefabAndmaterial(assetPath, rootList);
        }
        public static void GenPrefabAndmaterial(string assetPath, List<string> exportPathList = null)
        {
            if (exportPathList == null)
            {
                exportPathList = new List<string>();
                string selectRootPath = Path.GetDirectoryName(assetPath);
                string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
                XFolderTools.TraverseFiles(selectRootPath, (fullPath) =>
                {
                    string fileEx = Path.GetExtension(fullPath).ToLower();
                    if (fileEx.Contains("controller"))
                    {
                        string fleRelaPath = XFileTools.GetFileRelativePath(fullPath);
                        string fileRootPath = Path.GetDirectoryName(fleRelaPath);

                        exportPathList.Add(fileRootPath);
                    }
                }, true);
            }
            foreach (var exportRootPath in exportPathList)
            {
                string ctrlFilePath = XPathTools.Combine(exportRootPath, SpriteEditorTools.controllerName);

                string folderId = ResourceUtil.GetFolderId(exportRootPath);
                string spriteSavePath = XPathTools.Combine(exportRootPath, string.Format("{0}.prefab", folderId));
                string materialSavePath = XPathTools.Combine(exportRootPath, string.Format("{0}.mat", folderId));
                var sprite = SpriteEditorTools.GeneratePrefabFromAnimationControllerFile(ctrlFilePath, spriteSavePath);
                var material = SpriteEditorTools.GenerateMaterialFromAnimationControllerFile(ctrlFilePath, materialSavePath);

                SpriteEditorTools.SetupMaterial(sprite, material);
                SpriteEditorTools.SetupBoxCollider(sprite);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

        }
        public static void GenAnimaAndCtrler(string assetPath, List<string> exportPathList = null)
        {
            string selectRootPath = Path.GetDirectoryName(assetPath);
            string selectFileName = Path.GetFileNameWithoutExtension(assetPath);
            //处理逻辑
            var ctrlMap = SpriteEditorTools.GenerateAnimationClipFromTextureFile(assetPath, "", (clip) =>
              {
                  bool isLoop = SpriteToolsConfig.GetInstance().IsNeedLoop(clip.name);
                  if (isLoop)
                  {
                      SpriteEditorTools.SetupAnimationClipLoop(clip, isLoop);
                  }
              });
            foreach (var groupPair in ctrlMap)
            {
                foreach (var clipPair in groupPair.Value)
                {
                    var clip = clipPair.Value;
                    string clipFilePath = AssetDatabase.GetAssetPath(clip);
                    string clipRootPath = Path.GetDirectoryName(clipFilePath);

                    bool isDefault = SpriteToolsConfig.GetInstance().IsDefaultState(clip.name);

                    //上层目录检查
                    //如果上层有公共的,直接用公共的
                    //如果上层有模板,生成继承控制器
                    string prevRootPath = XPathTools.GetParentPath(clipRootPath);
                    string parentCtrl = XPathTools.Combine(prevRootPath, SpriteEditorTools.controllerName);
                    string parentCtrlTmpl = XPathTools.Combine(prevRootPath, SpriteEditorTools.controllerTmplName);
                    if (XFileTools.Exists(parentCtrl))
                    {
                        var ctrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(parentCtrl);
                        SpriteEditorTools.SetupAnimationState(ctrl, clip, isDefault);
                    }
                    else if (XFileTools.Exists(parentCtrlTmpl))
                    {
                        string overrideCtrlSavePath = XPathTools.Combine(clipRootPath, SpriteEditorTools.overrideControllerName);
                        var overrideCtrl = SpriteEditorTools.GenerateAnimationOverrideControllerFromAnimationClipFile("", parentCtrlTmpl, overrideCtrlSavePath);
                        SpriteEditorTools.SetupOverrideMotion(overrideCtrl, clip);
                    }
                    else
                    {
                        string ctrlSavePath = XPathTools.Combine(clipRootPath, SpriteEditorTools.controllerName);

                        var ctrl = SpriteEditorTools.GenerateAnimationControllerFromAnimationClipFile("", ctrlSavePath);
                        SpriteEditorTools.SetupAnimationState(ctrl, clip, isDefault);
                    }
                }

                if (exportPathList != null)
                {
                    string groupPath = SpriteEditorTools.GroupName2Path(groupPair.Key);
                    string exportRootPath = XPathTools.Combine(selectRootPath, groupPath);

                    exportPathList.Add(exportRootPath);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
