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
            handler.onCallback?.Invoke(new AssetLoadResult(obj, true));
#endif
            yield break;
        }
    }
}

