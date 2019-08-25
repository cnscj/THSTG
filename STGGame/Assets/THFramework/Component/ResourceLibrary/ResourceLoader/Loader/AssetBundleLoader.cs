using UnityEngine.Events;
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
            ResourceLoaderUtil.SplitBundlePath(path, out abPath, out assetName);

            Debug.Log("[LoadAsset]: " + path);
            //打的ab包都资源名称和文件名都是小写的
            string assetBundleName = path;

            AssetBundle assetBundle = null;
            Debug.Log("[AssetBundle]加载目标资源: " + path);
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
            

            //加入缓存
            ResourceManager.GetInstance().PushCache(path, obj);

            return obj as T;
        }

        public IEnumerator LoadAssetAsync<T>(string path, UnityAction<T> callback) where T : class
        {
            path = PathUtil.NormalizePath(path);

            string abPath;
            string assetName;
            ResourceLoaderUtil.SplitBundlePath(path, out abPath, out assetName);

            Debug.Log("[LoadAssetAsync]: " + path);

            ////加载目标资源
            AssetBundleCreateRequest createRequest;
            AssetBundle assetBundle = null;
            Debug.Log("[AssetBundle]加载目标资源: " + path);
            createRequest = AssetBundle.LoadFromFileAsync(abPath);
            yield return createRequest;
            if (createRequest.isDone)
            {
                assetBundle = createRequest.assetBundle;
            }
            AssetBundleRequest abr = assetBundle.LoadAssetAsync(assetName, typeof(T));
            yield return abr;
            Object obj = abr.asset;

            //加入缓存
            ResourceManager.GetInstance().PushCache(path, obj);

            callback(obj as T);

            //释放依赖资源
            assetBundle.Unload(false);
        }

    }

}
