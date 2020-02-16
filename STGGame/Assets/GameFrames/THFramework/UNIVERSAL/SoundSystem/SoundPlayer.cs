using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundPlayer : MonoBehaviour
    {
        public int maxSoundCount = 1;               //最大同时播放个数

        private float m_volume;
        private bool m_mute;
        private float m_speed;

        private Queue<SoundData> m_playingSounds; //播放队列
        private SoundPool m_poolObj;

        /// <summary>
        /// 播放器音量
        /// </summary>
        public float Volume
        {
            get { return m_volume; }
            set
            {
                m_volume = Mathf.Clamp(value, 0, 1);
            }
        }

        /// <summary>
        /// 播放器静音
        /// </summary>
        public bool Mute
        {
            get { return m_mute; }
            set
            {
                m_mute = value;
            }
        }

        /// <summary>
        /// 播放器速度
        /// </summary>
        public float Speed
        {
            get { return m_speed; }
            set
            {
                m_speed = value;
            }
        }

        public void Play(string key)
        {

        }

        public void Stop(string key)
        {

        }

        public void Pause(string key, float fadeOut = 0f)
        {

        }

        public void Resume(string key, float fadeIn = 0f)
        {

        }

        public void StopAll()
        {

        }

        public SoundPool GetPool()
        {
            if (m_poolObj == null)
            {
                GameObject poolGobj = new GameObject("SoundPool");
                m_poolObj = poolGobj.AddComponent<SoundPool>();
            }
            return m_poolObj;
        }
    }


}
