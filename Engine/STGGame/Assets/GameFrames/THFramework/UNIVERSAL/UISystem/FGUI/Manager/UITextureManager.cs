
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FairyGUI;
using UnityEngine;
using UnityEngine.Networking;
using XLibrary.Package;

namespace THGame.UI
{
    //TODO:同时包含动态合并图集功能
    //异步加载图片资源或者预加载资源,一般一张图片一个AB包
    //离散释放,如果一次加载太多又同时释放会卡
    public static class UITextureLoadStatus
    {
        public static readonly int LOAD_ERROR = -1;
        public static readonly int NETWORK_ERROR = -2;
        public static readonly int NETWORK_TIMEOUT = -3;
        public static readonly int URL_ERROR = -4;
        public static readonly int DATA_ERROR = -5;

    }

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

            public Texture texture;

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

        public TextureInfo Add(string key, Texture texture, bool isReplace = false)
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

        public Texture Get(string key)
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

            foreach (var dictPair in m_texturesDict)
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

            while (m_releaseQueue.Count > 0)
            {
                if (m_texturesDict == null || m_texturesDict.Count <= 0)
                    continue;

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


    //用于加载网络资源
    //TODO:缺少缓存机制
    public class NetworkCentral : MonoBehaviour
    {
        public delegate void LoadSuccess(object data);
        public delegate void LoadFailed(int reason);

        public class TaskInfo
        {
            public int id;
            public string path;
            public Coroutine coroutine;
            public Action<object> onSuccess;
            public Action<int> onFailed;
        }

        public class TaskHandler
        {
            public TaskInfo taskInfo;
            public Action<object> onSuccess;
            public Action<int> onFailed;
        }

        public long maxLocalCacheSize = 0;           //本地缓存大小
        public long maxMemoryCacheSize = 50 * 1024;  //内存缓存池

        private int m_id;
        private Dictionary<int, TaskHandler> m_taskHandlerDict;
        private Dictionary<string, TaskInfo> m_taskInfoDict;

        private Dictionary<int, TaskInfo> m_taskDict;

        public int Load(string url, Action<object> onSuccess, Action<int> onFailed)
        {
            //Url地址严格大小写
            if (string.IsNullOrEmpty(url))
            {
                onFailed?.Invoke(UITextureLoadStatus.URL_ERROR);
                return -1;
            }


            var taskInfo = NewTask(url, onSuccess, onFailed);
            StartTask(taskInfo);

            GetTaskDict().Add(taskInfo.id, taskInfo);
            return taskInfo.id;
        }

        public void Stop(int id)
        {
            if (m_taskDict == null)
                return;

            if (m_taskDict.TryGetValue(id, out var taskInfo))
            {
                StopTask(taskInfo);
                RemoveTask(taskInfo);
            }
        }

        public int LoadTextureFormLocal(string url, Action<Texture> onSuccess, Action<int> onFailed)
        {
            return 0;
        }

        public int LoadTextureFormNetwork(string url, Action<Texture> onSuccess, Action<int> onFailed)
        {
            return Load(url, (obj) =>
            {
                var data = (byte[])obj;
                var texture = new Texture2D(100, 100);
                var ret = texture.LoadImage(data);
                if (!ret)
                {
                    Debug.LogWarningFormat("[TextureNew]url {0} load error", url);
                    onFailed?.Invoke(UITextureLoadStatus.DATA_ERROR);
                    return;
                }
                onSuccess?.Invoke(texture);
            }, onFailed);
        }

        private void Remove(int id)
        {
            if (m_taskDict == null)
                return;

            m_taskDict.Remove(id);
        }

        private TaskInfo NewTask(string path, Action<object> onSuccess, Action<int> onFailed)
        {
            var id = ++m_id;
            var taskInfo = new TaskInfo();

            taskInfo.id = id;
            taskInfo.path = path;
            taskInfo.onSuccess = onSuccess;
            taskInfo.onFailed = onFailed;
            return taskInfo;
        }

        private void StartTask(TaskInfo taskInfo)
        {
            taskInfo.coroutine = StartCoroutine(OnGetCoroutine(taskInfo));
        }

        private void StopTask(TaskInfo taskInfo)
        {
            StopCoroutine(taskInfo.coroutine);
        }

        private void RemoveTask(TaskInfo taskInfo)
        {
            if (m_taskDict == null)
                return;

            m_taskDict.Remove(taskInfo.id);
        }

        private IEnumerator OnGetCoroutine(TaskInfo taskInfo)
        {
            var newUrl = taskInfo.path;
            var request = UnityWebRequest.Get(newUrl);
            request.timeout = 15;

            yield return request.SendWebRequest();
            OnRequestCallback(taskInfo, request);
        }

        private void OnRequestCallback(TaskInfo taskInfo, UnityWebRequest webRequest)
        {
            Remove(taskInfo.id);

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                taskInfo.onFailed?.Invoke(UITextureLoadStatus.NETWORK_ERROR);
                return;
            }


            if (!webRequest.isDone)
            {
                taskInfo.onFailed?.Invoke(UITextureLoadStatus.LOAD_ERROR);
                return;
            }


            if (webRequest.responseCode != 200)
            {
                taskInfo.onFailed?.Invoke(UITextureLoadStatus.LOAD_ERROR);
                return;
            }

            var data = webRequest.downloadHandler.data;
            taskInfo.onSuccess?.Invoke(data);
        }

        private Dictionary<int, TaskInfo> GetTaskDict()
        {
            m_taskDict = m_taskDict ?? new Dictionary<int, TaskInfo>();
            return m_taskDict;
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
                if (dict.ContainsKey(assetName))
                {
                    dict[assetName] += callback;
                }
                else
                {
                    dict.Add(assetName, callback);
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

        public void LoadAsync<T>(string abPath, string assetName, Action<T> onSuccess, Action<int> onFailed) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(abPath))
                return;


            if (TryGetLoadingBundle(abPath, out var loadingInfo))
            {
                //回调合并
                loadingInfo.Add(assetName, (obj) => { onSuccess?.Invoke(obj as T); });
                return;
            }

            loadingInfo = new LoadingInfo();
            loadingInfo.Add(assetName, (obj) => { onSuccess?.Invoke(obj as T); });
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
            if (TryGetLoadedBundle(abPath, out var bundleInfo))
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

    //NTex对象池
    public class NTexturePool : MonoBehaviour
    {
        public class PoolGroup
        {
            public class NTextureInfo
            {
                public NTexture ntexture;
                public Action onDispose;

            }

            public float stayTime = 120f;
            private int maxCount = 50;
            private Dictionary<int, NTextureInfo> m_ntextureMapper;
            private LinkedList<NTextureInfo> m_availableTexs;
            private float m_visitTickTime;

            public int Count()
            {
                if (m_availableTexs == null)
                    return 0;

                return m_availableTexs.Count;
            }

            public NTextureInfo Get()
            {
                if (m_availableTexs == null)
                    return null;

                if (m_availableTexs.Count <= 0)
                    return null;

                var ntextureInfo = m_availableTexs.Last.Value;
                m_availableTexs.RemoveLast();

                UpdateTick();
                return ntextureInfo;
            }

            public bool Add(NTexture ntexture)
            {
                if (ntexture == null)
                    return false;

                var ntextureInfo = CreateNTexture(ntexture);
                if (ntextureInfo == null)
                    return false;

                var code = TransNTextureID(ntexture);
                GetNTextureMap().Add(code, ntextureInfo);
                GetAvailableList().AddLast(ntextureInfo);

                UpdateTick();
                return true;
            }

            public void Release(NTexture ntexture)
            {
                if (Count() > maxCount)
                    return;

                var ntextureMap = GetNTextureMap();

                var code = TransNTextureID(ntexture);
                if (ntextureMap.TryGetValue(code, out var ntextureInfo))
                {
                    GetAvailableList().AddLast(ntextureInfo);
                    UpdateTick();
                }
                else
                {
                    Add(ntexture);
                }
            }

            public void Dispose()
            {
                if (m_availableTexs == null)
                    return;

                foreach (var ntextureInfo in m_availableTexs)
                {
                    ntextureInfo.onDispose?.Invoke();
                }
                m_ntextureMapper?.Clear();
                m_availableTexs.Clear();
            }

            public void Update()
            {
                UpdateInvalid();
            }

            public bool CheckDispose()
            {
                if (Time.realtimeSinceStartup - m_visitTickTime < stayTime)
                    return false;

                return true;
            }

            public void UpdateTick()
            {
                m_visitTickTime = Time.realtimeSinceStartup;
            }

            private void UpdateInvalid()
            {
                if (m_availableTexs == null)
                    return;

                for (LinkedListNode<NTextureInfo> iterNode = m_availableTexs.Last; iterNode != null; iterNode = iterNode.Previous)
                {
                    var ntextureInfo = iterNode.Value;
                    if (ntextureInfo.ntexture == null)
                    {
                        m_availableTexs.Remove(iterNode);
                    }

                }
            }

            private NTextureInfo CreateNTexture(NTexture ntexture)
            {
                NTextureInfo ntextureInfo = new NTextureInfo();
                ntextureInfo.ntexture = ntexture;

                return ntextureInfo;
            }

            private LinkedList<NTextureInfo> GetAvailableList()
            {
                m_availableTexs = m_availableTexs ?? new LinkedList<NTextureInfo>();
                return m_availableTexs;
            }

            private Dictionary<int, NTextureInfo> GetNTextureMap()
            {
                m_ntextureMapper = m_ntextureMapper ?? new Dictionary<int, NTextureInfo>();
                return m_ntextureMapper;
            }

            private int TransNTextureID(NTexture ntexture)
            {
                if (ntexture == null)
                    return 0;

                return ntexture.GetHashCode();
            }

        }
        private static Dictionary<int, string> s_nameMap = new Dictionary<int, string>();
        private Dictionary<string, PoolGroup> m_poolGroups;
        private Queue<string> m_removeQueue;

        public PoolGroup.NTextureInfo Get(string key)
        {
            if (m_poolGroups == null)
                return null;

            if (!m_poolGroups.ContainsKey(key))
                return null;

            var poolGroup = m_poolGroups[key];
            if (poolGroup.Count() <= 0)
                return null;

            return poolGroup.Get();
        }

        public bool Add(string key, NTexture ntexture)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (ntexture == null)
                return false;

            var ret = GetOrCreatePoolGroup(key).Add(ntexture);
            if (ret)
            {
                var code = TransNTextureID(ntexture);
                if (s_nameMap.ContainsKey(code))
                    s_nameMap.Remove(code);

                s_nameMap.Add(code, key);
            }
            return ret;
        }
        public bool Add(string key, Texture texture) { return Add(key, new NTexture(texture)); }
        public bool Add(string key, Texture texture, Rect rect) { return Add(key, new NTexture(texture, rect)); }
        public bool Add(string key, Sprite sprite) { return Add(key, new NTexture(sprite)); }

        public void Release(NTexture ntexture)
        {
            if (ntexture == null)
                return;

            if (m_poolGroups == null)
                return;

            int code = TransNTextureID(ntexture);
            string key = GetNTextureKey(code);
            if (string.IsNullOrEmpty(key))
                return;

            if (!m_poolGroups.ContainsKey(key))
                return;

            var poolGroup = m_poolGroups[key];

            poolGroup.Release(ntexture);
            s_nameMap.Remove(code);
        }

        public void Clear()
        {
            if (m_poolGroups == null)
                return;

            foreach (var poolPair in m_poolGroups)
            {
                var poolGroup = poolPair.Value;
                poolGroup.Dispose();
            }
            m_poolGroups.Clear();
            s_nameMap.Clear();
        }

        private PoolGroup GetOrCreatePoolGroup(string key)
        {
            var poolGroups = GetPoolGroups();
            if (!poolGroups.ContainsKey(key))
            {
                poolGroups[key] = new PoolGroup();
            }
            var poolGroup = poolGroups[key];
            return poolGroup;
        }

        private Dictionary<string, PoolGroup> GetPoolGroups()
        {
            m_poolGroups = m_poolGroups ?? new Dictionary<string, PoolGroup>();
            return m_poolGroups;
        }
        private Queue<string> GetRemoveQueue()
        {
            m_removeQueue = m_removeQueue ?? new Queue<string>();
            return m_removeQueue;
        }

        private int TransNTextureID(NTexture ntexture)
        {
            if (ntexture == null)
                return 0;

            return ntexture.GetHashCode();
        }

        private string GetNTextureKey(int code)
        {
            if (s_nameMap.TryGetValue(code, out var key))
            {
                return key;
            }
            return string.Empty;
        }

        private void Update()
        {
            UpdateGroup();
            UpdateRemove();
        }
        private void UpdateGroup()
        {
            if (m_poolGroups == null)
                return;

            foreach (var itPair in m_poolGroups)
            {
                var poolGroup = itPair.Value;
                poolGroup.Update();
                if (poolGroup.CheckDispose())
                {
                    GetRemoveQueue().Enqueue(itPair.Key);
                }
            }
        }

        private void UpdateRemove()
        {
            if (m_removeQueue == null)
                return;

            while (m_removeQueue.Count > 0)
            {
                var itKey = m_removeQueue.Dequeue();
                if (m_poolGroups == null || m_poolGroups.Count <= 0)
                    continue;

                if (m_poolGroups.TryGetValue(itKey, out var poolGroup))
                {
                    poolGroup.Dispose();
                    m_poolGroups.Remove(itKey);
                }
            }
        }
    }

    public class UITextureManager : MonoSingleton<UITextureManager>
    {
        public static readonly string DEFAULT_TEXTURE_KEY = "_DefaultTexture_";

        private TextureCache m_u3dTexCache;
        private NetworkCentral m_networkCentral;
        private BundleManager m_bundleManager;
        private NTexturePool m_ntexturePool;
        //TODO:小图图集打包
        private Func<string, Texture> m_customLoaderSync;              //自定义同步加载器
        private Action<string, Action<Texture>> m_customLoaderAsync;   //自定义异步加载器
        private Dictionary<string, string> m_pathDict;
        private Texture m_defaultTexture;

        public Texture DefaultTexture
        {
            get
            {
                return m_defaultTexture;
            }

            set
            {
                m_defaultTexture = value;
                AddTexture(DEFAULT_TEXTURE_KEY, m_defaultTexture, true);
            }
        }                    //默认纹理                

        public void SetCustomLoader(Func<string, Texture> syncFunc, Action<string, Action<Texture>> asyncFunc)
        {
            m_customLoaderSync = syncFunc;
            m_customLoaderAsync = asyncFunc;
        }

        public Texture GetTexture(string key)
        {
            if (m_u3dTexCache == null)
                return null;

            return m_u3dTexCache.Get(key);
        }

        public bool AddTexture(string key, Texture texture,bool isReplace = false)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (texture == null)
                return false;

            var cache = GetTextureCache();
            var textureInfo = cache.Add(key, texture, isReplace);
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

        public void GetOrCreateNTexture(string key, bool isAsync, Action<NTexture> onSuccess, Action<int> onFailed)
        {
            if (string.IsNullOrEmpty(key))
                return;

            var ntexturePool = GetNTexturePool();
            var ntextureInfo = ntexturePool.Get(key);
            if (ntextureInfo != null)
            {
                onSuccess?.Invoke(ntextureInfo.ntexture);
            }
            else
            {
                var texture = GetTexture(key);
                if (texture != null)
                {
                    ntexturePool.Add(key, texture);
                    ntextureInfo = ntexturePool.Get(key);
                    onSuccess?.Invoke(ntextureInfo.ntexture);
                }
                else
                {
                    LoadTexture(key, isAsync, (textureInfo) =>
                    {
                        texture = textureInfo.texture;
                        textureInfo.Retain();

                        ntexturePool.Add(key, texture);
                        ntextureInfo = ntexturePool.Get(key);
                        ntextureInfo.onDispose = () =>
                        {
                            textureInfo.Release();
                        };
                        onSuccess?.Invoke(ntextureInfo.ntexture);
                    },(reason) =>
                    {
                        onFailed?.Invoke(reason);
                    });
                }
            }
        }

        public string ParseFormatPath(string srcUrl)
        {
            if (m_pathDict == null)
                return srcUrl;

            if (string.IsNullOrEmpty(srcUrl))
                return srcUrl;

            int protocolStartPos = srcUrl.IndexOf("://");
            if (protocolStartPos <= 0)
                return srcUrl;

            int protocolEndPos = protocolStartPos + "://".Length;
            string protocolStr = srcUrl.Substring(0, protocolStartPos);

            if (m_pathDict.TryGetValue(protocolStr,out var format))
            {
                string assetPath = srcUrl.Substring(protocolEndPos, srcUrl.Length - protocolEndPos);
                string buildStr = format;

                string parentPath = Path.GetDirectoryName(assetPath);
                string fineNameNotEx = Path.GetFileNameWithoutExtension(assetPath);

                buildStr = buildStr.Replace("{path}", assetPath);
                buildStr = buildStr.Replace("{folder}", parentPath);
                buildStr = buildStr.Replace("{fileNameNotEx}", fineNameNotEx);
                buildStr = buildStr.Replace("{protocol}", protocolStr);
                return buildStr;
            }
            return srcUrl;
        }

        public void AddFormatPath(string prefix, string format)
        {
            if (string.IsNullOrEmpty(prefix))
                return;

            m_pathDict = m_pathDict ?? new Dictionary<string, string>();
            m_pathDict[prefix] = format;
        }

        public void ReleaseNTexture(NTexture ntexture)
        {
            var ntexturePool = GetNTexturePool();
            ntexturePool.Release(ntexture);
        }

        public void LoadTexture(string path, bool isAsync, Action<TextureCache.TextureInfo> onSuccess, Action<int> onFailed)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (m_u3dTexCache != null)
            {
                var tetureInfo = m_u3dTexCache.GetTextureInfo(path);
                if (tetureInfo != null)
                {
                    OnLoadCallbackSuccess(true, path, tetureInfo.texture, onSuccess);
                    return;
                }
            }

            if (IsUrl(path))
            {
                GetNetworkCentral().LoadTextureFormNetwork(path, (texture2d) =>
                {
                    OnLoadCallbackSuccess(false, path, texture2d, onSuccess);
                },(reason) =>
                {
                    OnLoadCallbackFailed(reason, onFailed, onSuccess);
                });
            }
            else
            {
                if (isAsync)
                {
                    if (m_customLoaderAsync != null)
                    {
                        m_customLoaderAsync(path, (texture2d) =>
                        {
                            OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                        });
                    }
                    else
                    {
                        if (SplitePath(path, out var abPath, out var assetName) >= 0)
                        {
                            GetBundleManager().LoadAsync<Texture>(abPath, assetName, (texture2d) =>
                            {
                                var textureInfo = OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                                OnManagerCallback(path, textureInfo);
                            },(reason) =>
                            {
                                OnLoadCallbackFailed(reason, onFailed, onSuccess);
                            });
                        }
                    }
                }
                else
                {
                    if (m_customLoaderSync != null)
                    {
                        var texture2d = m_customLoaderSync(path);
                        OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                    }
                    else
                    {
                        if (SplitePath(path, out var abPath, out var assetName) >= 0)
                        {
                            var texture2d = GetBundleManager().LoadSync<Texture>(abPath, assetName);

                            var textureInfo = OnLoadCallbackSuccess(true, path, texture2d, onSuccess);
                            OnManagerCallback(path, textureInfo);
                        }
                    }
                }
            }
        }

        private TextureCache.TextureInfo OnLoadCallbackSuccess(bool isUseCache, string path, Texture texture2d, Action<TextureCache.TextureInfo> onSuccess)
        {
            if (texture2d == null)
                return null;

            var cache = GetTextureCache();
            TextureCache.TextureInfo textureInfo = null;
            cache.Add(path, texture2d);
            textureInfo = cache.GetTextureInfo(path);
            if (!isUseCache)
            {
                cache.Remove(path);
            }

            onSuccess?.Invoke(textureInfo);

            return textureInfo;
        }

        private void OnLoadCallbackFailed(int reason, Action<int> onFailed, Action<TextureCache.TextureInfo> onSuccess)
        {
            if (DefaultTexture == null)
            {
                onFailed?.Invoke(reason);
            }
            else//使用默认贴图
            {
                var cache = GetTextureCache();
                TextureCache.TextureInfo  textureInfo = cache.GetTextureInfo(DEFAULT_TEXTURE_KEY);
                onSuccess?.Invoke(textureInfo);
            }
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

        private bool IsUrl(string str)
        {
            try
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
                return System.Text.RegularExpressions.Regex.IsMatch(str, Url);
            }
            catch (Exception)
            {
                return false;
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

        private NetworkCentral GetNetworkCentral()
        {
            if (m_networkCentral == null)
            {
                GameObject newworkGobj = new GameObject("NetworkCentral");
                newworkGobj.transform.SetParent(transform, false);
                m_networkCentral = newworkGobj.AddComponent<NetworkCentral>();
            }
            return m_networkCentral;
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

        private NTexturePool GetNTexturePool()
        {
            if (m_ntexturePool == null)
            {
                GameObject managerGobj = new GameObject("NTexturePool");
                managerGobj.transform.SetParent(transform, false);
                m_ntexturePool = managerGobj.AddComponent<NTexturePool>();
            }
            return m_ntexturePool;
        }
    }
}