using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundController : MonoBehaviour
    {
        public delegate void FinishCallback(SoundController ctrl);
        //音频源控件
        public new AudioSource audio;

        public FinishCallback onFinish;

        private float m_volume = 1f;
        public object m_userData;
        private Coroutine m_fadeSpeedCoroutine = null;
        private Coroutine m_fadeVolumeCoroutine = null;
        private Coroutine m_finishByStepCoroutine = null;
        private Coroutine m_finishByMonentCoroutine = null;

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
                return !IsPlaying && NormalizedTime >= 0f;
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
                UpdateFinishCoroutine(false);
            }
        }

        public float Volume
        {
            get { return m_volume; }
            set
            {
                m_volume = value;
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
                UpdateFinishCoroutine(false);
            }
        }

        public float Time
        {
            get { return GetAudio().time; }
            set {
                GetAudio().time = value;
                UpdateFinishCoroutine(true);
            }
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
                UpdateFinishCoroutine(true);
            }
        }

        public float Length
        {
            get { return (GetAudio().clip ? GetAudio().clip.length : 0f); }
        }

        public object CustomData
        {
            get
            {
                return m_userData;
            }
            set
            {
                m_userData = value;
            }
        }

        public AudioSource GetAudio()
        {
            if (audio == null)
            {
                audio = gameObject.GetComponent<AudioSource>();
                if (audio == null)
                {
                    audio = gameObject.AddComponent<AudioSource>();
                    audio.playOnAwake = false;
                }
            }

            return audio;
        }

        //
        public void Play(AudioClip clip = null, float delay = 0f)
        {
            Stop();
            AudioClip realClip = clip ?? GetAudio().clip;

            if (realClip != null) 
            {
                GetAudio().clip = realClip;
                GetAudio().volume = m_volume;
                GetAudio().PlayDelayed(delay);

                StartFinishCoroutine(delay);
            }
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
        private void UpdateFinishCoroutine(bool isForce, float delay = 0)
        {
            if (onFinish == null)
            {
                StopFinishCoroutine();
                return;
            }

            if (IsLoop)
            {
                StopFinishCoroutine();
                return;
            }

            //如果播放速度没有改变,就不用每帧update了,否则用每帧的
            if (Pitch == 1.0f)
            {
                if (m_finishByStepCoroutine != null)
                {
                    StopCoroutine(m_finishByStepCoroutine);
                    m_finishByStepCoroutine = null;
                }
                if (isForce)
                {
                    if (m_finishByMonentCoroutine != null)
                    {
                        StopCoroutine(m_finishByMonentCoroutine);
                        m_finishByMonentCoroutine = null;
                    }
                }
                if (m_finishByMonentCoroutine == null)
                {
                    m_finishByMonentCoroutine = StartCoroutine(WaitFinishByMoment(delay));
                }
            }
            else
            {
                if (m_finishByMonentCoroutine != null)
                {
                    StopCoroutine(m_finishByMonentCoroutine);
                    m_finishByMonentCoroutine = null;
                }
                if (m_finishByStepCoroutine == null)
                {
                    m_finishByStepCoroutine = StartCoroutine(WaitFinishByStep());
                }
            }
        }
        private void StartFinishCoroutine(float delay)
        {
            StopFinishCoroutine();
            UpdateFinishCoroutine(false, delay);
        }

        private void StopFinishCoroutine()
        {
            if (m_finishByStepCoroutine != null)
            {
                StopCoroutine(m_finishByStepCoroutine);
                m_finishByStepCoroutine = null;
            }

            if (m_finishByMonentCoroutine != null)
            {
                StopCoroutine(m_finishByMonentCoroutine);
                m_finishByMonentCoroutine = null;
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
                GetAudio().volume = (from > to) ? (from + (to - from) * Mathf.Pow(time / fadeTime, 3f)) : (from + (to - from) * (Mathf.Pow(time / fadeTime - 1f, 3f) + 1.0f));
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
                GetAudio().pitch = (from > to) ? (from + (to - from) * Mathf.Pow(time / fadeTime, 3f)) : (from + (to - from) * (Mathf.Pow(time / fadeTime - 1f, 3f) + 1.0f));
                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
            GetAudio().pitch = to;
           
        }

        private IEnumerator WaitFinishByMoment(float delay)
        {
            float waitTime = (delay + Length) * UnityEngine.Time.timeScale;
            yield return new WaitForSeconds(waitTime);
            m_finishByMonentCoroutine = null;
            if (!IsLoop)
            { 
                onFinish?.Invoke(this);
                onFinish = null;
            }
        }

        private IEnumerator WaitFinishByStep()
        {
            while (NormalizedTime < 1f)
            {
                yield return null;  //每帧检查
            }
            //等待结束,执行回调
            m_finishByStepCoroutine = null;
            onFinish?.Invoke(this);
            onFinish = null;
        }

        private void OnDestroy()
        {
            StopFadeVolumeCoroutine();
            StopFinishCoroutine();
        }
    }


}
