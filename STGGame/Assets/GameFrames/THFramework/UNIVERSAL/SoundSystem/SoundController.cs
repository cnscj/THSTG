using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundController : MonoBehaviour
    {
        public static readonly SoundArgs DEFAULT_ARGS = new SoundArgs();
        //音频源控件
        public new AudioSource audio;
        public AudioClip clip;
        public SoundArgs args = DEFAULT_ARGS;

        private float m_volume = 1f;
        private bool m_mute = false;
        private Coroutine m_fadeCoroutine = null;
        public AudioSource GetAudio()
        {
            if (audio == null)
            {
                audio = gameObject.GetComponent<AudioSource>();
                if (audio == null)
                {
                    gameObject.AddComponent<AudioSource>();
                    audio.volume = m_volume;
                    audio.mute = m_mute;
                    audio.playOnAwake = false;
                }
                else
                {
                    m_volume = audio.volume;
                    m_mute = audio.mute;
                }
                
            }

            return audio;
        }

        public SoundArgs GetArgs()
        {
            return args ?? DEFAULT_ARGS;
        }

        public bool IsPlaying
        {
            get
            {
                return GetAudio() != null && GetAudio().isPlaying;
            }
        }

        public float Volume
        {
            get { return m_volume; }
            set
            {
                m_volume = value;
                GetAudio().volume = Volume;

            }
        }

        public bool Mute
        {
            get { return m_mute; }
            set
            {
                m_mute = value;
                GetAudio().mute = m_mute;
            }
        }

        public float Pitch
        {
            get { return GetAudio().pitch; }
            set { GetAudio().pitch = value; }
        }

        public float Time
        {
            get { return GetAudio().time; }
            set { GetAudio().time = value; }
        }

        public float NormalizedTime
        {
            get
            {
                return GetAudio().time / (GetAudio().clip ? GetAudio().clip.length : 1f);
            }
            set
            {
                GetAudio().time = value / (GetAudio().clip ? GetAudio().clip.length : 1f);
            }
        }

        //
        public void Play(AudioClip clip, SoundArgs args = null)
        {
            Play();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (clip == null) return;
            StopFadeCoroutine();

            GetAudio().clip = clip;
            GetAudio().loop = GetArgs().isLoop;
            GetAudio().PlayDelayed(GetArgs().delay);
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            StopFadeCoroutine();

            GetAudio().Stop();
        }

        [ContextMenu("Pause")]
        public void Pause(float fadeOut = 1f)
        {
            StopFadeCoroutine();
            if (fadeOut > 0f)
            {
                m_fadeCoroutine = StartCoroutine(StartFadeOut(fadeOut));
            }
            else
            {
                GetAudio().Pause();
            }
        }

        [ContextMenu("Resume")]
        public void Resume(float fadeIn = 1f)
        {
            StopFadeCoroutine();
            if (fadeIn > 0f)
            {
                m_fadeCoroutine = StartCoroutine(StartFadeIn(fadeIn));
            }
            else
            {
                GetAudio().UnPause();
            }
        }


        ///
        private void StopFadeCoroutine()
        {
            if (m_fadeCoroutine != null)
            {
                StopCoroutine(m_fadeCoroutine);
                m_fadeCoroutine = null;
            }
        }

        private IEnumerator StartFadeIn(float fadeIn)
        {
            GetAudio().UnPause();

            float time = 0f;
            while (time <= fadeIn)
            {
                GetAudio().volume = Mathf.Lerp(0f, m_volume, time / fadeIn);
                time += UnityEngine.Time.deltaTime;
                yield return 1;
            }
            m_fadeCoroutine = null;
        }

        private IEnumerator StartFadeOut(float fadeOut)
        {
            float time = 0f;
            while(time <= fadeOut)
            {
                GetAudio().volume = Mathf.Lerp(m_volume, 0f, time / fadeOut);
                time += UnityEngine.Time.deltaTime;
                yield return 1;
            }

            GetAudio().Pause();
            m_fadeCoroutine = null;
        }



    }


}
