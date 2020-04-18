using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
using XLibGame;
using System.IO;
using System.Linq;

namespace ASGame
{
    //TODO:要非常注意循环依赖的问题
    //加载依赖应该返回依赖信息,包括哪些依赖文件加载失败
    public class BundleLoader : BaseCoroutineLoader
    {
        public class BundleObject : BaseRef
        {
            public string hashName;                                                 //hash标识符
            public AssetBundle assetBundle;
            public HashSet<BundleObject> depends = new HashSet<BundleObject>();     //依赖项

            protected override void OnRelease()
            {
                //TODO:递归向上释放
            }
        }

        //TODO:AssetBundler的引用计数
        private Dictionary<string, string[]> m_dependsDataList = new Dictionary<string, string[]>();
        private Dictionary<string, BundleObject> m_bundlesMap = new Dictionary<string, BundleObject>();     //已经加载完成的
        private string m_assetBundleRootPath = "";

        private Queue<BundleObject> m_unloadList = new Queue<BundleObject>();                               //释放队列

        /// <summary>
        /// 加载重写,先加载依赖项
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override AssetLoadHandler StartLoad(string path)
        {
            //TODO:判断加载队列里是否已经存在过正在加载的项了,有就不用再次加载

            var mainHandler = base.StartLoad(path);
            var dependencies = GetBundleDependencies(path, false);
            if (dependencies != null && dependencies.Length > 0)
            {
                //把mainHandler的回调保存下来
                var oldCallback = mainHandler.onCallback;
                mainHandler.onCallback = (AssetLoadResult mainResult) =>
                {
                    //如果所有的子handler都加载完了,那么在通知父handler
                    //如果上层没有handler了,才真正回调
                    
                    //XXX:按照异步回调,实际上这里已经调用完了才回调
                };

                foreach(var subDependence in dependencies)
                {
                    //TODO:产生了循环加载,且同一帧可能加载了多个
                    var subHandler = StartLoad(subDependence);      //TODO:这里递归加载了,容易加载到相同的文件

                    subHandler.onCallback += (AssetLoadResult subResult) =>
                    {
                        if (subResult.isDone)
                        {
                            //TODO:通知父handler,已经加载完了
                        }
                    };
                }
            }
            return mainHandler;
        }

        /// <summary>
        /// 加载全局依赖文件
        /// </summary>
        /// <param name="mainfestPath"></param>
        public void LoadMainfest(string mainfestPath, bool isSync = true)
        {
            m_assetBundleRootPath = Path.GetDirectoryName(mainfestPath);
            if (isSync)
            {
                var ab = AssetBundle.LoadFromFile(mainfestPath);
                OnLoadMainfestCallback(ab);
            }
            else
            {
                //用父目录作为assetbundles的root目录
                var handler = StartLoad(mainfestPath);
                handler.onCallback = (AssetLoadResult result) =>
                {
                    var ab = result.asset as AssetBundle;
                    OnLoadMainfestCallback(ab);
                }; 
            }

        }

        protected override void OnUpdate()
        {
            UpdateUnload();
        }

        protected void UpdateUnload()
        {
            while (m_unloadList.Count > 0)
            {
                //TODO:卸载队列,如果引用已经没有了,会被送往这里卸载,
                //但由可能在同一帧时,卸载前又有加载
            }
        }

        /// <summary>
        /// 取得依赖
        /// </summary>
        /// <param name="assetBundlePath">bundle路径</param>
        /// <param name="isAll">是否取得依赖的依赖</param>
        /// <returns></returns>
        private string []GetBundleDependencies(string assetBundlePath, bool isAll = false)
        {
            if (string.IsNullOrEmpty(assetBundlePath))
                return null;

            if (m_dependsDataList != null)
            {
                if (!isAll)
                {
                    //assetBundlePath路径必须是从assets/assetbundle开始,
                    string lowPath = assetBundlePath.ToLower();
                    if (m_dependsDataList.TryGetValue(lowPath, out var dependencies))
                    {
                        return dependencies;
                    }
                }
                else
                {
                    //递归取得所有依赖,并且解决循环依赖加载的问题
                    HashSet<string> dependenciesSet = new HashSet<string>();
                    Stack<string> dependenciesStack = new Stack<string>();

                    dependenciesStack.Push(assetBundlePath.ToLower());
                    while (dependenciesStack.Count > 0)
                    {
                        var depPath = dependenciesStack.Pop();
                        if (m_dependsDataList.TryGetValue(depPath, out var dependencies))
                        {
                            if (dependencies.Length > 0)
                            {
                                foreach (var filePath in dependencies)
                                {
                                    if (!dependenciesSet.Contains(filePath))
                                    {
                                        dependenciesSet.Add(filePath);
                                        dependenciesStack.Push(filePath);
                                    }
                                }
                            }
                        }
                    }
                    return dependenciesSet.ToArray();
                }

            }
            return null;
        }

        private string GetAbsoluteFullPath(string shortBundlePath)
        {
            return Path.Combine(m_assetBundleRootPath, shortBundlePath).ToLower();
        }

        private string GetRelativeShortPath(string fullBundlePath)
        {
            int startPos = fullBundlePath.IndexOf(m_assetBundleRootPath, StringComparison.OrdinalIgnoreCase);
            if (startPos >= 0)
            {
                return fullBundlePath.Substring(startPos + m_assetBundleRootPath.Length + 1).ToLower();
            }
            return fullBundlePath.ToLower();
        }

        private void OnLoadAssetCallback(AssetLoadHandler handler, AssetLoadResult result)
        {
            //如果加载了Bundle,必须记录下来
            handler?.onCallback?.Invoke(result);
        }

        private void OnLoadMainfestCallback(AssetBundle ab)
        {
            if (ab == null)
            {
                Debug.LogError(string.Format("LoadMainfest ab NULL error !"));
                return;
            }

            m_dependsDataList.Clear();
            AssetBundleManifest mainfest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            if (mainfest == null)
            {
                Debug.LogError(string.Format("LoadMainfest NULL error !"));
                return;
            }

            foreach (string assetName in mainfest.GetAllAssetBundles())
            {
                string fullPathLow = GetAbsoluteFullPath(assetName);
                string[] dps = mainfest.GetAllDependencies(assetName);
                for (int i = 0; i < dps.Length; i++)
                {
                    dps[i] = GetAbsoluteFullPath(dps[i]);
                }
                m_dependsDataList.Add(fullPathLow, dps);
            }

            ab.Unload(true);
            Debug.Log("AssetBundleLoadMgr dependsCount=" + m_dependsDataList.Count);
        }

        private BundleObject GetBundleObject(string bundlePath)
        {
            if (m_bundlesMap.TryGetValue(bundlePath, out var bundleObject))
            {
                return bundleObject;
            }
            return null;
        }

        private void AddBundleObject(string bundlePath, AssetBundle assetBundle)
        {
            if (!m_bundlesMap.ContainsKey(bundlePath))
            {
                var bundleObject = new BundleObject();
                bundleObject.assetBundle = assetBundle;
                m_bundlesMap.Add(bundlePath, bundleObject);
            } 
        }

        //加载元操作
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            if (string.IsNullOrEmpty(handler.path))
            {
                var result = AssetLoadResult.FAILED_RESULT;
                OnLoadAssetCallback(handler, result);
                yield break;
            }

            string assetPath = handler.path;
            string assetName = null;
            if (handler.path.IndexOf("|") > 0)
            {
                string[] pathPairs = handler.path.Split('|');
                assetPath = pathPairs[0];
                assetName = pathPairs[1];
            }

            //是否已经在加载池中,如果是就直接返回,引用数加1
            var bundleObject = GetBundleObject(assetPath);
            if (bundleObject != null)
            {
                var assetBundle = bundleObject.assetBundle;
                var result = new AssetLoadResult(assetBundle,true);
                OnLoadAssetCallback(handler, result);
                bundleObject.Retain();
                yield break;
            }

            ////////////////////////////////
            var request = AssetBundle.LoadFromFileAsync(assetPath);
            yield return request;

            if (handler.onCallback != null)
            {
                Object asset = request.assetBundle;
                bool isDone = request.isDone;
                if (request.isDone) //路径不正确,加载不到文件也会返回true
                {
                    var assetBundle = asset as AssetBundle;
                    if (assetBundle != null)
                    {
                        //先把加载到的AssetBundle加入记录缓存,并且标记引用次数+1
                        //不记录Bundle为空的项
                        bundleObject = GetBundleObject(assetPath);
                        if (bundleObject == null)
                        {
                            AddBundleObject(assetPath, assetBundle);
                        }

                        if (!string.IsNullOrEmpty(assetName))
                        {
                            var loadRequest = assetBundle.LoadAssetAsync(assetName);
                            yield return loadRequest;

                            asset = loadRequest.asset;
                            isDone = loadRequest.isDone;
                        }
                    }
                }

                var result = new AssetLoadResult(asset, isDone);
                OnLoadAssetCallback(handler, result);
            }
        }
    }
}

