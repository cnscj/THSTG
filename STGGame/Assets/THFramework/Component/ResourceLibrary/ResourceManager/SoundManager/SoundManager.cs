using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace THGame
{
    /// <summary>
    /// 游戏音效管理组件
    /// </summary>
    public class SoundManager
    {
        public static SoundManager s_instance;

        public static SoundManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new SoundManager();
                }

                return s_instance;
            }
        }

        /// <summary>
        /// 控制游戏全局音量
        /// </summary>
        public float SoundVolume
        {
            get { return m_soundVolume; }
            set
            {
                m_soundVolume = Mathf.Clamp(value, 0, 1);
                foreach (SoundData clip in m_clips.Values)
                {
                    clip.Volume = m_soundVolume * clip.volume;
                }
            }
        }

        private float m_soundVolume = 0.8f;

        //所有音效
        private Dictionary<string, SoundData> m_clips = new Dictionary<string, SoundData>();

        //根据类型分类所有音效
        private Dictionary<SoundType, Dictionary<string, SoundData>> m_allClips =
            new Dictionary<SoundType, Dictionary<string, SoundData>>()
            {
                {SoundType.Music, new Dictionary<string, SoundData>()},
                {SoundType.Sound, new Dictionary<string, SoundData>()}
            };

        //catch ab资源
        private static Dictionary<string, SoundData> m_abSounds = new Dictionary<string, SoundData>();

        //根物体
        private Transform m_root;

        /// <summary>
        /// 音乐静音
        /// </summary>
        public bool MusicMute
        {
            get { return m_musicMute; }
            set
            {
                m_musicMute = value;
                foreach (var soundData in m_allClips[SoundType.Music].Values)
                {
                    soundData.Mute = m_musicMute;
                }

                PlayerPrefs.SetInt("MusicMute", value ? 1 : 0);
            }
        }

        private bool m_musicMute = false;

        /// <summary>
        /// 音效静音
        /// </summary>
        public bool SoundMute
        {
            get { return m_soundMute; }
            set
            {
                m_soundMute = value;
                foreach (var soundData in m_allClips[SoundType.Sound].Values)
                {
                    soundData.Mute = m_soundMute;
                }

                PlayerPrefs.SetInt("SoundMute", value ? 1 : 0);
            }
        }

        private bool m_soundMute = false;

        public void Initialize()
        {
            m_musicMute = PlayerPrefs.GetInt("MusicMute", 0) == 1;
            m_soundMute = PlayerPrefs.GetInt("SoundMute", 0) == 1;

            m_root = new GameObject("SoundDatas").transform;
            GameObject.DontDestroyOnLoad(m_root.gameObject);
        }

        private bool IsContainClip(string clipName)
        {
            lock (m_clips)
            {
                if (m_clips.ContainsKey(clipName))
                    return true;
                return false;
            }
        }

        private SoundData GetAudioSource(string clipName)
        {
            if (IsContainClip(clipName))
                return m_clips[clipName];
            return null;
        }

        private void AddClip(string clipName, SoundData data, SoundType type)
        {
            lock (m_clips)
            {
                data.IsPause = false;
                data.transform.transform.SetParent(m_root);
                data.Sound = type;
                if (IsContainClip(clipName))
                {
                    m_clips[clipName] = data;
                    m_allClips[type][clipName] = data;
                }
                else
                {
                    m_clips.Add(clipName, data);
                    m_allClips[type].Add(clipName, data);
                }
            }
        }

        /// <summary>
        /// 短暂的声音和特效
        /// 无法暂停
        /// 异步加载音效
        /// </summary>
        public async void PlayClip(string clipName, float volume = 1)
        {
//            SoundData sd = await LoadSound(clipName);
//            if (sd != null)
//            {
//                sd.volume = Mathf.Clamp(volume, 0, 1);
//                sd.Mute = SoundMute;
//                if (!IsContainClip(clipName))
//                {
//                    AddClip(clipName, sd, SoundType.Sound);
//                }
//
//                PlayMusic(clipName, sd);
//            }
//            else
//            {
//                Debug.LogError($"没有此音效 ={clipName}");
//            }
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
            SoundData sd = ((GameObject) soundObj).GetComponent<SoundData>();
            if (sd != null)
            {
                sd.isForceReplay = forceReplay;
                sd.isLoop = isloop;
                sd.delay = delay;
                sd.volume = Mathf.Clamp(volume, 0, 1);
                sd.Mute = MusicMute;
                if (!IsContainClip(clipName))
                {
                    AddClip(clipName, sd, SoundType.Music);
                }

                PlayMusic(clipName, sd);
            }
            else
            {
                Debug.LogError($"没有此音效 ={clipName}");
            }
        }

        //播放SoundData
        private void PlayMusic(string clipName, SoundData asource)
        {
            if (null == asource)
                return;
            bool forceReplay = asource.isForceReplay;
            asource.audio.volume = asource.volume * SoundVolume;
            asource.audio.loop = asource.isLoop;
            if (!forceReplay)
            {
                if (!asource.IsPlaying)
                {
                    if (!asource.IsPause)
                        asource.audio.Play(asource.delay);
                    else
                        Resume(clipName);
                }
            }
            else
            {
                asource.audio.PlayDelayed(asource.delay);
                asource.audio.PlayScheduled(0);
            }
        }

        /// <summary>
        /// 停止并销毁声音
        /// </summary>
        /// <param name="clipName"></param>
        public void Stop(string clipName)
        {
            SoundData data = GetAudioSource(clipName);
            if (null != data)
            {
                if (m_allClips[data.Sound].ContainsKey(clipName))
                {
                    m_allClips[data.Sound].Remove(clipName);
                }

                m_clips.Remove(clipName);
                m_abSounds.Remove(clipName);
                data.Dispose();
            }
        }

        /// <summary>
        /// 暂停声音
        /// </summary>
        /// <param name="clipName"></param>
        public void Pause(string clipName)
        {
            SoundData data = GetAudioSource(clipName);
            if (null != data)
            {
                data.IsPause = true;
                data.audio.Pause();
            }
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        /// <param name="clipName"></param>
        public void Resume(string clipName)
        {
            SoundData data = GetAudioSource(clipName);
            if (null != data)
            {
                data.IsPause = false;
                data.audio.UnPause();
            }
        }

        /// <summary>
        /// 销毁所有声音
        /// </summary>
        public void DisposeAll()
        {
            foreach (var allClip in m_allClips.Values)
            {
                allClip.Clear();
            }

            foreach (var item in m_clips)
            {
                item.Value.Dispose();
            }

            m_clips.Clear();
        }
    }
}