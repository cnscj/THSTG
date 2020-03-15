using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;

namespace ASGame
{
    public class BundleLoader : BaseCoroutineLoader
    {
        //TODO:AssetBundler的引用计数

        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            string[] pathPairs = handler.assetPath.Split('|');
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

