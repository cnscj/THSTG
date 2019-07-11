using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class ResourceConfig : ScriptableObject
    {
        //全局常量
        public static readonly string[] textureFileSuffixs = { "tga", "png", "jpg" };                           //常用图像文件后缀

        public static readonly string resourcePath = "Assets/Resources";
        public static readonly string configAssetsPath = PathUtil.Combine(resourcePath,"THResourceConfig.asset");

        private static ResourceConfig s_asset;


        //需要手动设置
        public string srcFolderSprite = "Assets/Asset/Source/Sprite";
        public string srcFolderModel = "Assets/Asset/Source/Models";
        public string srcFolderEffect = "Assets/Asset/Source/Effects";
        public string srcFolderLevel = "Assets/Asset/Source/Level";

        public bool isAutoGenModelPrefab = false;
        public bool isAutoGenSpriteClip = false;

        public static ResourceConfig GetInstance()
        {
            if (!s_asset)
            {
                s_asset = GetOrCreateAsset();
            }
            return s_asset;
        }

        static ResourceConfig GetOrCreateAsset()
        {
            ResourceConfig asset = null;
            if (XFileTools.Exists(configAssetsPath))
            {
                asset = AssetDatabase.LoadAssetAtPath<ResourceConfig>(configAssetsPath);
            }
            else
            {
                asset = ScriptableObject.CreateInstance<ResourceConfig>();
                if (!XFolderTools.Exists(resourcePath))
                {
                    XFolderTools.CreateDirectory(resourcePath);
                }
                AssetDatabase.CreateAsset(asset, configAssetsPath);
                AssetDatabase.Refresh();
            }
            return asset;
        }
    }

}
