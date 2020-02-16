using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SoundArgs
    {
        /// <summary>
        /// 是否强制重新播放
        /// </summary>
        //[HideInInspector]
        public bool isForceReplay = false;

        /// <summary>
        /// 是否循环播放
        /// </summary>
        //[HideInInspector]
        public bool isLoop = false;

        /// <summary>
        /// 延迟
        /// </summary>
        //[HideInInspector]
        public ulong delay = 0;

        /// <summary>
        /// 淡入
        /// </summary>
        public ulong fadeIn = 0;

        /// <summary>
        /// 淡出
        /// </summary>
        public ulong fadeOut = 0;

        /// <summary>
        /// 结束回调
        /// </summary>
        public Action onCompleted;
    }


}
