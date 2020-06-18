using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public class ResourcesLoader : BaseLoadMethod
    {
        protected override IEnumerator OnLoadAssetAsync(AssetLoadHandler handler)
        {
            string assetPath = handler.path;
            var request = Resources.LoadAsync(assetPath);
            yield return request;

            handler.status = AssetLoadStatus.LOAD_FINISHED;
            var result = new AssetLoadResult(request.asset, request.isDone);
            handler.Callback(result);
        }

        protected override void OnLoadAssetSync(AssetLoadHandler handler)
        {
            string assetPath = handler.path;
            var request = Resources.Load(assetPath);

            handler.status = AssetLoadStatus.LOAD_FINISHED;
            var result = new AssetLoadResult(request, true);
            handler.Callback(result);
        }
    }
}

