using System;
using System.Collections;
using UnityEditor;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class EditorLoader : BaseNextframeLoader
    {
#if UNITY_EDITOR
        protected override void OnLoadAsset(AssetLoadHandler handler)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(handler.path);
            var result = new AssetLoadResult(obj, true);

            var isCompleted = handler.Transmit(result);
            if (isCompleted)
            {
                handler.status = AssetLoadStatus.LOAD_FINISHED;
                handler.Callback();
            }

        }
#else
        protected override void OnLoadAsset(AssetLoadHandler handler)
        {
            handler.status = AssetLoadStatus.LOAD_FINISHED;
        }
#endif
    }
}

