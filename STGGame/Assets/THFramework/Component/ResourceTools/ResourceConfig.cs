using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class ResourceConfig : ScriptableObject
    {
        [System.Serializable]
        public class ReourcesConfigInfos
        {
            public string resName;
            public string editorFolder;
            public bool isAutoProcess;
        }
        //全局常量
        public static readonly string[] textureFileSuffixs = { "tga", "png", "jpg" };                           //常用图像文件后缀

        public static readonly string resourcePath = "Assets/Resources";
        public static readonly string configAssetsPath = PathUtil.Combine(resourcePath,"THResourceConfig.asset");

        private static ResourceConfig s_asset;


        //需要手动设置
        public List<ReourcesConfigInfos> editorResList = new List<ReourcesConfigInfos>();
        private Dictionary<string,ReourcesConfigInfos> m_editorInfoMap;

        public static ResourceConfig GetInstance()
        {
            if (!s_asset)
            {
                s_asset = GetOrCreateAsset();
            }
            return s_asset;
        }

        public ReourcesConfigInfos GetResourceInfos(string key)
        {
            var maps = GetInfosMap();
            ReourcesConfigInfos outInfos;
            maps.TryGetValue(key, out outInfos);
            return outInfos;
        }

        Dictionary<string, ReourcesConfigInfos> GetInfosMap()
        {
            if (m_editorInfoMap == null)
            {
                m_editorInfoMap = new Dictionary<string, ReourcesConfigInfos>();
            }
            {
                m_editorInfoMap.Clear();

                foreach(var item in editorResList)
                {
                    if (item.resName != "")
                    {
                        m_editorInfoMap.Add(item.resName, item);
                    }
                }
            }
            return m_editorInfoMap;
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
