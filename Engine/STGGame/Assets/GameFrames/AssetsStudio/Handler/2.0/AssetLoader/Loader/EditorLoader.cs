﻿using System;
using System.Collections;
using Object = UnityEngine.Object;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASGame
{
    public class EditorLoader : BaseNextframeLoader
    {
#if UNITY_EDITOR
        protected override void OnLoadAsset(AssetLoadHandler handler)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(handler.path);
            var result = new AssetLoadResult(obj, true);

            handler.status = AssetLoadStatus.LOAD_FINISHED;
            handler.Callback(result);
        }

#else
        protected override void OnLoadAsset(AssetLoadHandler handler)
        {
            handler.status = AssetLoadStatus.LOAD_FINISHED;
        }

#endif
    }
}
