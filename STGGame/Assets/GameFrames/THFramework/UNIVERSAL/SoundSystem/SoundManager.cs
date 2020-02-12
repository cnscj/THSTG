using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
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

        private Dictionary<string, SoundData> m_clips = new Dictionary<string, SoundData>();                                                        //所有音效

        private Dictionary<SoundType, Dictionary<string, SoundData>> m_allClips = new Dictionary<SoundType, Dictionary<string, SoundData>>          //根据类型分类所有音效
        {
            [SoundType.Music] = new Dictionary<string, SoundData>(),
            [SoundType.Effect] = new Dictionary<string, SoundData>(),
        };

        private Transform m_root;           //根物体

        private float m_soundVolume = 1f;
        private float m_effectVolume = 0.8f;
        private float m_musicVolume = 0.8f;

        private bool m_soundMute = false;
        private bool m_effectMute = false;
        private bool m_musicMute = false;


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

        public void Initialize()
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


        /// <summary>
        /// 短暂的音效
        /// 无法暂停
        /// </summary>
        public void PlayEffect(string clipName, float volume = 1)
        {

        }

        /// <summary>
        /// 播放长音乐 背景音乐等
        /// 可以暂停 继续播放
        /// 异步加载音效
        /// </summary>
        /// <param name="clipName">声音的预设名字(不包括前缀路径名)</param>
        /// <param name="delay">延迟播放 单位秒</param>
        /// <param name="volume">音量</param>
        /// <param name="isloop">是否循环播放</param>
        /// /// <param name="forceReplay">是否强制重头播放</param>
        public void PlayMusic(string clipName, UnityEngine.Object soundObj, ulong delay = 0, float volume = 1,
            bool isloop = false, bool forceReplay = false)
        {
           
        }

        //播放SoundData
        private void PlayMusic(string clipName, SoundData asource)
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
    }
}