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

        public int maxEffectCount;  //最大音效数量

        private float m_soundVolume;
        private float m_effectVolume;
        private float m_musicVolume;

        private bool m_soundMute;
        private bool m_effectMute;
        private bool m_musicMute;

        private Dictionary<SoundType, SoundPlayer> m_soundDict;

        /// <summary>
        /// 控制游戏全局音量
        /// </summary>
        public float SoundVolume
        {
            get { return m_soundVolume; }
            set
            {
                m_soundVolume = Mathf.Clamp(value, 0, 1);
                UpdateAllVolume();
                
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
                UpdateMusicVolume();
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
                UpdateEffectVolume();
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
                UpdateAllVolume();
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
                UpdateMusicVolume();
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
                UpdateEffectVolume();
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
        public void PlayMusic(string clipName, SoundArgs args)
        {
   
        }

        /// <summary>
        /// 停止并销毁声音
        /// </summary>
        /// <param name="clipName"></param>
        public void StopMusic()
        {
           
        }

        /// <summary>
        /// 暂停声音
        /// </summary>
        /// <param name="clipName"></param>
        public void PauseMusic(float fadeOut = 0f)
        {
           
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        /// <param name="clipName"></param>
        public void ResumeMusic(float fadeIn = 0f)
        {
            
        }

        /// <summary>
        /// 设置音乐速度
        /// </summary>
        /// <param name="speed"></param>
        public void SetMuiscSpeed(float speed)
        {

        }

        public float GetMuiscSpeed()
        {
            return 0;
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
            
         
        }

        
        public void LoadConfig()
        {
            m_soundVolume = PlayerPrefs.GetFloat(KEY_SOUND_VOLUME, 1f);
            m_musicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 1f);
            m_effectVolume = PlayerPrefs.GetFloat(KEY_EFFECT_VOLUME, 1f);

            m_soundMute = PlayerPrefs.GetInt(KEY_SOUND_MUTE, 0) > 0 ? true : false;
            m_musicMute = PlayerPrefs.GetInt(KEY_MUSIC_MUTE, 0) > 0 ? true : false;
            m_effectMute = PlayerPrefs.GetInt(KEY_EFFECT_MUTE, 0) > 0 ? true : false;

            UpdateAllPlayers();

        }

        public void SaveConfig()
        {
            PlayerPrefs.SetFloat(KEY_SOUND_VOLUME, m_soundVolume);
            PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, m_musicVolume);
            PlayerPrefs.SetFloat(KEY_EFFECT_VOLUME, m_effectVolume);

            PlayerPrefs.SetInt(KEY_SOUND_MUTE, m_soundMute ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MUSIC_VOLUME, m_musicMute ? 1 : 0);
            PlayerPrefs.SetInt(KEY_EFFECT_MUTE, m_effectMute ? 1 : 0);
        }
        ///

        private float GetRealMusicVolume()
        {
            return (SoundMute || MusicMute) ? 0f : SoundVolume * MusicVolume;
        }

        private float GetRealEffectVolume()
        {
            return (SoundMute || EffectMute) ? 0f : SoundVolume * EffectVolume;
        }

        private SoundPlayer GetOrCreatePlayer(SoundType type)
        {
            m_soundDict = m_soundDict ?? new Dictionary<SoundType, SoundPlayer>();
            if (!m_soundDict.ContainsKey(type))
            {
                string typeName = Enum.Parse(typeof(SoundType), type.ToString()).ToString();
                string playerName = string.Format("{0}Player", typeName);
                GameObject playerGobj = new GameObject(playerName);
                SoundPlayer soundPlayer = playerGobj.AddComponent<SoundPlayer>();
                m_soundDict[type] = soundPlayer;
            }
            return m_soundDict[type];
        }

        private void UpdateAllPlayers()
        {
            UpdateAllVolume();
            UpdateAllMute();
        }

        private void UpdateAllVolume()
        {
            UpdateMusicVolume();
            UpdateEffectVolume();
        }

        private void UpdateAllMute()
        {
            UpdateMusicMute();
            UpdateEffectMute();
        }

        private void UpdateMusicVolume()
        {
            GetOrCreatePlayer(SoundType.Music).Volume = GetRealMusicVolume();
        }

        private void UpdateEffectVolume()
        {
            GetOrCreatePlayer(SoundType.Effect).Volume = GetRealEffectVolume();
        }

        private void UpdateMusicMute()
        {
            GetOrCreatePlayer(SoundType.Music).Mute = MusicMute;
        }

        private void UpdateEffectMute()
        {
            GetOrCreatePlayer(SoundType.Effect).Mute = EffectMute;
        }
    }
}