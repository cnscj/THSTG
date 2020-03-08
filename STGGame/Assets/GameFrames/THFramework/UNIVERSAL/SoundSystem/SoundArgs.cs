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
        /// tag标记
        /// </summary>
        public string tag = null;

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
        /// 淡入
        /// </summary>
        public float fadeIn = 0f;

        /// <summary>
        /// 淡出
        /// </summary>
        public float fadeOut = 0f;

        /// <summary>
        /// 结束回调
        /// </summary>
        public Action onCompleted = null;

        /// <summary>
        /// 恢复成默认值
        /// </summary>
        public void Reset()
        {
            tag = null;
            volume = 1f;
            mute = false;
            isLoop = false;
            startTime = 0;
            endTime = -1;
            delay = 0;
            fadeIn = 0f;
            fadeOut = 0f;
            onCompleted = null;
        }
    }


}
