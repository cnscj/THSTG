using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundController : MonoBehaviour
    {
        public delegate void FinishCallback();
        //音频源控件
        public new AudioSource audio;
        public FinishCallback onFinish;

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

                    audio.playOnAwake = false;
                }
                

            }

            return audio;
        }

        public AudioClip Clip
        {
            get
            {
                return GetAudio().clip;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return GetAudio() != null && GetAudio().isPlaying;
            }
        }
        public bool IsPause
        {
            get
            {
                return IsPlaying && NormalizedTime <= 0f;
            }
        }

        public bool IsLoop
        {
            get
            {
                return GetAudio() != null && GetAudio().loop;
            }
            set
            {
                GetAudio().loop = value;
            }
        }

        public float Volume
        {
            get { return GetAudio().volume; }
            set
            {
                GetAudio().volume = value;

            }
        }

        public bool Mute
        {
            get { return GetAudio().mute; }
            set
            {
                GetAudio().mute = value;
            }
        }

        public float Pitch
        {
            get { return GetAudio().pitch; }
            set {
                GetAudio().pitch = value;
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
        public void Play(AudioClip clip, float delay = 0f)
        {
            if (clip == null) return;

            Stop();

            GetAudio().clip = clip;

            GetAudio().PlayDelayed(delay);

            StartFinishCoroutine();
        }

        public void Stop()
        {
            StopFadeVolumeCoroutine();
            StopFinishCoroutine();
            StopFadeSpeedCoroutine();

            GetAudio().Stop();
        }

        public void Speed(float from,float to, float fadeTime)
        {
            StopFadeSpeedCoroutine();
            StartFadeSpeedCoroutine(from, to, fadeTime);
        }

        public void Pause(float fadeOut = 1f)
        {
            StopFadeVolumeCoroutine();

            if (fadeOut > 0f)
            {
                StartFadeVolumeCoroutine(Volume, 0f, fadeOut);
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
                StartFadeVolumeCoroutine(0f, Volume, fadeIn);
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
            //如果播放速度没有改变,就不用每帧update了,否则用每帧的
            m_finishCoroutine = StartCoroutine(WaitFinishByStep());
        }


        private void StopFinishCoroutine()
        {
            if (m_finishCoroutine != null)
            {
                StopCoroutine(m_finishCoroutine);
                m_finishCoroutine = null;
            }
        }

        private void StartFadeSpeedCoroutine(float from, float to, float fadeTime)
        {
            m_fadeSpeedCoroutine = StartCoroutine(TweenFadeSpeed(from, to, fadeTime));
        }


        private void StopFadeSpeedCoroutine()
        {
            if (m_fadeSpeedCoroutine != null)
            {
                StopCoroutine(m_fadeSpeedCoroutine);
                m_fadeSpeedCoroutine = null;
            }
        }

        private void StartFadeVolumeCoroutine(float from, float to, float fadeTime)
        {
            m_fadeVolumeCoroutine = StartCoroutine(TweenFadeVolume(from, to, fadeTime)); 
        }
        private void StopFadeVolumeCoroutine()
        {
            if (m_fadeVolumeCoroutine != null)
            {
                StopCoroutine(m_fadeVolumeCoroutine);
                m_fadeVolumeCoroutine = null;
            }
        }

        private IEnumerator TweenFadeVolume(float from, float to, float fadeTime)
        {
            if (from < to)//淡入
            {
                GetAudio().UnPause();
            }
            
            float time = 0f;
            while (time <= fadeTime)
            {
                GetAudio().volume = (from > to) ? (to + (from - to) * Mathf.Pow(time / fadeTime, 3f)) : (from + (to - from) * (Mathf.Pow(time / fadeTime - 1f, 3f) + 1.0f)); ;
                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
            if (from > to)//淡出
            {
                GetAudio().Pause();
            }

            GetAudio().volume = to;
            m_fadeVolumeCoroutine = null;
        }
        
        private IEnumerator TweenFadeSpeed(float from, float to, float fadeTime)
        {
            float time = 0f;
            GetAudio().pitch = from;
            while (time <= fadeTime)
            {
                GetAudio().pitch = (from > to) ? (to + (from - to) * Mathf.Pow(time / fadeTime, 3f)) : (from + (to - from) * (Mathf.Pow(time / fadeTime - 1f, 3f) + 1.0f));
                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
            GetAudio().pitch = to;
           
        }


        private IEnumerator WaitFinishByMoment(float delay)
        {
            float waitTime = (delay + Length) * UnityEngine.Time.timeScale;
            yield return new WaitForSeconds(waitTime);
            m_finishCoroutine = null;
            if (!IsLoop) onFinish?.Invoke();
        }

        private IEnumerator WaitFinishByStep()
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
