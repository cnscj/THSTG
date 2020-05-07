﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
using XLibGame;
using System.IO;
using System.Linq;
using UnityEngine.Networking;

namespace ASGame
{
    //加载依赖应该返回依赖信息,包括哪些依赖文件加载失败
    public class BundleLoader : BaseCoroutineLoader
    {
        public static readonly float HANDLER_BUNDLE_LOCAL_STAY_TIME = -5;
        public static readonly float HANDLER_BUNDLE_NETWORK_STAY_TIME = -15f;
        public class RequestObj
        {
            public int id;
            public AssetBundleCreateRequest abRequest;
            public UnityWebRequest webRequest;
        }

        public class BundleObject : BaseRef
        {
            public string bundlePath;
            public AssetBundle assetBundle;

            protected override void OnRelease() { assetBundle?.Unload(false); }
        }

        private Dictionary<string, string[]> m_dependsDataList = new Dictionary<string, string[]>();       //总依赖表
        private Dictionary<string, BundleObject> m_bundlesMap = new Dictionary<string, BundleObject>();    //已经加载完成的
        private Dictionary<int, RequestObj> m_handlerWithRequestMap = new Dictionary<int, RequestObj>();   //正在异步的请求
        private string m_assetBundleRootPath = "";


        /// <summary>
        /// 加载全局依赖文件
        /// </summary>
        /// <param name="mainfestPath"></param>
        public void LoadMainfest(string mainfestPath, bool isAsync = false)
        {
            m_assetBundleRootPath = Path.GetDirectoryName(mainfestPath);
            if (isAsync)
            {
                //用父目录作为assetbundles的root目录
                var handler = StartLoad(mainfestPath);
                handler.OnCompleted((AssetLoadResult result) =>
                {
                    var ab = result.asset as AssetBundle;
                    OnLoadMainfestCallback(ab);
                });   
            }
            else
            {
                var ab = AssetBundle.LoadFromFile(mainfestPath);
                OnLoadMainfestCallback(ab);
            }
        }

        public override void UnLoad(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (m_bundlesMap.TryGetValue(path, out var bundleObj))
                {
                    UnloadBundleObject(bundleObj);
                }
            }
        }

        private void UnloadBundleObject(BundleObject mainBundleObj)
        {
            if (mainBundleObj != null)
            {
                string mainBundlePath = mainBundleObj.bundlePath;
                var dependiencies = GetBundleDependencies(mainBundlePath);
                foreach (var subBundlePath in dependiencies)
                {
                    if (m_bundlesMap.TryGetValue(subBundlePath, out var subBundleObj))
                    {
                        subBundleObj.Release();
                    }
                }
                mainBundleObj.Release();
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
                bundleObject.bundlePath = bundlePath;
                bundleObject.assetBundle = assetBundle;

                m_bundlesMap.Add(bundlePath, bundleObject);
            } 
        }

        private void AddOrRetainBundleObject(string bundlePath, AssetBundle assetBundle)
        {
            var bundleObject = GetBundleObject(bundlePath);
            if (bundleObject != null)
            {
                bundleObject.assetBundle = bundleObject.assetBundle ?? assetBundle;
                bundleObject.Retain();
            }
            else
            {
                AddBundleObject(bundlePath, assetBundle);
            }
        }

        //非等待加载
        protected override void OnStartLoad(AssetLoadHandler handler)
        {
            var mainHandler = handler;
            string assetPath = mainHandler.path;

            if (handler.path.IndexOf("|") > 0)
            {
                string[] pathPairs = handler.path.Split('|');
                assetPath = pathPairs[0];
            }

            //加载顺序决定是否能完全卸载,如果先加载依赖,在加载自己,就能够完全释放(这个与释放顺序无关
            var mainDependencies = GetBundleDependencies(assetPath, false);
            if (mainDependencies != null && mainDependencies.Length > 0)
            {
                foreach (var subDependence in mainDependencies)
                {
                    var subHandler = GetOrCreateHandler(subDependence);
                    //FIXME:用OnStartLoad无法加到LoadingMap中,无法自释放
                    //StartLoadWithHandler(subHandler);
                    base.OnStartLoad(subHandler);
                    mainHandler.AddChild(subHandler);
                }
            }
            base.OnStartLoad(mainHandler);
        }

        protected override void OnStopLoad(AssetLoadHandler handler)
        {
            //XXX:这里只中止了主Handler的加载,没有阻止子Handler加载
            //问题是如果子Handler被别的ab引用,是不能被中止的,除非没有引用才中止
            //因此可能要记录下有多少个主handler引用着这个子handle
            //不过如果中止子handler 的话,可能引起主handler卡死无法回调
            handler.result = handler.result ?? AssetLoadResult.EMPTY_RESULT;  //防止卡死无法回调
            base.OnStopLoad(handler);
        }

        protected override void OnLoadSuccess(AssetLoadHandler handler)
        {
            m_handlerWithRequestMap.Remove(handler.id);
        }

        protected override void OnLoadFailed(AssetLoadHandler handler)
        {
            if (m_handlerWithRequestMap.TryGetValue(handler.id, out var requestObj))
            {
                if (requestObj.webRequest != null)
                {
                    requestObj.webRequest.Abort();
                }

                if(requestObj.abRequest != null)
                {
                    //这里强制将异步转同步,提前结束加载
                    //直接取assetBundle即为同步
                    requestObj.abRequest?.assetBundle.Unload(false);
                }
            }
            m_handlerWithRequestMap.Remove(handler.id);
        }

        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            //循环等待所有子加载器加载完在回调
            var children = handler.GetChildren();
            if (children != null && children.Length > 0)
            {
                foreach (var subHandler in children)
                {
                    if (!subHandler.IsCompleted())
                    {
                        yield return null;
                    }
                }
            }
            yield return LoadAssetPrimitive(handler);
        }

        //加载资源回调处理
        private void LoadAssetPrimitiveCallback(AssetLoadHandler handler, AssetLoadResult result)
        {
            //TODO:回调有问题,被多次执行
            result = result ?? AssetLoadResult.EMPTY_RESULT;
            var isCompleted = handler.Transmit(result);
            //if (isCompleted)
            //{
            //    handler.status = AssetLoadStatus.LOAD_FINISHED;
            //    handler.Callback();
            //}
        }

        //加载bundle的回调
        private void LoadAssetBundleCallback(AssetLoadHandler handler, AssetBundle assetBundle)
        {
            if (assetBundle)
            {
                string bundlePath = GetAbsoluteFullPath(assetBundle.name);
                AddOrRetainBundleObject(bundlePath, assetBundle);
                var dependiencies = GetBundleDependencies(bundlePath);
                foreach (var subBundlePath in dependiencies)
                {
                    AddOrRetainBundleObject(subBundlePath, null);   //就算子没加载完也记录
                }
            }
        }

        //加载元操作
        private IEnumerator LoadAssetPrimitive(AssetLoadHandler handler)
        {
            if (string.IsNullOrEmpty(handler.path))
            {
                LoadAssetPrimitiveCallback(handler, AssetLoadResult.EMPTY_RESULT);
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

            Object asset = null;
            bool isDone = false;

            //是否已经在加载池中,如果是就直接返回,引用数加1
            var bundleObject = GetBundleObject(assetPath);
            if (bundleObject != null)
            {
                asset = bundleObject.assetBundle;
                isDone = true;
            }
            else
            {
                if (assetPath.Contains("://"))
                {
                    handler.stayTime = HANDLER_BUNDLE_NETWORK_STAY_TIME;    //网络Handler超时时间
                    var request = UnityWebRequestAssetBundle.GetAssetBundle(assetPath);
                    request.timeout = (int)handler.stayTime;
                    m_handlerWithRequestMap[handler.id] = new RequestObj()
                    {
                        id = handler.id,
                        webRequest = request,
                    };
                    yield return request.SendWebRequest();

                    asset = DownloadHandlerAssetBundle.GetContent(request);
                    isDone = request.isDone;
                }
                else
                {
                    handler.stayTime = HANDLER_BUNDLE_LOCAL_STAY_TIME;    //本地Handler超时时间
                    var request = AssetBundle.LoadFromFileAsync(assetPath);
                    m_handlerWithRequestMap[handler.id] = new RequestObj()
                    {
                        id = handler.id,
                        abRequest = request,
                    };
                    yield return request;

                    asset = request.assetBundle;
                    isDone = request.isDone;
                }

                //先把加载到的AssetBundle加入记录缓存,并且标记引用次数+1
                //不记录Bundle为空的项
                LoadAssetBundleCallback(handler, asset as AssetBundle);
                
            }

            ////////////////////////////////
            var assetBundle = asset as AssetBundle;
            if (assetBundle != null)
            {
                if (!string.IsNullOrEmpty(assetName))
                {
                    var loadRequest = assetBundle.LoadAssetAsync(assetName);
                    yield return loadRequest;

                    asset = loadRequest.asset;
                    isDone = loadRequest.isDone;
                }
            }

            var result = new AssetLoadResult(asset, isDone);
            LoadAssetPrimitiveCallback(handler, result);
        }
    }
}

