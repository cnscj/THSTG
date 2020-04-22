using System;
using System.Collections;
using UnityEditor;
using Object = UnityEngine.Object;
namespace ASGame
{
    public class EditorLoader : BaseNextframeLoader
    {
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
#if UNITY_EDITOR
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(handler.path);
            
            if (handler.TryInvoke(new AssetLoadResult(obj, true)))
            {
                handler.status = AssetLoadStatus.LOAD_FINISHED;
            }
#endif
            yield break;
        }
    }
}

