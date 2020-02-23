using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundController : MonoBehaviour
    {
        public delegate void FinishCallback();
        public static readonly SoundArgs DEFAULT_ARGS = new SoundArgs();
        //音频源控件
        public new AudioSource audio;
        public AudioClip clip;
        public SoundArgs args = DEFAULT_ARGS;
        public FinishCallback onFinish;

        private float m_volume = 1f;
        private bool m_mute = false;
        private float m_pitch = 1f;

        private Coroutine m_fadeSpeedCoroutine = null;
        private Coroutine m_fadeVolumeCoroutine = null;
        private Coroutine m_finishCoroutine = null;

        public AudioSource GetAudio()
        {
            if (audio == null)
            {
                audio = gameObject.GetComponent<AudioSource>();
                if (audio == null)
                {
                    gameObject.AddComponent<AudioSource>();
                    audio.clip = clip;
                    audio.volume = m_volume;
                    audio.mute = m_mute;
                    audio.playOnAwake = false;
                }
                else
                {
                    clip = audio.clip;
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

        public bool IsLoop
        {
            get
            {
                return GetAudio() != null && GetAudio().loop;
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
            get { return m_pitch; }
            set {
                m_pitch = value;
                GetAudio().pitch = m_pitch;
            }
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

        public float Length
        {
            get { return (GetAudio().clip ? GetAudio().clip.length : 0f); }
        }

        //
        public void Play(AudioClip clip, SoundArgs args = null)
        {
            Play();
        }

        public void Play()
        {
            if (clip == null) return;

            Stop();

            GetAudio().clip = clip;
            GetAudio().mute = m_mute;
            GetAudio().volume = m_volume;

            GetAudio().loop = GetArgs().isLoop;
            GetAudio().PlayDelayed(GetArgs().delay);

            StartFinishCoroutine();
        }

        public void Stop()
        {
            StopFadeVolumeCoroutine();
            StopFinishCoroutine();
            StopFadeSpeedCoroutine();

            GetAudio().Stop();
        }

        public void Speed(float form,float to, float fadeTime)
        {
            StopFadeSpeedCoroutine();
            StartFadeSpeedCoroutine(form, to, fadeTime);
        }

        public void Pause(float fadeOut = 1f)
        {
            StopFadeVolumeCoroutine();

            if (fadeOut > 0f)
            {
                StartFadeVolumeCoroutine(m_volume, 0f, fadeOut);
            }
            else
            {
                GetAudio().Pause();
            }
        }

        public void Resume(float fadeIn = 1f)
        {
            StopFadeVolumeCoroutine();

            if (fadeIn > 0f)
            {
                StartFadeVolumeCoroutine(0f, m_volume, fadeIn);
            }
            else
            {
                GetAudio().UnPause();
            }
        }

        ///
        //如果有暂停,继续,重新播放快播慢播,延迟,循环播等操作,需要更新下
        private void StartFinishCoroutine()
        {

            m_finishCoroutine = StartCoroutine(WaitFinish());
        }


        private void StopFinishCoroutine()
        {
            if (m_finishCoroutine != null)
            {
                StopCoroutine(m_finishCoroutine);
                m_finishCoroutine = null;
            }
        }

        private void StartFadeSpeedCoroutine(float form, float to, float fadeTime)
        {

            m_fadeSpeedCoroutine = StartCoroutine(TweenFadeSpeed(form, to, fadeTime));
        }


        private void StopFadeSpeedCoroutine()
        {
            if (m_fadeSpeedCoroutine != null)
            {
                StopCoroutine(m_fadeSpeedCoroutine);
                m_fadeSpeedCoroutine = null;
            }
        }

        private void StartFadeVolumeCoroutine(float form, float to, float fadeTime)
        {
            if (form > to)
            {
                m_fadeVolumeCoroutine = StartCoroutine(TweenFadeVolumeOut(fadeTime));
            }
            else
            {
                m_fadeVolumeCoroutine = StartCoroutine(TweenFadeVolumeIn(fadeTime));
            }
        }
        private void StopFadeVolumeCoroutine()
        {
            if (m_fadeVolumeCoroutine != null)
            {
                StopCoroutine(m_fadeVolumeCoroutine);
                m_fadeVolumeCoroutine = null;
            }
        }

        private IEnumerator TweenFadeVolumeIn(float fadeIn)
        {
            GetAudio().UnPause();
            float time = 0f;
            while (time <= fadeIn)
            {
                GetAudio().volume = 0f + (m_volume - 0f) * Mathf.Pow(time / fadeIn, 3f);
                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
            GetAudio().volume = m_volume;
            m_fadeVolumeCoroutine = null;
        }

        private IEnumerator TweenFadeVolumeOut(float fadeOut)
        {
            float time = 0f;
            while(time <= fadeOut)
            {
                GetAudio().volume = m_volume + (0f - m_volume) * (Mathf.Pow(time / fadeOut - 1f, 3f) + 1.0f);
                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
            GetAudio().volume = 0f;
            GetAudio().Pause();
            m_fadeVolumeCoroutine = null;
        }

        private IEnumerator TweenFadeSpeed(float form, float to, float fadeTime)
        {
            float time = 0f;
            GetAudio().pitch = form;
            while (time <= fadeTime)
            {
                GetAudio().pitch = (form > to) ? (to + (form - to) * Mathf.Pow(time / fadeTime, 3f)) : (form + (to - form) * (Mathf.Pow(time / fadeTime - 1f, 3f) + 1.0f));
                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
            GetAudio().pitch = to;
           
        }

        private IEnumerator WaitFinish()
        {
            while (NormalizedTime >= 1f)
            {
                yield return null;  //每帧检查
            }

            //等待结束,执行回调
            m_finishCoroutine = null;
            onFinish?.Invoke();
        }

        private void OnDestroy()
        {
            StopFadeVolumeCoroutine();
            StopFinishCoroutine();
        }
    }


}
