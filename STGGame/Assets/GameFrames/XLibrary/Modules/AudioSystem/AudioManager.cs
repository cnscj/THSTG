using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    //TODO:
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public int maxEffects = 6;       //最大音效个数,

        private int m_auidoIds = 0;
        private float m_audioVolume = 1f;
        private float m_effectVolume = 0.8f;
        private float m_musicVolume = 0.8f;

        private Queue<AudioInfos> m_effects;
        private Queue<AudioInfos> m_musics;
        private Dictionary<int, AudioInfos> m_audios;

        /// <summary>
        /// 总音响控制
        /// </summary>
        public float AudioVolume
        {
            get { return m_audioVolume; }
            set
            {
                m_audioVolume = Mathf.Clamp(value, 0f, 1f);
            }
        }

        /// <summary>
        /// 音效音量 final = total * factory
        /// </summary>
        public float EffectVolume
        {
            get { return m_effectVolume; }
            set
            {
                m_effectVolume = Mathf.Clamp(value, 0f, 1f);
            }
        }

        /// <summary>
        /// 音乐音量 final = total * factory
        /// </summary>
        public float MusicVolume
        {
            get { return m_musicVolume; }
            set
            {
                m_musicVolume = Mathf.Clamp(value, 0f, 1f);
            }
        }
        ///
        public int PlayEffect(AudioClip clip, bool isLoop)
        {
            var infos = CreateAudioInfos(clip , AudioCategory.Effect);
            if (infos != null)
            {

            }

            return -1;
        }

        public int PlayMusic(AudioClip clip, bool isLoop)
        {
            var infos = CreateAudioInfos(clip, AudioCategory.Music);
            if (infos != null)
            {

            }

            return -1;

        }

        public void StopMusic(int id)
        {

        }

        AudioInfos CreateAudioInfos(AudioClip clip, AudioCategory category)
        {
            AudioInfos infos = new AudioInfos();
            var id = ++m_auidoIds;

            return infos;
        }

    }
}
