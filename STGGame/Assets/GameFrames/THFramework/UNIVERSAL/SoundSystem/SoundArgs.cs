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
        /// 音量大小
        /// </summary>
        public float volume = 1f;

        /// <summary>
        /// 是否静音
        /// </summary>
        public bool mute = false;

        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool isLoop = false;

        /// <summary>
        /// 播放的开始时间
        /// </summary>
        public float startTime = 0;

        /// <summary>
        /// 播放的结束时间
        /// </summary>
        public float endTime = -1;

        /// <summary>
        /// 延迟
        /// </summary>
        public ulong delay = 0;

        /// <summary>
        /// 结束回调
        /// </summary>
        public Action onCompleted = null;

    }


}
