
using System;
using ASGame;
using UnityEngine;
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
            GameObject prefab = AssetManager.GetInstance().LoadEffect(code);
            if (prefab)
            {
                var fxGO = UnityEngine.Object.Instantiate(prefab, hangNode?.transform, false);
                fxGO.transform.position = position;
                var fxLenMono = fxGO.GetComponent<EffectLengthMono>();
                if (fxLenMono != null)
                {
                    fxLenMono.onCompleted = () =>
                    {
                        onCompleted?.Invoke();
                        UnityEngine.Object.Destroy(fxGO);
                    };
                }
            }
        }
    }
}
