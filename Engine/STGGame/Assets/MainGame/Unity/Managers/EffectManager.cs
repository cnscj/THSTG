
using System;
using ASGame;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace STGU3D
{

    public class EffectManager : MonoSingleton<EffectManager>
    {
        /// <summary>
        /// 播放一个特效
        /// </summary>
        /// <param name="code"></param>
        /// <param name="hangNode"></param>
        /// <param name="position"></param>
        /// <param name="onComplete"></param>
        public void PlayOnce(string code, GameObject hangNode, Vector3 position, Action onCompleted = null)
        {
            var pool = GameObjectPoolManager.GetInstance().GetGameObjectPool(code);
            if (pool == null)
            {
                GameObject prefab = AssetManager.GetInstance().LoadEffect(code);
                pool = GameObjectPoolManager.GetInstance().NewGameObjectPool(code, prefab,10);
            }
            var fxGO = pool.GetOrCreate();
            if (fxGO)
            {
                fxGO.transform.position = position;
                if (hangNode != null)
                {
                    fxGO.transform.SetParent(hangNode.transform, false);
                }
                var fxLenMono = fxGO.GetComponent<EffectLengthMono>();
                if (fxLenMono != null)
                {
                    fxLenMono.onCompleted = () =>
                    {
                        onCompleted?.Invoke();
                        GameObjectPoolManager.GetInstance().ReleaseGameObject(fxGO);
                    };
                }
            }
        }
    }
}
