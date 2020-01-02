
using System;
using System.IO;
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
            //TODO:
        }
    }
}
