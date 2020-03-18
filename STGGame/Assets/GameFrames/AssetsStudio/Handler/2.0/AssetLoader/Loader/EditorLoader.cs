using System;
using System.Collections;
using UnityEditor;
using ASGame;
using Object = UnityEngine.Object;
namespace ASEditor
{
    public class EditorLoader : BaseNextframeLoader
    {
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(handler.path);
            handler.onCallback?.Invoke(new AssetLoadResult()
            {
                asset = obj,
                isDone = true,
            });
            yield break;
        }
    }
}

