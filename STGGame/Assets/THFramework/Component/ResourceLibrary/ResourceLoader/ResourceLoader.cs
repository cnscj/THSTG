using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Object = UnityEngine.Object;
using THGame.Package;
using System.IO;

namespace THGame
{
    //资源加载器
    public class ResourceLoader : MonoSingleton<ResourceLoader>
    {
        [Header("当前资源加载模式:")]
        public ResourceLoadMode loadMode = ResourceLoadMode.AssetBundler;

        private IFileLoader m_fileLoader = new FileAssetLoader();
        private IMenoryLoader m_menoryLoader = new MenoryAssetLoader();
        private INetworkLoader m_networkLoader = new NetworkAssetLoader();

        public ResourceLoader()
        {

        }

        //设置加载器
        public void SetFileLoader(IFileLoader loader)
        {
            if (loader != null)
            {
                m_fileLoader = loader;
            }
        }
        public void SetMenoryLoader(IMenoryLoader loader)
        {
            if (loader != null)
            {
                m_menoryLoader = loader;
            }
        }
        public void SetNetworkLoader(INetworkLoader loader)
        {
            if (loader != null)
            {
                m_networkLoader = loader;
            }
        }

        ///
        public T LoadFromFile<T>(ResourceLoadParams args) where T : Object
        {
            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(args.GetUID());
            if (info != null)
            {
                info.UpdateTick();
                return info.cacheObj as T;
            }
            else
            {
                var obj = m_fileLoader.LoadAsset<T>(args.resPath, args.assetName);

                //加入缓存
                ResourceLoaderCache.GetInstance().PushCache(args.GetUID(), obj, args.stayTime);

                return obj;
            }
        }
        public ResourceLoadListener<T> LoadFromFileAsync<T>(ResourceLoadParams args) where T : Object
        {
            ResourceLoadListener<T> listener = new ResourceLoadListener<T>();
            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(args.GetUID());
            if (info != null)
            {
                info.UpdateTick();

                listener.CallCompeleted(info.cacheObj as T);
                
            }
            else
            {
                StartCoroutine(m_fileLoader.LoadAssetAsync<T>(args.resPath, args.assetName, (obj) =>
                {
                    //加入缓存
                    ResourceLoaderCache.GetInstance().PushCache(args.GetUID(), obj, args.stayTime);

                    listener.CallCompeleted(obj);

                }));
            }

            return listener;
        }
        public T LoadFromMenory<T>(ResourceLoadParams args) where T : Object
        {
            return m_menoryLoader.LoadAsset<T>(args.resData, args.assetName);
        }
        public ResourceLoadListener<T> LoadFromMenoryAsync<T>(ResourceLoadParams args) where T : Object
        {
            ResourceLoadListener<T> listener = new ResourceLoadListener<T>();
            StartCoroutine(m_menoryLoader.LoadAssetAsync<T>(args.resData, args.assetName, listener.CallCompeleted));
            return listener;
        }
        public T LoadFromWWW<T>(ResourceLoadParams args) where T : Object
        {
            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(args.resPath);
            if (info != null)
            {
                info.UpdateTick();
                return info.cacheObj as T;
            }
            else
            {
                var obj = m_networkLoader.LoadAsset<T>(args.resPath, args.assetName);

                //加入缓存
                ResourceLoaderCache.GetInstance().PushCache(args.resPath, obj, args.stayTime);

                return obj;
            }
        }
        public ResourceLoadListener<T> LoadFromWWWAsync<T>(ResourceLoadParams args) where T : Object
        {
            ResourceLoadListener<T> listener = new ResourceLoadListener<T>();
            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(args.resPath);
            if (info != null)
            {
                info.UpdateTick();

                listener.CallCompeleted(info.cacheObj as T);
                
            }
            else
            {
                StartCoroutine(m_networkLoader.LoadAssetAsync<T>(args.resPath, args.assetName, (obj) =>
                {
                    //加入缓存
                    ResourceLoaderCache.GetInstance().PushCache(args.resPath, obj);

                    listener.CallCompeleted(obj);

                }, listener.CallProgress));
            }
            return listener;
        }

        ////////////////////////
        //同步加载
        public T LoadFromFile<T>(string path) where T : Object
        {
            return LoadFromFile<T>(new ResourceLoadParams(path,typeof(T)));
        }

        //异步加载
        public ResourceLoadListener<T> LoadFromFileAsync<T>(string path) where T : Object
        {
            return LoadFromFileAsync<T>(new ResourceLoadParams(path, typeof(T)));
        }

        public T LoadFromMenory<T>(byte[] binary, string assetName) where T : Object
        {
            return LoadFromMenory<T>(new ResourceLoadParams(binary, assetName));
        }

        public ResourceLoadListener<T> LoadFromMenoryAsync<T>(byte[] binary, string assetName) where T : Object
        {
            return LoadFromMenoryAsync<T>(new ResourceLoadParams(binary, assetName));
        }

        public T LoadFromWWW<T>(string url) where T : Object
        {
            return LoadFromWWW<T>(new ResourceLoadParams(url));
        }

        public ResourceLoadListener<T> LoadFromWWWAsync<T>(string url) where T : Object
        {
            return LoadFromWWWAsync<T>(new ResourceLoadParams(url));
        }
    }

}
