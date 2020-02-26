using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundPlayer : MonoBehaviour
    {
        public int maxSoundCount = 6;               //最大同时播放个数

        private float m_volume;
        private bool m_mute;
        private float m_speed;

        private Queue<SoundData> m_readingSounds = new Queue<SoundData>();                                          //准备队列
        private Dictionary<string,SoundController> m_playingSounds = new Dictionary<string, SoundController>();     //播放队列
        private Queue<SoundController> m_releaseSounds = new Queue<SoundController>();                                          //销毁队列


        private SoundPool m_poolObj;

        /// <summary>
        /// 播放器音量
        /// </summary>
        public float Volume
        {
            get { return m_volume; }
            set
            {
                m_volume = Mathf.Clamp(value, 0, 1);
                UpdateVolume();
            }
        }

        /// <summary>
        /// 播放器静音
        /// </summary>
        public bool Mute
        {
            get { return m_mute; }
            set
            {
                m_mute = value;
                UpdateMute();
            }
        }

        /// <summary>
        /// 播放器速度
        /// </summary>
        public float Speed
        {
            get { return m_speed; }
            set
            {
                m_speed = value;
                UpdateSpeed();
            }
        }

        public void Play(string key, SoundArgs args)
        {
            //
            if (m_playingSounds.Count >= maxSoundCount)
            {
                //如果是强制播放
                if (args.isForceReplay)
                {

                }
                else
                {
                    var data = new SoundData();
                    data.key = key;
                    data.args = args;
                    m_readingSounds.Enqueue(data);
                }
                
            }
            else
            {
                PlaySound(key, args);
            }
        }

        public void Stop(string key)
        {
            if (m_playingSounds.TryGetValue(key,out var ctrl))
            {
                ctrl.Stop();
            }

        }

        public void Pause(string key, float fadeOut = 0f)
        {
            if (m_playingSounds.TryGetValue(key, out var ctrl))
            {
                ctrl.Pause(fadeOut);
            }
        }

        public void Resume(string key, float fadeIn = 0f)
        {
            if (m_playingSounds.TryGetValue(key, out var ctrl))
            {
                ctrl.Resume(fadeIn);
            }
        }

        public void StopAll()
        {
            foreach (var pairs in m_playingSounds)
            {
                var ctrl = pairs.Value;
                ctrl.Stop();
                GetorCreateSoundPool().Release(ctrl);
            }
            m_playingSounds.Clear();
        }
        //
        private void Update()
        {
            //轮询
            PollSound();
        }
        //TODO:
        private void PollSound()
        {
            //如果准备队列中还有音源,出队播放
            if (m_readingSounds.Count > 0)
            {
                var data = m_readingSounds.Dequeue();
                PlaySound(data.key, data.args);
            }

            //如果释放队列中有音源,出队释放
            if (m_releaseSounds.Count > 0)
            {
                var soundCtrl = m_releaseSounds.Dequeue();

            }
        }

        private float GetRealSpeed()
        {
            return m_speed;
        }
  
        private void UpdateSpeed()
        {
            foreach (var pair in m_playingSounds)
            {
                pair.Value.Pitch = GetRealSpeed();
            }

        }

        private void UpdateVolume()
        {
            foreach(var pair in m_playingSounds)
            {
                pair.Value.Volume = Volume;
            }
        }

        private void UpdateMute()
        {
            foreach (var pair in m_playingSounds)
            {
                pair.Value.Mute = Mute;
            }
        }

        private void PlaySound(string key, SoundArgs args)
        {
            //入队并播放
            //TODO:检查是否存在可用控制器
            var ctrl = GetOrCreateSoundController(key);
            
            m_playingSounds.Add(key, ctrl);

        }

        private SoundController GetOrCreateSoundController(string key)
        {
            var pool = GetorCreateSoundPool();
            SoundController ctrl = pool.GetOrCreate(key);
            
            return ctrl;
        }

        private SoundPool GetorCreateSoundPool()
        {
            if (m_poolObj == null)
            {
                GameObject poolGobj = new GameObject("SoundPool");
                m_poolObj = poolGobj.AddComponent<SoundPool>();
            }
            return m_poolObj;
        }

        //用于判断声音是否播放结束
        private IEnumerator UpdateSoundState()
        {
            //检查播放队列的播放状态
            foreach(var soundPair in m_playingSounds)
            {
                var soundCtrl = soundPair.Value;
                if (!soundCtrl.IsLoop && soundCtrl.NormalizedTime >= 1f)
                {
                    m_releaseSounds.Enqueue(soundCtrl);
                }
            }
            yield return null;
        }

    }


}
