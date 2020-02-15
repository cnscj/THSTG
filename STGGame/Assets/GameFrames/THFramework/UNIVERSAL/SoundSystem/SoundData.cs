using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class SoundData
    {
        /// <summary>
        /// 音效类型
        /// </summary>
        public SoundType type = SoundType.Effect;

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
        /// 音量
        /// </summary>
        //[HideInInspector]
        public float volume = 1;

        /// <summary>
        /// 延迟
        /// </summary>
        //[HideInInspector]
        public ulong delay = 0;



    }


}
