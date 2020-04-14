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
            public string hashName;                                            //hash标识符
            public List<BundleObject> depends = new List<BundleObject>();      //依赖项

            protected override void OnRelease()
            {
                //TODO:递归向上释放
            }
        }

        //TODO:AssetBundler的引用计数
        private Dictionary<string, string[]> m_dependsDataList = new Dictionary<string, string[]>();
        private Dictionary<string, BundleLoader> m_bundlesMap = new Dictionary<string, BundleLoader>();     //已经加载完成的

        private Queue<BundleObject> m_unloadList = new Queue<BundleObject>();                               //释放队列

        /// <summary>
        /// 加载全局依赖文件
        /// </summary>
        /// <param name="mainfestPath"></param>
        public bool LoadMainfest(string mainfestPath)
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
                string hashName = Path.GetFileNameWithoutExtension(assetName);
                string[] dps = mainfest.GetAllDependencies(assetName);
                for (int i = 0; i < dps.Length; i++)
                {
                    dps[i] = Path.GetFileNameWithoutExtension(dps[i]);
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
                //TODO:卸载队列,如果引用已经没有了,会被送往这里卸载,
                //但由可能在同一帧时,卸载前又有加载
            }
        }

        //TODO:需要加载其他的依赖,最后才能返回回调
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            string[] pathPairs = handler.path.Split('|');
            string assetPath = pathPairs[0];
            string assetName = pathPairs[1];
            //TODO:先加载依赖项

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

                handler.onCallback?.Invoke(new AssetLoadResult(asset, isDone));
                FinishHandler(handler);
            }
        }

    }
}

