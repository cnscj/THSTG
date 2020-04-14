﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using XLibrary;
using Object = UnityEngine.Object;


namespace ASGame
{
    //编辑器模式加载器[编辑器模式 同步异步无区别]
    public class EditorAssetLoader : IFileLoader
    {

        public T LoadAsset<T>(string path, string assetName) where T : class
        {
            return load<T>(path);
        }

        public IEnumerator LoadAssetAsync<T>(string path, string assetName, UnityAction<T> callback) where T : class
        {
            if (callback != null) {
                callback(load<T>(path));
            }
            yield return null;
        }


        T load<T>(string path) where T : class
        {
#if UNITY_EDITOR
            path = XPathTools.NormalizePath(path);

            string resPath;
            string assetName;
            ResourceLoaderUtil.SplitBundlePath(path, out resPath, out assetName);

            //绝对路径转为相对Assets文件夹的相对路径
            path = XPathTools.GetRelativePath(resPath);

            Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(resPath, typeof(T));
            if (obj == null)
            {
                Debug.LogError("Asset not found - path:" + resPath);
            }

            return obj as T;
#endif
            return null;
        }
    }

}
