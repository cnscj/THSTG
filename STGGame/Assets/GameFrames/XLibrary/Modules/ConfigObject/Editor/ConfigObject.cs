using UnityEngine;
using UnityEditor;
using XLibrary;

namespace XLibraryEditor
{
    public class ConfigObject<T> : ScriptableObject where T : ScriptableObject, new()
    {
        public static string resourcePath = "Assets/Resources";
        public static string configAssetsPath = PathUtil.Combine(resourcePath, string.Format("{0}.asset",typeof(T).Name));

        private static T s_asset;

        public static T GetInstance()
        {
            if (!s_asset)
            {
                s_asset = GetOrCreateAsset();
            }
            return s_asset;
        }

        static T GetOrCreateAsset()
        {
            T asset = null;
            if (XFileTools.Exists(configAssetsPath))
            {
                asset = AssetDatabase.LoadAssetAtPath<T>(configAssetsPath);
            }
            else
            {
                asset = ScriptableObject.CreateInstance<T>();
                if (!XFolderTools.Exists(resourcePath))
                {
                    XFolderTools.CreateDirectory(resourcePath);
                }
                AssetDatabase.CreateAsset(asset, configAssetsPath);
                AssetDatabase.Refresh();
            }
            return asset;
        }

        //修改加载目录
        protected static string ChangeAssetPath(string path)
        {
            if (path != null && path != "")
            {
                configAssetsPath = path;
                return path;
            }
            return configAssetsPath;
        }

    }
}
