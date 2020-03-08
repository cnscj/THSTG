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
        private static readonly SoundArgs DEFAULT_MUSIC_ARGS = new SoundArgs() { isLoop = true };
        private static readonly SoundArgs DEFAULT_EFFECT_ARGS = new SoundArgs() { isLoop = false };
        private static readonly int DEFAULT_MUSIC_COUNT = 1;    //音乐播放最大可播放1个
        private static readonly int DEFAULT_EFFECT_COUNT = -1;   //音效播放最大可播放无限个

        public static readonly string KEY_SOUND_VOLUME = "MaxSoundVolume";
        public static readonly string KEY_MUSIC_VOLUME = "MaxMusicVolume";
        public static readonly string KEY_EFFECT_VOLUME = "MaxEffectVolume";
        public static readonly string KEY_SOUND_MUTE = "IsSoundMute";
        public static readonly string KEY_MUSIC_MUTE = "IsMusicMute";
        public static readonly string KEY_EFFECT_MUTE = "IsEffectMute";

        //最大音量,静音设置
        private float m_soundMaxVolume = 1f;
        private float m_effectMaxVolume = 1f;
        private float m_musicMaxVolume = 1f;
        private bool m_soundIsMute = false;
        private bool m_effectIsMute = false;
        private bool m_musicIsMute = false;

        private Dictionary<SoundType, SoundPlayer> m_soundDict; //播放器

        public float MusicVolume
        {
            get {return (MaxSoundVolume <= 0f || MaxMusicVolume <= 0f) ? 0f: GetOrCreatePlayer(SoundType.Music).Volume / (MaxSoundVolume * MaxMusicVolume);}
            set { GetOrCreatePlayer(SoundType.Music).Volume = MaxSoundVolume * MaxMusicVolume * value; }
        }
        public float EffectVolume
        {
            get { return (MaxSoundVolume <= 0f || MaxEffectVolume <= 0f) ? 0f : GetOrCreatePlayer(SoundType.Effect).Volume / (MaxSoundVolume * MaxEffectVolume); }
            set { GetOrCreatePlayer(SoundType.Effect).Volume = MaxSoundVolume * MaxEffectVolume * value; }
        }

        public bool MusicMute
        {
            get { return GetOrCreatePlayer(SoundType.Music).Mute || IsMusicMute || IsSoundMute; }
            set { GetOrCreatePlayer(SoundType.Music).Mute = IsMusicMute || IsSoundMute || value; }
        }

        public bool EffectMute
        {
            get { return GetOrCreatePlayer(SoundType.Effect).Mute || IsEffectMute || IsSoundMute; }
            set { GetOrCreatePlayer(SoundType.Effect).Mute = IsEffectMute || IsSoundMute || value; }
        }

        public int MusicMaxCount
        {
            get { return GetOrCreatePlayer(SoundType.Music).maxCount; }
            set { GetOrCreatePlayer(SoundType.Music).maxCount = value; }
        }

        public int EffectMaxCount
        {
            get { return GetOrCreatePlayer(SoundType.Effect).maxCount; }
            set { GetOrCreatePlayer(SoundType.Effect).maxCount = value; }
        }

        public float MaxSoundVolume
        {
            get { return m_soundMaxVolume; }
            set
            {
                var oldMusicVolume = MusicVolume;
                var oldEffectVolume = EffectVolume;
                m_soundMaxVolume = Mathf.Clamp(value, 0f, 1f);
                MusicVolume = oldMusicVolume;
                EffectVolume = oldEffectVolume;
            }
        }

        public float MaxEffectVolume
        {
            get { return m_effectMaxVolume; }
            set
            {
                var oldEffectVolume = EffectVolume;
                m_effectMaxVolume = Mathf.Clamp(value, 0f, 1f);
                EffectVolume = oldEffectVolume;
            }
        }
        public float MaxMusicVolume
        {
            get { return m_musicMaxVolume; }
            set
            {
                var oldMusicVolume = MusicVolume;
                m_musicMaxVolume = Mathf.Clamp(value, 0f, 1f);
                MusicVolume = oldMusicVolume;
            }
        }

        public bool IsSoundMute
        {
            get { return m_soundIsMute; }
            set
            {
                var oldMusicMute = MusicMute;
                var oldEffectMute = EffectMute;
                m_soundIsMute = value;
                MusicMute = oldMusicMute;
                EffectMute = oldEffectMute;
            }
        }

        public bool IsMusicMute
        {
            get { return m_effectIsMute; }
            set
            {
                var oldMusicMute = MusicMute;
                m_effectIsMute = value;
                MusicMute = oldMusicMute;
            }
        }

        public bool IsEffectMute
        {
            get { return m_musicIsMute; }
            set
            {
                var oldEffectMute = EffectMute;
                m_musicIsMute = value;
                EffectMute = oldEffectMute;
            }
        }

        /////////////////////////////

        /// <summary>
        /// 短暂的音效
        /// 无法暂停
        /// </summary>
        public int PlayEffect(AudioClip clip, SoundArgs args = null)
        {
            args = args ?? DEFAULT_EFFECT_ARGS;
            return GetOrCreatePlayer(SoundType.Effect).Play(new SoundData() { clip = clip }, args);
        }
        public int PlayEffect(AudioClip clip, string tag, Action onCompleted = null)
        {
            var args = GetOrCreatePlayer(SoundType.Effect).GetOrCreateArgs();
            args.Reset();
            args.tag = tag;
            args.onCompleted = onCompleted;

            return PlayEffect(clip, args);
        }

        /// <summary>
        /// 停止所有音效
        /// </summary>
        public void StopEffects()
        {
            var player = GetSoundPlayer(SoundType.Effect);
            if (player != null)
            {
                player.StopAll();
            }
        }

        /// <summary>
        /// 相同tag最大 数量
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="count"></param>
        public void SetEffectTagMaxCount(string tag,int count)
        {
            GetOrCreatePlayer(SoundType.Effect).SetTagMaxCount(tag, count);
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
                player.TweenVolume(from, to, fadeTime);
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
            MaxSoundVolume = PlayerPrefs.GetFloat(KEY_SOUND_VOLUME, 1f);
            MaxMusicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, 1f);
            MaxEffectVolume = PlayerPrefs.GetFloat(KEY_EFFECT_VOLUME, 1f);

            IsSoundMute = PlayerPrefs.GetInt(KEY_SOUND_MUTE, 0) > 0 ? true : false;
            IsMusicMute = PlayerPrefs.GetInt(KEY_MUSIC_MUTE, 0) > 0 ? true : false;
            IsEffectMute = PlayerPrefs.GetInt(KEY_EFFECT_MUTE, 0) > 0 ? true : false;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            PlayerPrefs.SetFloat(KEY_SOUND_VOLUME, MaxSoundVolume);
            PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, MaxMusicVolume);
            PlayerPrefs.SetFloat(KEY_EFFECT_VOLUME, MaxEffectVolume);

            PlayerPrefs.SetInt(KEY_SOUND_MUTE, IsSoundMute ? 1 : 0);
            PlayerPrefs.SetInt(KEY_MUSIC_VOLUME, IsMusicMute ? 1 : 0);
            PlayerPrefs.SetInt(KEY_EFFECT_MUTE, IsEffectMute ? 1 : 0);
        }
        ///
        private void Awake()
        {
            //新建两个播放器,音效和音乐
            var effectPlayer = GetOrCreatePlayer(SoundType.Effect);
            effectPlayer.maxCount = DEFAULT_EFFECT_COUNT;


            var musicPlayer = GetOrCreatePlayer(SoundType.Music);
            musicPlayer.maxCount = DEFAULT_MUSIC_COUNT;   

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

    }
}