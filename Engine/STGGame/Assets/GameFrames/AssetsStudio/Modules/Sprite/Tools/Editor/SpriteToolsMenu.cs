using System.Collections;
using System.Collections.Generic;
using System.IO;
using ASGame;
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

                        if (!AssetRuntimeUtil.IsImageFile(fullPath))
                            return;

                        string assetPath = XPathTools.GetRelativePath(fullPath);
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

                        if (!AssetRuntimeUtil.IsImageFile(fullPath))
                            return;

                        string assetPath = XPathTools.GetRelativePath(fullPath);
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

                        if (!AssetRuntimeUtil.IsImageFile(fullPath))
                            return;

                        string assetPath = XPathTools.GetRelativePath(fullPath);
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

        [MenuItem("Assets/ASEditor/资源工具/精灵菜单/图集/打包图集")]
        public static void GenTexturePackage()
        {
            var selecteds = Selection.objects;
            List<string> texPathList = new List<string>();
            foreach (var selected in selecteds)
            {
                string selectPath = AssetDatabase.GetAssetPath(selected);
                string fileExName = Path.GetExtension(selectPath).ToLower();
                if (fileExName.Contains("png") || fileExName.Contains("jpg") || fileExName.Contains("tga"))
                {
                    texPathList.Add(selectPath);
                }
            }

            List<Texture2D> texList = new List<Texture2D>();
            foreach(var texPath in texPathList)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                texList.Add(texture);
            }

            string firstFile = texPathList[0];
            string parentRoot = Path.GetDirectoryName(firstFile);
            string savePath = Path.Combine(parentRoot, string.Format("altas.png"));
            
            SpriteEditorTools.TexturePackage(texList.ToArray(), savePath);
        }

        [MenuItem("Assets/ASEditor/资源工具/精灵菜单/图集/解包图集")]
        public static void GenTextureUnPackage()
        {
            var selected = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selected);

            var altasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(selectPath);
            SpriteEditorTools.TextureUnpackage(altasTexture);
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

                        if (!AssetRuntimeUtil.IsImageFile(fullPath))
                            return;

                        string assetPath = XPathTools.GetRelativePath(fullPath);
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

        [MenuItem("Assets/ASEditor/资源工具/精灵菜单/动画/用选中项生成动画")]
        public static void MenuGenSelectionAnimaAndController()
        {
            var selection = Selection.objects;
            if (selection != null && selection.Length > 0)
            {
                var target = selection[0];
                string filePath = AssetDatabase.GetAssetPath(target);
                string fileRoot = Path.GetDirectoryName(filePath);
                string fileKey = XStringTools.SplitPathName(target.name);
                string savePath = Path.Combine(fileRoot, string.Format("{0}.anim", fileKey));

                SpriteEditorTools.MakeAnimationClip(selection, 12f, savePath);
            }
            else
            {
                Debug.LogError("没有选中的文件");
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
                        string fleRelaPath = XPathTools.GetRelativePath(fullPath);
                        string fileRootPath = Path.GetDirectoryName(fleRelaPath);

                        exportPathList.Add(fileRootPath);
                    }
                }, true);
            }
            foreach (var exportRootPath in exportPathList)
            {
                string ctrlFilePath = XPathTools.Combine(exportRootPath, SpriteEditorTools.controllerName);

                string folderId = XStringTools.SplitPathKey(exportRootPath);
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
