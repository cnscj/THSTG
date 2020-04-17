using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
using XLibGame;
using System.IO;

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
        private Dictionary<string, BundleLoader> m_bundlesMap = new Dictionary<string, BundleLoader>();     //已经加载完成的
        private string m_assetBundleRootPath = "";

        private Queue<BundleObject> m_unloadList = new Queue<BundleObject>();                               //释放队列

        /// <summary>
        /// 加载重写,先加载依赖项
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override AssetLoadHandler StartLoad(string path)
        {
            var mainHandler = base.StartLoad(path);
            var dependencies = GetBundleDependencies(path);
            if (dependencies != null && dependencies.Length > 0)   //XXX:需要解决循环依赖加载的问题
            {
                //把mainHandler的回调保存下来
                var oldCallback = mainHandler.onCallback;
                mainHandler.onCallback = (AssetLoadResult mainResult) =>
                {
                    //如果所有的子handler都加载完了,那么在通知父handler
                    //如果上层没有handler了,才真正回调
                    
                    //XXX:按照异步回调,实际上这里已经调用完了才回调
                };

                foreach (var subAssetbundlePath in dependencies)
                {
                    //XXX:查找是否有加载过,加载过的话引用加1完事
                    var subHandler = StartLoad(subAssetbundlePath);
                    subHandler.onCallback += (AssetLoadResult subResult) =>
                    {
                        //:异步可能同时加载了2次一样的,如果之前有过,就放弃
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
        public void LoadMainfest(string mainfestPath)
        {
            //用父目录作为assetbundles的root目录
            m_assetBundleRootPath = Path.GetDirectoryName(mainfestPath);
            var handler = StartLoad(mainfestPath);
            handler.onCallback = OnLoadMainfestCallback;
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

        //TODO:需要加载其他的依赖,最后才能返回回调
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            if (string.IsNullOrEmpty(handler.path))
            {
                handler.onCallback?.Invoke(new AssetLoadResult(null, false));
                FinishHandler(handler);
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

            var request = AssetBundle.LoadFromFileAsync(assetPath);
            yield return request;

            if (handler.onCallback != null)
            {
                Object asset = null;
                bool isDone = false;
                if (request.isDone) //XXX:路径不正确,加载不到文件也会返回true
                {
                    if (!string.IsNullOrEmpty(assetName))
                    {
                        //TODO:这里会产生引用计数,
                        AssetBundle assetBundle = request.assetBundle;
                        var loadRequest = assetBundle.LoadAssetAsync(assetName);
                        yield return loadRequest;

                        asset = loadRequest.asset;
                        isDone = loadRequest.isDone;
                    }
                    else
                    {
                        //这里需要调用者自己管理引用
                        asset = request.assetBundle;
                        isDone = request.isDone;
                    }
                }

                handler.onCallback?.Invoke(new AssetLoadResult(asset, isDone));
                FinishHandler(handler); //XXX:考虑到还有依赖没加载完,可能不能这么快finish
            }
        }

        private string []GetBundleDependencies(string assetBundlePath)
        {
            if (string.IsNullOrEmpty(assetBundlePath))
                return null;

            if (m_dependsDataList != null)
            {
                //assetBundlePath路径必须是从assets/assetbundle开始,
                string lowPath = assetBundlePath.ToLower();
                if (m_dependsDataList.TryGetValue(lowPath, out var dependencies))
                {
                    return dependencies;
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

        private void OnLoadMainfestCallback(AssetLoadResult result)
        {
            var ab = result.asset as AssetBundle;
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
    }
}

