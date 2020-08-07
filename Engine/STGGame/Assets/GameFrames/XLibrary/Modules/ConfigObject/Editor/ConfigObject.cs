using UnityEngine;
using UnityEditor;
using XLibrary;

namespace XLibEditor
{
    public class ConfigObject<T> : ScriptableObject where T : ScriptableObject, new()
    {
        protected static string _saveFolder = "Assets/Resources";
        protected static string _configAssetsPath = XPathTools.Combine(_saveFolder, string.Format("{0}{1}.asset", typeof(T).Namespace, typeof(T).Name));

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
            if (XFileTools.Exists(_configAssetsPath))
            {
                asset = AssetDatabase.LoadAssetAtPath<T>(_configAssetsPath);
            }
            else
            {
                asset = ScriptableObject.CreateInstance<T>();
                if (!XFolderTools.Exists(_saveFolder))
                {
                    XFolderTools.CreateDirectory(_saveFolder);
                }
                AssetDatabase.CreateAsset(asset, _configAssetsPath);
                AssetDatabase.Refresh();
            }
            return asset;
        }

        //修改加载目录
        protected static string ChangeAssetPath(string path)
        {
            if (path != null && path != "")
            {
                _configAssetsPath = path;
                return path;
            }
            return _configAssetsPath;
        }

    }
}
