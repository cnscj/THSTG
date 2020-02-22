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

        private Coroutine m_fadeCoroutine = null;
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

            StopFadeCoroutine();

            GetAudio().clip = clip;
            GetAudio().loop = GetArgs().isLoop;
            GetAudio().PlayDelayed(GetArgs().delay);
        }

        public void Stop()
        {
            StopFadeCoroutine();

            GetAudio().Stop();
        }

        public void Pause(float fadeOut = 1f)
        {

            StopFadeCoroutine();

            if (fadeOut > 0f)
            {
                m_fadeCoroutine = StartCoroutine(TweenFadeOut(fadeOut));
            }
            else
            {
                GetAudio().Pause();
            }
        }

        public void Resume(float fadeIn = 1f)
        {
            StopFadeCoroutine();
            StopFinishCoroutine();

            if (fadeIn > 0f)
            {
                m_fadeCoroutine = StartCoroutine(TweenFadeIn(fadeIn));
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
            if (IsLoop) return;

            //取得最新的的时长
            float soundPitch = Pitch;
            float soundLength = Length;
            bool soundLoop = IsLoop;

            float waitTime = 0f;            //受播放延迟,速度影响,重新估算需要等待的时间
            m_finishCoroutine = StartCoroutine(WaitFinish(waitTime));
        }


        private void StopFinishCoroutine()
        {
            if (m_finishCoroutine != null)
            {
                StopCoroutine(m_finishCoroutine);
                m_finishCoroutine = null;
            }
        }

        private void StopFadeCoroutine()
        {
            if (m_fadeCoroutine != null)
            {
                StopCoroutine(m_fadeCoroutine);
                m_fadeCoroutine = null;
            }
        }

        private IEnumerator TweenFadeIn(float fadeIn)
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
            m_fadeCoroutine = null;
        }

        private IEnumerator TweenFadeOut(float fadeOut)
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
            m_fadeCoroutine = null;
        }

        private IEnumerator WaitFinish(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            //等待结束,执行回调
            m_finishCoroutine = null;
            onFinish?.Invoke();
        }

        private void OnDestroy()
        {
            StopFadeCoroutine();
            StopFinishCoroutine();
        }
    }


}
