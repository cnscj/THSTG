﻿using System;
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
            public string bundlePath;
            public AssetBundle assetBundle;
            public HashSet<BundleObject> depends = new HashSet<BundleObject>();     //依赖项

            protected override void OnRelease()
            {
                assetBundle.Unload(false); //TODO:递归向下释放,先释放自己在释放依赖
            }
        }

        private Dictionary<string, string[]> m_dependsDataList = new Dictionary<string, string[]>();
        private Dictionary<string, BundleObject> m_bundlesMap = new Dictionary<string, BundleObject>();     //已经加载完成的
        private Queue<BundleObject> m_unloadList = new Queue<BundleObject>();                               //释放队列
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

        protected override void OnUpdate()
        {
            UpdateUnload();
        }

        protected void UpdateUnload()
        {
            //规则:
            //正在异步加载中也不能卸载
            //常驻资源就不卸载；非常驻资源，并且引用计数为0才能卸载
            while (m_unloadList.Count > 0)
            {
                var bundleObj = m_unloadList.Dequeue();
                
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
            //只有子依赖完成回调了,才真正回调
            result = result ?? AssetLoadResult.EMPTY_RESULT;
            var isDone = handler.TryInvoke(result);
            if (isDone)
            {
                //记录依赖信息,引用自增
                handler.status = AssetLoadStatus.LOAD_FINISHED;
            }  
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

            //这里一次性读取所有依赖,无需递归
            var mainDependencies = GetBundleDependencies(assetPath, false);
            if (mainDependencies != null && mainDependencies.Length > 0)
            {
                foreach (var subDependence in mainDependencies)
                {
                    var subHandler = GetOrCreateHandler(subDependence);
                    base.OnStartLoad(subHandler);//StartLoadWithHandler(subHandler);
                    mainHandler.AddChild(subHandler);
                }
            }
            base.OnStartLoad(mainHandler);
        }

        //加载元操作
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            if (string.IsNullOrEmpty(handler.path))
            {
                OnLoadAssetCallback(handler, AssetLoadResult.EMPTY_RESULT);
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
                bundleObject.Retain();
                asset = bundleObject.assetBundle;
                isDone = true;
            }
            else
            {
                var request = AssetBundle.LoadFromFileAsync(assetPath);
                yield return request;

                asset = request.assetBundle;
                isDone = request.isDone;

                //先把加载到的AssetBundle加入记录缓存,并且标记引用次数+1
                //不记录Bundle为空的项
                if (isDone && asset != null)
                {
                    //TODO:这里需要把子依赖项也添加进去,不然会出大问题(引用计数
                    AddBundleObject(assetPath, asset as AssetBundle);
                }
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
            OnLoadAssetCallback(handler, result);
        }
    }
}

