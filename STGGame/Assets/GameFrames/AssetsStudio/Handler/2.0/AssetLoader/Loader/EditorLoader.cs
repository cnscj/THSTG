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
            
            if (handler.TryInvoke(new AssetLoadResult(obj, true)))
            {
                handler.status = AssetLoadStatus.LOAD_FINISHED;
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

