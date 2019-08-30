﻿using UnityEngine.Events;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace THGame
{
    //ab资源加载器
    public class AssetBundleLoader : IFileLoader
    {
        public AssetBundleLoader()
        {
        }

        public T LoadAsset<T>(string path) where T : class
        {
            path = PathUtil.NormalizePath(path);

            string abPath;
            string assetName;
            ResourceLoaderUtil.SplitBundlePath(path, out abPath, out assetName, typeof(T));

            AssetBundle assetBundle = null;
            assetBundle = AssetBundle.LoadFromFile(abPath);

            Object obj;
            if (assetName != null)
            {
                obj = assetBundle.LoadAsset(assetName, typeof(T));

                //释放资源
                assetBundle.Unload(false);
            }
            else
            {
                obj = assetBundle;
            }

            return obj as T;
        }

        public IEnumerator LoadAssetAsync<T>(string path, UnityAction<T> callback) where T : class
        {
            path = PathUtil.NormalizePath(path);

            string abPath;
            string assetName;
            ResourceLoaderUtil.SplitBundlePath(path, out abPath, out assetName, typeof(T));

            ////加载目标资源
            AssetBundleCreateRequest createRequest;
            AssetBundle assetBundle = null;

            createRequest = AssetBundle.LoadFromFileAsync(abPath);
            yield return createRequest;
            if (createRequest.isDone)
            {
                assetBundle = createRequest.assetBundle;
            }
            AssetBundleRequest abr = assetBundle.LoadAssetAsync(assetName, typeof(T));
            yield return abr;
            Object obj = abr.asset;

            callback(obj as T);

            //释放依赖资源
            assetBundle.Unload(false);
        }

    }

}