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

        private IFileLoader m_localLoader = new FileAssetLoader();
        private IMenoryLoader m_menoryLoader = new MenoryAssetLoader();
        private INetworkLoader m_networkLoader = new NetworkAssetLoader();
        //设置加载器
        public void SetLocalLoader(IFileLoader loader)
        {
            if (loader != null)
            {
                m_localLoader = loader;
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

        public ResourceLoader()
        {

        }

        //同步加载
        public T LoadFromFile<T>(string path) where T : Object
        {
            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(path);
            if (info != null)
            {
                info.UpdateTick();
                return info.cacheObj as T;
            }
            else
            {
                var obj = m_localLoader.LoadAsset<T>(path);

                //加入缓存
                ResourceLoaderCache.GetInstance().PushCache(path, obj);

                return obj;
            }
        }

        //异步加载
        public void LoadFromFileAsync<T>(string path, UnityAction<T> onLoadComplate) where T : Object
        {

            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(path);
            if (info != null)
            {
                info.UpdateTick();
                if (onLoadComplate != null)
                {
                    onLoadComplate(info.cacheObj as T);
                }
            }
            else
            {
                StartCoroutine(m_localLoader.LoadAssetAsync<T>(path, (obj)=>
                {
                    //加入缓存
                    ResourceLoaderCache.GetInstance().PushCache(path, obj);

                    onLoadComplate(obj);

                }));
            }
        }

        public T LoadFromMenory<T>(byte[] binary, string assetName) where T : Object
        {
            return m_menoryLoader.LoadAsset<T>(binary, assetName);
        }

        public void LoadFromMenoryAsync<T>(byte[] binary, string assetName ,UnityAction<T> onLoadComplate) where T : Object
        {
            StartCoroutine(m_menoryLoader.LoadAssetAsync<T>(binary, assetName, onLoadComplate));
        }

        public T LoadFromWWW<T>(string path) where T : Object
        {
            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(path);
            if (info != null)
            {
                info.UpdateTick();
                return info.cacheObj as T;
            }
            else
            {
                var obj = m_networkLoader.LoadAsset<T>(path);

                //加入缓存
                ResourceLoaderCache.GetInstance().PushCache(path, obj);

                return obj;
            }
        }

        public void LoadFromWWWAsync<T>(string path, UnityAction<T> onLoadComplate, UnityAction<float> onLoadProgress = null) where T : Object
        {
            ResourceLoaderCacheDataInfo info = ResourceLoaderCache.GetInstance().QueryCache(path);
            if (info != null)
            {
                info.UpdateTick();
                if (onLoadComplate != null)
                {
                    onLoadComplate(info.cacheObj as T);
                }
            }
            else
            {
                StartCoroutine(m_networkLoader.LoadAssetAsync<T>(path, (obj)=>
                {
                    //加入缓存
                    ResourceLoaderCache.GetInstance().PushCache(path, obj);

                    onLoadComplate(obj);

                }, onLoadProgress));
            }
        }
    }

}
