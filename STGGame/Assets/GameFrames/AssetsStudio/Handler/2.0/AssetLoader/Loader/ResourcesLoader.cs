﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using IEnumerator = System.Collections.IEnumerator;
namespace ASGame
{
    public class ResourcesLoader : BaseCoroutineLoader
    {
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            string assetPath = handler.path;
            var request = Resources.LoadAsync(assetPath);
            yield return request;

            handler.Transmit(new AssetLoadResult(request.asset, request.isDone));
            if (handler.IsCompleted())
            {
                handler.status = AssetLoadStatus.LOAD_FINISHED;
                handler.Callback();
            }
        }
    }
}

