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
        //TODO:音量调节

        private static readonly SoundArgs DEFAULT_MUSIC_ARGS = new SoundArgs() { isLoop = true };
        private static readonly SoundArgs DEFAULT_EFFECT_ARGS = new SoundArgs() { isLoop = false };
        private static readonly int DEFAULT_MUSIC_COUNT = 1;
        private static readonly int DEFAULT_EFFECT_COUNT = 6;

        public static readonly string KEY_SOUND_VOLUME = "MaxSoundVolume";
        public static readonly string KEY_MUSIC_VOLUME = "MaxMusicVolume";
        public static readonly string KEY_EFFECT_VOLUME = "MaxEffectVolume";
        public static readonly string KEY_SOUND_MUTE = "IsSoundMute";
        public static readonly string KEY_MUSIC_MUTE = "IsMusicMute";
        public static readonly string KEY_EFFECT_MUTE = "IsEffectMute";

        //TODO:最大音量与实际音量不能混在一起?
        private float m_soundMaxVolume = 1f;
        private float m_effectMaxVolume = 1f;
        private float m_musicMaxVolume = 1f;
        private bool m_soundIsMute = false;
        private bool m_effectIsMute = false;
        private bool m_musicIsMute = false;

        private Dictionary<SoundType, SoundPlayer> m_soundDict;

        public float MaxSoundVolume
        {
            get { return m_soundMaxVolume; }
            set
            {
                m_soundMaxVolume = Mathf.Clamp(value, 0, 1);
                UpdateAllVolume();

            }
        }

        public float MaxEffectVolume
        {
            get { return m_effectMaxVolume; }
            set
            {
                m_effectMaxVolume = Mathf.Clamp(value, 0, 1);
                UpdateAllVolume();

            }
        }
        public float MaxMusicVolume
        {
            get { return m_musicMaxVolume; }
            set
            {
                m_musicMaxVolume = Mathf.Clamp(value, 0, 1);
                UpdateAllVolume();

            }
        }

        public bool IsSoundMute
        {
            get { return m_soundIsMute; }
            set
            {
                m_soundIsMute = value;
                UpdateAllVolume();
            }
        }

        public bool IsMusicMute
        {
            get { return m_effectIsMute; }
            set
            {
                m_effectIsMute = value;
                UpdateAllVolume();
            }
        }

        public bool IsEffectMute
        {
            get { return m_musicIsMute; }
            set
            {
                m_musicIsMute = value;
                UpdateAllVolume();
            }
        }

        /// <summary>
        /// 音乐全局音量
        /// </summary>
        public float SoundVolume
        {
            set
            {
                GetOrCreatePlayer(SoundType.Music).Volume = GetRealEffectVolume(value);
                GetOrCreatePlayer(SoundType.Effect).Volume = GetRealEffectVolume(value);
            }
        }

        public int MaxMusicCount
        {
            get { return GetOrCreatePlayer(SoundType.Music).maxCount; }
            set
            {
                GetOrCreatePlayer(SoundType.Music).maxCount = value;
            }
        }

        public float MusicVolume
        {
            get { return GetOrCreatePlayer(SoundType.Music).Volume ; }
            set
            {
                GetOrCreatePlayer(SoundType.Music).Volume = GetRealMusicVolume(value);
            }
        }

        /// <summary>
        /// 音效游戏全局静音
        /// </summary>
        public float EffectVolume
        {
            get { return GetOrCreatePlayer(SoundType.Effect).Volume; }
            set
            {
                GetOrCreatePlayer(SoundType.Effect).Volume = GetRealEffectVolume(value);
            }
        }

        /// <summary>
        /// 音乐静音
        /// </summary>
        public bool MusicMute
        {
            get { return GetOrCreatePlayer(SoundType.Music).Mute && IsMusicMute && IsSoundMute; }
            set
            {
                GetOrCreatePlayer(SoundType.Music).Mute = value && IsMusicMute && IsSoundMute;
            }
        }

        public int MaxEffectCount
        {
            get { return GetOrCreatePlayer(SoundType.Effect).maxCount; }
            set
            {
                GetOrCreatePlayer(SoundType.Effect).maxCount = value;
            }
        }

        /// <summary>
        /// 音效静音
        /// </summary>
        public bool EffectMute
        {
            get { return GetOrCreatePlayer(SoundType.Effect).Mute && IsEffectMute && IsSoundMute; }
            set
            {
                GetOrCreatePlayer(SoundType.Effect).Mute = value && IsEffectMute && IsSoundMute;
            }
        }

        /// <summary>
        /// 短暂的音效
        /// 无法暂停
        /// </summary>
        public int PlayEffect(AudioClip clip, SoundArgs args = null)
        {
            args = args ?? DEFAULT_EFFECT_ARGS;
            return GetOrCreatePlayer(SoundType.Effect).Play(new SoundData() { clip = clip }, args);
        }

        ///////////////////
        //播放SoundData
        public int PlayMusic(AudioClip clip, SoundArgs args = null)
        {
            args = args ?? DEFAULT_MUSIC_ARGS;
            return GetOrCreatePlayer(SoundType.Music).PlayForce(new SoundData() { clip = clip }, args);
        }

        public int PlayMusic(AudioClip clip, bool isLoop, float startTime = 0f ,Action onCompleted = null)
        {
            var args = GetOrCreatePlayer(SoundType.Music).GetOrCreateArgs();
            args.Reset();
            args.isLoop = isLoop;
            args.startTime = startTime;
            args.onCompleted = onCompleted;

            return PlayMusic(clip, args);
        }

        /// <summary>
        /// 停止并销毁声音
        /// </summary>
        /// <param name="clipName"></param>
        public void StopMusic()
        {
            var player = GetSoundPlayer(SoundType.Music);
            if (player != null)
            {
                player.StopAll();
            }
        }

        /// <summary>
        /// 暂停声音
        /// </summary>
        /// <param name="clipName"></param>
        public void PauseMusic(float fadeOut = 0f)
        {
            var player = GetSoundPlayer(SoundType.Music);
            if (player != null)
            {
                player.Pause(fadeOut);
            }
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        /// <param name="clipName"></param>
        public void ResumeMusic(float fadeIn = 0f)
        {
            var player = GetSoundPlayer(SoundType.Music);
            if (player != null)
            {
                player.Resume(fadeIn);
            }
        }

        /// <summary>
        /// 音量渐变
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="fadeTime"></param>
        public void TweenMusicVolume(float from, float to, float fadeTime)
        {
            var player = GetSoundPlayer(SoundType.Music);
            if (player != null)
            {
                player.TweenVolume(GetRealMusicVolume(from), GetRealMusicVolume(to), fadeTime);
            }
        }

        /// <summary>
        /// 销毁所有声音
        /// </summary>
        public void DisposeAll()
        {
            if (m_soundDict != null)
            {
                foreach(var pair in m_soundDict)
                {
                    pair.Value.DisposeAll();
                }
            }
        }

        /// <summary>
        /// 销毁声音
        /// </summary>
        /// <param name="type"></param>
        public void Dispose(SoundType type)
        {
            var player = GetSoundPlayer(type);
            if (player != null)
            {
                player.DisposeAll();
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        public void LoadConfig()
        {
            m_soundMaxVolume = PlayerPrefs.GetFloat(KEY_SOUND_VOLUME, 1f);
            m_musicMaxVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 1f);
            m_effectMaxVolume = PlayerPrefs.GetFloat(KEY_EFFECT_VOLUME, 1f);

            m_soundIsMute = PlayerPrefs.GetInt(KEY_SOUND_MUTE, 0) > 0 ? true : false;
            m_musicIsMute = PlayerPrefs.GetInt(KEY_MUSIC_MUTE, 0) > 0 ? true : false;
            m_effectIsMute = PlayerPrefs.GetInt(KEY_EFFECT_MUTE, 0) > 0 ? true : false;

            UpdateAllPlayers();

        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            PlayerPrefs.SetFloat(KEY_SOUND_VOLUME, m_soundMaxVolume);
            PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, m_musicMaxVolume);
            PlayerPrefs.SetFloat(KEY_EFFECT_VOLUME, m_effectMaxVolume);

            PlayerPrefs.SetInt(KEY_SOUND_MUTE, m_soundIsMute ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MUSIC_VOLUME, m_musicIsMute ? 1 : 0);
            PlayerPrefs.SetInt(KEY_EFFECT_MUTE, m_effectIsMute ? 1 : 0);
        }
        ///
        private void Awake()
        {
            //新建两个播放器,音效和音乐
            var effectPlayer = GetOrCreatePlayer(SoundType.Effect);
            effectPlayer.maxCount = DEFAULT_EFFECT_COUNT;


            var musicPlayer = GetOrCreatePlayer(SoundType.Music);
            musicPlayer.maxCount = DEFAULT_MUSIC_COUNT;   //音乐播放最大可播放1个

        }

        private float GetRealMusicVolume(float volume)
        {
            return (IsSoundMute || IsMusicMute) ? 0f : MaxSoundVolume * MaxMusicVolume * volume;
        }

        private float GetRealEffectVolume(float volume)
        {
            return (IsSoundMute || IsEffectMute) ? 0f : MaxSoundVolume * MaxEffectVolume * volume;
        }

        private SoundPlayer GetSoundPlayer(SoundType type)
        {
            if (m_soundDict != null)
            {
                if (m_soundDict.ContainsKey(type))
                {
                    return m_soundDict[type];
                }
            }
            return null;
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
                playerGobj.transform.SetParent(transform);
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
            GetOrCreatePlayer(SoundType.Music).Volume = GetRealMusicVolume(MusicVolume);
        }

        private void UpdateEffectVolume()
        {
            GetOrCreatePlayer(SoundType.Effect).Volume = GetRealEffectVolume(EffectVolume);
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