using System.Collections;
using Object = UnityEngine.Object;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASGame
{
    public class EditorLoader : BaseLoadMethod
    {
#if UNITY_EDITOR
        protected override IEnumerator OnLoadAssetAsync(AssetLoadHandler handler)
        {
            yield return null;  //保证下一帧

            OnLoadAssetSync(handler);
        }

        protected override void OnLoadAssetSync(AssetLoadHandler handler)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(handler.path);
            var result = new AssetLoadResult(obj, true);

            handler.status = AssetLoadStatus.LOAD_FINISHED;
            handler.Callback(result);
        }

        protected override LoadMethod OnLoadMethod(AssetLoadHandler handler)
        {
            return LoadMethod.Nextframe;    //改用下一帧执行,减少协程调用
        }

#else
        protected override IEnumerator OnLoadAssetAsync(AssetLoadHandler handler)
        {
            throw new NotImplementedException();
        }

        protected override void OnLoadAssetSync(AssetLoadHandler handler)
        {
            throw new NotImplementedException();
        }

#endif

    }
}

