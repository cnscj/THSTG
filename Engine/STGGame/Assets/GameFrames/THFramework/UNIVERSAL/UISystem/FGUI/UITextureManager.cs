
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame.UI
{
    //TODO:同时包含动态合并图集功能
    //异步加载图片资源或者预加载资源,一般一张图片一个AB包
    //离散释放,如果一次加载太多又同时释放会卡
    public class UITextureManager : MonoSingleton<UITextureManager>
    {
        public class TextureCache : MonoBehaviour
        {
            public class TextureInfo
            {
                public string key;
                public int usedTimes;
                public bool isResident;
                public float lastVisitTime;
                public bool isAddByManager;

                public string path;
                public Action<TextureInfo> onRelease;

                private float m_disposeTime;    //释放时间

                public Texture2D texture;

                public void UpdateVisitTime()
                {
                    m_disposeTime = -1f;
                    lastVisitTime = Time.realtimeSinceStartup;
                }


                public void Retain()
                {
                    usedTimes++;
                }

                public void Release()
                {
                    usedTimes--;
                    usedTimes = Math.Max(0, usedTimes);
                }

                public bool CheckDispose()
                {
                    if (isResident)
                        return false;

                    if (m_disposeTime > 0f)
                    {
                        if (Time.realtimeSinceStartup >= m_disposeTime)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        var newUsedTimes = usedTimes;
                        if (isAddByManager) newUsedTimes--; //1是Cache的引用
                        if (newUsedTimes <= 0)
                        {
                            var timeMs = UnityEngine.Random.Range(1, 6000);
                            m_disposeTime = Time.realtimeSinceStartup + timeMs / 1000;
                        }
                    }

                    return false;
                }

                public void Dispose()
                {
                    onRelease?.Invoke(this);
                }

            }

            public float checkFrequence = 1f;
            private Dictionary<string, TextureInfo> m_texturesDict;
            private Queue<string> m_releaseQueue;
            private float m_lastCheckTic;

            public TextureInfo Add(string key, Texture2D texture, bool isReplace = false)
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                if (texture == null)
                    return null;

                var dict = GetTextureDict();
                if (dict.ContainsKey(key))
                {
                    if (!isReplace)
                    {
                        return null;
                    }
                }

                var textureInfo = new TextureInfo();
                textureInfo.key = key;
                textureInfo.texture = texture;
                textureInfo.UpdateVisitTime();
                dict[key] = textureInfo;

                return textureInfo;
            }

            public TextureInfo GetTextureInfo(string key)
            {
                if (string.IsNullOrEmpty(key))
                    return null;

                if (m_texturesDict == null)
                    return null;

                if (!m_texturesDict.ContainsKey(key))
                    return null;

                var textureInfo = m_texturesDict[key];
                textureInfo.UpdateVisitTime();
                return textureInfo;
            }

            public Texture2D Get(string key)
            {
                var textureInfo = GetTextureInfo(key);
                if (textureInfo == null)
                    return default;

                textureInfo.Retain();

                return textureInfo.texture;
            }

            public void Release(string key)
            {
                var textureInfo = GetTextureInfo(key);
                if (textureInfo == null)
                    return;

                textureInfo.Release();
            }

            public bool Remove(string key)
            {
                if (string.IsNullOrEmpty(key))
                    return false;

                if (m_texturesDict == null)
                    return false;

                return m_texturesDict.Remove(key);
            }

            public void Dispose(string key)
            {
                if (string.IsNullOrEmpty(key))
                    return;

                if (m_texturesDict == null)
                    return;

                var releaseQueue = GetReleaseQueue();
                releaseQueue.Enqueue(key);
            }

            public void Clear()
            {
                if (m_texturesDict == null)
                    return;

                foreach (var key in m_texturesDict.Keys)
                {
                    var releaseQueue = GetReleaseQueue();
                    releaseQueue.Enqueue(key);
                }
            }

            private void Update()
            {
                UpdateCheck();
                UpdateRemove();
            }

            private void UpdateCheck()
            {
                if (checkFrequence < 0)
                    return;

                if (Time.realtimeSinceStartup - m_lastCheckTic < checkFrequence)
                    return;

                if (m_texturesDict == null || m_texturesDict.Count <= 0)
                    return;

                foreach(var dictPair in m_texturesDict)
                {
                    var key = dictPair.Key;
                    var textureInfo = dictPair.Value;
                    if (textureInfo.CheckDispose())
                    {
                        var releaseQueue = GetReleaseQueue();
                        releaseQueue.Enqueue(key);
                    }
                }

                m_lastCheckTic = Time.realtimeSinceStartup;
            }

            private void UpdateRemove()
            {
                if (m_releaseQueue == null)
                    return;

                if (m_texturesDict == null || m_texturesDict.Count <= 0)
                    return;

                while (m_releaseQueue.Count > 0)
                {
                    var key = m_releaseQueue.Dequeue();
                    var textureInfo = GetTextureInfo(key);
                    if (textureInfo == null)
                        continue;

                    textureInfo.Dispose();

                    m_texturesDict.Remove(key);
                }
            }

            private Dictionary<string, TextureInfo> GetTextureDict()
            {
                m_texturesDict = m_texturesDict ?? new Dictionary<string, TextureInfo>();
                return m_texturesDict;
            }

            private Queue<string> GetReleaseQueue()
            {
                m_releaseQueue = m_releaseQueue ?? new Queue<string>();
                return m_releaseQueue;
            }
        }

        //AB包计数支持释放
        //不支持依赖加载
        public class BundleManager : MonoBehaviour
        {
            public class BundleInfo
            {
                public AssetBundle assetBundle;
                public HashSet<string> dependencies;

                public void Add(string assetName)
                {
                    var dict = GetDependencies();
                    if (!dict.Contains(assetName))
                        dict.Add(assetName);

                }

                public void Remove(string assetName)
                {
                    if (dependencies == null)
                        return;

                    dependencies.Remove(assetName);
                }

                public bool IsNotDepends()
                {
                    if (dependencies == null)
                        return true;

                    return dependencies.Count <= 0;
                }

                private HashSet<string> GetDependencies()
                {
                    dependencies = dependencies ?? new HashSet<string>();
                    return dependencies;
                }


            }
            public delegate void LoadCallback(UnityEngine.Object obj);
            public class LoadingInfo
            {
                public Dictionary<string, LoadCallback> loadDict;

                public void Add(string assetName, LoadCallback callback)
                {
                    var dict = GetLoadDict();
                    if(dict.ContainsKey(assetName))
                    {
                        dict[assetName] += callback;
                    }
                    else
                    {
                        dict.Add(assetName,callback);
                    }
                }

                private Dictionary<string, LoadCallback> GetLoadDict()
                {
                    loadDict = loadDict ?? new Dictionary<string, LoadCallback>();
                    return loadDict;
                }
            }

            private Dictionary<string, BundleInfo> m_loadedBundle;
            private Dictionary<string, LoadingInfo> m_loadingBundle;

            public T LoadSync<T>(string abPath, string assetName) where T : UnityEngine.Object
            {
                if (string.IsNullOrEmpty(abPath))
                    return default;

                return OnSyncLoad<T>(abPath, assetName);
            }

            public void LoadAsync<T>(string abPath, string assetName, Action<T> action) where T : UnityEngine.Object
            {
                if (string.IsNullOrEmpty(abPath))
                    return;

                
                if (TryGetLoadingBundle(abPath, out var loadingInfo))
                {
                    //回调合并
                    loadingInfo.Add(assetName,(obj) => { action?.Invoke(obj as T); });
                    return;
                }

                loadingInfo = new LoadingInfo();
                loadingInfo.Add(assetName, (obj) => { action?.Invoke(obj as T); });
                GetLoadingBundleDict().Add(abPath, loadingInfo);

                StartCoroutine(OnAsyncLoad<T>(abPath, assetName));
            }

            public void Unload(string abPath)
            {
                if (string.IsNullOrEmpty(abPath))
                    return;

                if (TryGetLoadedBundle(abPath, out var bundleInfo))
                {
                    var ab = bundleInfo.assetBundle;
                    ab.Unload(false);   //有的纹理可能加载了,但是没受池管理,不理它

                    Debug.LogFormat("[TextureManager]The AssetBundle '{0}' had been unload!", abPath);
                    GetLoadedBundleDict().Remove(abPath);
                }
            }

            //只有没有依赖了,才开始释放Ab
            public void TryUnload(string abPath)
            {
                if (string.IsNullOrEmpty(abPath))
                    return;

                if (TryGetLoadedBundle(abPath, out var bundleInfo))
                {
                    if (bundleInfo.IsNotDepends())
                    {
                        Unload(abPath);
                    }
                }
            }

            public void AddDepend(string abPath, string assetName)
            {
                if (string.IsNullOrEmpty(abPath))
                    return;

                if (TryGetLoadedBundle(abPath, out var bundleInfo))
                {
                    bundleInfo.Add(assetName);
                }
            }

            public void RemoveDepend(string abPath, string assetName)
            {
                if (string.IsNullOrEmpty(abPath))
                    return;

                if (TryGetLoadedBundle(abPath, out var bundleInfo))
                {
                    bundleInfo.Remove(assetName);
                }
            }

            private T OnSyncLoad<T>(string abPath, string assetName) where T : UnityEngine.Object
            {

                AssetBundle ab = null;
                if (TryGetLoadedBundle(abPath,out var bundleInfo))
                {
                    ab = bundleInfo.assetBundle;
                }
                else
                {
                    ab = AssetBundle.LoadFromFile(abPath);
                    RecordBundle(abPath, ab);//对这个AB包保持引用
                }

                if (ab != null)
                {
                    var asset = ab.LoadAsset<T>(assetName);
                    if (asset != null)
                    {
                        return asset;
                    }
                }
                return default;
            }

            private IEnumerator OnAsyncLoad<T>(string abPath, string assetName) where T : UnityEngine.Object
            {
                AssetBundle ab = null;
                if (TryGetLoadedBundle(abPath, out var bundleInfo))
                {
                    ab = bundleInfo.assetBundle;
                }
                else
                {
                    var abRequest = AssetBundle.LoadFromFileAsync(abPath);
                    yield return abRequest;

                    ab = abRequest.assetBundle;
                    RecordBundle(abPath, ab);//对这个AB包保持引用
                }

                if (ab != null)
                {
                    if (TryGetLoadingBundle(abPath, out var loadingInfo))
                    {
                        GetLoadingBundleDict().Remove(abPath);  //防止在遍历中对Dict操作

                        if (loadingInfo.loadDict != null)
                        {
                            foreach (var pair in loadingInfo.loadDict)
                            {
                                var callName = pair.Key;
                                var callback = pair.Value;
                                var assetRequest = ab.LoadAssetAsync<T>(assetName);
                                yield return assetRequest;

                                var asset = assetRequest.asset as T;
                                if (asset != null)
                                {
                                    callback?.Invoke(asset);
                                }
                            }
                        }
                    }
                }
            }

            private bool RecordBundle(string key, AssetBundle assetBundle)
            {
                if (string.IsNullOrEmpty(key))
                    return false;

                if (assetBundle == null)
                    return false;

                var bundleDict = GetLoadedBundleDict();
                if (bundleDict.ContainsKey(key))
                    return false;

                var bundleInfo = new BundleInfo();
                bundleInfo.assetBundle = assetBundle;

                bundleDict[key] = bundleInfo;
                return true;
            }

            private bool TryGetLoadedBundle(string key, out BundleInfo bundleInfo)
            {
                bundleInfo = null;
                if (m_loadedBundle == null)
                    return false;

                return m_loadedBundle.TryGetValue(key, out bundleInfo);
            }

            private bool TryGetLoadingBundle(string abPath, out LoadingInfo loadingInfo)
            {
                loadingInfo = null;
                if (m_loadingBundle == null)
                    return false;

                string key = abPath;
                return m_loadingBundle.TryGetValue(key, out loadingInfo);
            }

            private Dictionary<string, BundleInfo> GetLoadedBundleDict()
            {
                m_loadedBundle = m_loadedBundle ?? new Dictionary<string, BundleInfo>();
                return m_loadedBundle;
            }

            private Dictionary<string, LoadingInfo> GetLoadingBundleDict()
            {
                m_loadingBundle = m_loadingBundle ?? new Dictionary<string, LoadingInfo>();
                return m_loadingBundle;
            }

        }
        ///////////////
        private TextureCache m_u3dTexCache;
        private BundleManager m_bundleManager;
        private Func<string, Texture2D> m_customLoaderSync;              //自定义同步加载器
        private Action<string, Action<Texture2D>> m_customLoaderAsync;   //自定义异步加载器

        public void SetCustomLoader(Func<string, Texture2D> syncFunc, Action<string, Action<Texture2D>> asyncFunc)
        {
            m_customLoaderSync = syncFunc;
            m_customLoaderAsync = asyncFunc;
        }

        public Texture2D GetTexture(string key)
        {
            if (m_u3dTexCache == null)
                return null;

            return m_u3dTexCache.Get(key);
        }

        public bool AddTexture(string key, Texture2D texture2D)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (texture2D == null)
                return false;

            var cache = GetTextureCache();
            var textureInfo = cache.Add(key, texture2D);
            textureInfo.isAddByManager = true;
            textureInfo.Retain();  //强引用

            return textureInfo != null;
        }

        public void RemoveTexture(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (m_u3dTexCache == null)
                return;

            m_u3dTexCache.Dispose(key);
        }

        public void LoadTexture(string path, bool isAsync, Action<TextureCache.TextureInfo> callback)
        {
            if (string.IsNullOrEmpty(path))
                return;

            path = path.ToLower();  //全部转为小写
            if (m_u3dTexCache != null)
            {
                var tetureInfo = m_u3dTexCache.GetTextureInfo(path);
                OnLoadCallback(path, tetureInfo.texture, callback);
                return;
            }

            if (isAsync)
            {
                if (m_customLoaderAsync != null)
                {
                    m_customLoaderAsync(path, (texture2d) =>
                    {
                        OnLoadCallback(path, texture2d, callback);
                    });
                }
                else
                {
                    if (SplitePath(path, out var abPath,out var assetName) >= 0)
                    {
                        GetBundleManager().LoadAsync<Texture2D>(abPath, assetName, (texture2d) =>
                        {
                            var textureInfo = OnLoadCallback(path, texture2d, callback);
                            OnManagerCallback(path, textureInfo);
                        });
                    }
                }
            }
            else
            {
                if (m_customLoaderSync != null)
                {
                    var texture2d = m_customLoaderSync(path);
                    OnLoadCallback(path, texture2d, callback);
                }
                else
                {
                    if (SplitePath(path, out var abPath, out var assetName) >= 0)
                    {
                        var texture2d = GetBundleManager().LoadSync<Texture2D>(abPath, assetName);
                        
                        var textureInfo = OnLoadCallback(path, texture2d, callback);
                        OnManagerCallback(path, textureInfo);
                    }
                }
            }
        }

        private TextureCache.TextureInfo OnLoadCallback(string path, Texture2D texture2d, Action<TextureCache.TextureInfo> action)
        {
            if (texture2d == null)
                return null;

            var cache = GetTextureCache();
            cache.Add(path, texture2d);

            var textureInfo = cache.GetTextureInfo(path);

            action?.Invoke(textureInfo);

            return textureInfo;
        }

        private void OnManagerCallback(string path, TextureCache.TextureInfo textureInfo)
        {
            if (textureInfo == null)
                return;

            if(string.IsNullOrEmpty(path))
                return;

            textureInfo.path = path;
            textureInfo.onRelease = OnReleaseTexture;
            if (SplitePath(textureInfo.path, out var abPath, out var assetName) >= 0)
            {
                GetBundleManager().AddDepend(abPath, assetName);
            }
        }

        private void OnReleaseTexture(TextureCache.TextureInfo textureInfo)
        {
            //释放,如果ab没有依赖了释放Ab
            if (SplitePath(textureInfo.path, out var abPath, out var assetName) >= 0)
            {
                GetBundleManager().RemoveDepend(abPath, assetName);
                GetBundleManager().TryUnload(abPath);
            }
        }

        private TextureCache GetTextureCache()
        {
            if (m_u3dTexCache == null)
            {
                GameObject cacheGobj = new GameObject("TextureCache");
                cacheGobj.transform.SetParent(transform, false);
                m_u3dTexCache = cacheGobj.AddComponent<TextureCache>();
            }
            return m_u3dTexCache;
        }

        private string CombinePath(string abPath,string assetName)
        {
            return string.Format("{0}|{1}", abPath, assetName);
        }

        private int SplitePath(string path,out string abPath,out string assetName)
        {
            abPath = path;
            assetName = null;

            if (string.IsNullOrEmpty(path))
                return -1;

            if (path.IndexOf("|") > 0)
            {
                string[] pathPairs = path.Split('|');
                abPath = pathPairs[0];
                assetName = pathPairs[1];

                return pathPairs.Length;
            }

            return 0;
        }

        private BundleManager GetBundleManager()
        {
            if (m_bundleManager == null)
            {
                GameObject managerGobj = new GameObject("BundleManager");
                managerGobj.transform.SetParent(transform, false);
                m_bundleManager = managerGobj.AddComponent<BundleManager>();
            }
            return m_bundleManager;
        }
    }

}