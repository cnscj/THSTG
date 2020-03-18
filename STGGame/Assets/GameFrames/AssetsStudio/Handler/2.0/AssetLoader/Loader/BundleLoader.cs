using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;

namespace ASGame
{
    //TODO:要非常注意循环依赖的问题
    public class BundleLoader : BaseCoroutineLoader
    {
        public class BundleObject
        {
            public string hashName;                                            //hash标识符
            public int refCount;                                               //引用计数

            public int dependLoadingCount;                                     //依赖计数
            public List<BundleObject> depends = new List<BundleObject>();      //依赖项
        }

        //TODO:AssetBundler的引用计数
        private Dictionary<string, string[]> m_dependsDataList = new Dictionary<string, string[]>();
        private Dictionary<string, BundleLoader> m_loadedMap = new Dictionary<string, BundleLoader>();  //已经加载完成的
        private Queue<BundleObject> m_unloadList = new Queue<BundleObject>();                           //释放队列

        /// <summary>
        /// 加载全局依赖文件
        /// </summary>
        /// <param name="mainfestPath"></param>
        /// <param name="bundleSuffix"></param>
        public bool LoadMainfest(string mainfestPath, string bundleSuffix = ".ab")
        {
            if (string.IsNullOrEmpty(mainfestPath))
                return false;
            
            m_dependsDataList.Clear();
            AssetBundle ab = AssetBundle.LoadFromFile(mainfestPath);

            if (ab == null)
            {
                Debug.LogError(string.Format("LoadMainfest ab NULL error !"));
                return false;
            }

            AssetBundleManifest mainfest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            if (mainfest == null)
            {
                Debug.LogError(string.Format("LoadMainfest NULL error !"));
                return false;
            }

            foreach (string assetName in mainfest.GetAllAssetBundles())
            {
                string hashName = assetName.Replace(bundleSuffix, "");
                string[] dps = mainfest.GetAllDependencies(assetName);
                for (int i = 0; i < dps.Length; i++)
                {
                    dps[i] = dps[i].Replace(bundleSuffix, "");
                }
                m_dependsDataList.Add(hashName, dps);
            }

            ab.Unload(true);
            Debug.Log("AssetBundleLoadMgr dependsCount=" + m_dependsDataList.Count);
            return true;
        }

        protected override void OnUpdate()
        {
            UpdateUnload();
        }

        protected void UpdateUnload()
        {
            while (m_unloadList.Count > 0)
            {

            }
        }

        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            string[] pathPairs = handler.path.Split('|');
            string assetPath = pathPairs[0];
            string assetName = pathPairs[1];
            var request = AssetBundle.LoadFromFileAsync(assetPath);
            yield return request;

            if (handler.onCallback != null)
            {
                Object asset = null;
                bool isDone = false;
                if (request.isDone)
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

                handler.onCallback.Invoke(new AssetLoadResult()
                {
                    asset = asset,
                    isDone = isDone,
                });
            }
        }
    }
}

