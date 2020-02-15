using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace THGame
{
    /// <summary>
    /// 游戏音效管理组件
    /// 包括预加载声音,同一声音资源唯一
    /// </summary>
    public class SoundManager : MonoSingleton<SoundManager>
    {
        public static readonly string KEY_SOUND_VOLUME = "SoundVolume";
        public static readonly string KEY_MUSIC_VOLUME = "MusicVolume";
        public static readonly string KEY_EFFECT_VOLUME = "EffectVolume";
        public static readonly string KEY_SOUND_MUTE = "SoundMute";
        public static readonly string KEY_MUSIC_MUTE = "MusicMute";
        public static readonly string KEY_EFFECT_MUTE = "EffectMute";

        private Dictionary<string, SoundData> m_sounds;                             //
        private Dictionary<SoundType, Dictionary<string, SoundData>> m_soundDic;

        private float m_soundVolume = 1f;
        private float m_effectVolume = 0.8f;
        private float m_musicVolume = 0.8f;

        private bool m_soundMute = false;
        private bool m_effectMute = false;
        private bool m_musicMute = false;

        private SoundPool m_soundPool;

        /// <summary>
        /// 控制游戏全局音量
        /// </summary>
        public float SoundVolume
        {
            get { return m_soundVolume; }
            set
            {
                m_soundVolume = Mathf.Clamp(value, 0, 1);
                
                PlayerPrefs.SetFloat(KEY_SOUND_VOLUME, value);
            }
        }

        /// <summary>
        /// 音乐全局音量
        /// </summary>
        public float MusicVolume
        {
            get { return m_musicVolume; }
            set
            {
                m_musicVolume = Mathf.Clamp(value, 0, 1);
               
                PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, value);
            }
        }

        /// <summary>
        /// 音效游戏全局静音
        /// </summary>
        public float EffectVolume
        {
            get { return m_effectVolume; }
            set
            {
                m_effectVolume = Mathf.Clamp(value, 0, 1);
                
                PlayerPrefs.SetFloat(KEY_EFFECT_VOLUME, value);
            }
        }

        /// <summary>
        /// 控制游戏静音
        /// </summary>
        public bool SoundMute
        {
            get { return m_soundMute; }
            set
            {
                m_soundMute = value;

                PlayerPrefs.SetInt(KEY_SOUND_MUTE, value ? 1 : 0);
            }
        }

        /// <summary>
        /// 音乐静音
        /// </summary>
        public bool MusicMute
        {
            get { return m_musicMute; }
            set
            {
                m_musicMute = value;

                PlayerPrefs.SetInt(KEY_MUSIC_VOLUME, value ? 1 : 0);
            }
        }

        /// <summary>
        /// 音效静音
        /// </summary>
        public bool EffectMute
        {
            get { return m_effectMute; }
            set
            {
                m_effectMute = value;

                PlayerPrefs.SetInt(KEY_EFFECT_MUTE, value ? 1 : 0);
            }
        }

        /// <summary>
        /// 短暂的音效
        /// 无法暂停
        /// </summary>
        public void PlayEffect(string clipName)
        {

        }

        //播放SoundData
        public void PlayMusic(string clipName)
        {
   
        }

        /// <summary>
        /// 停止并销毁声音
        /// </summary>
        /// <param name="clipName"></param>
        public void Stop(string clipName)
        {
           
        }

        /// <summary>
        /// 暂停声音
        /// </summary>
        /// <param name="clipName"></param>
        public void Pause(string clipName)
        {
           
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        /// <param name="clipName"></param>
        public void Resume(string clipName)
        {
            
        }

        /// <summary>
        /// 销毁所有声音
        /// </summary>
        public void DisposeAll()
        {

        }

        //
        void Awake()
        {
            m_soundVolume = PlayerPrefs.GetFloat(KEY_SOUND_VOLUME, 1f);
            m_musicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 0.8f);
            m_effectVolume = PlayerPrefs.GetFloat(KEY_EFFECT_VOLUME, 0.8f);

            m_soundMute = PlayerPrefs.GetInt(KEY_SOUND_MUTE, 0) > 0 ? true : false;
            m_musicMute = PlayerPrefs.GetInt(KEY_MUSIC_MUTE, 0) > 0 ? true : false;
            m_effectMute = PlayerPrefs.GetInt(KEY_EFFECT_MUTE, 0) > 0 ? true : false;

            
        }

        ///
        private float GetRealMusicVolume()
        {
            return SoundMute || MusicMute ? 0f : SoundVolume * MusicVolume;
        }

        private float GetRealEffectVolume()
        {
            return SoundMute || EffectMute ? 0f : SoundVolume * EffectVolume;
        }

        private SoundPool GetPool()
        {
            if (m_soundPool == null)
            {
                GameObject poolGobj = new GameObject("SoundPool");
                m_soundPool = poolGobj.AddComponent<SoundPool>();
            }
            return m_soundPool;
        }
    }
}