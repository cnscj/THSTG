using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundPlayer : MonoBehaviour
    {
        private static readonly SoundArgs DEFAULT_ARGS = new SoundArgs();
        public int maxCount = 6;               //最大同时播放个数

        private float m_volume = 1f;
        private bool m_mute = false;
        private float m_speed = 1f;
        private bool m_isAbort = false;
        private int m_id = 0;

        private List<SoundCommand> m_readingSounds = new List<SoundCommand>();                                      //准备队列
        private Dictionary<int,SoundController> m_playingSounds = new Dictionary<int, SoundController>();           //播放队列
        private Queue<int> m_releaseSounds = new Queue<int>();                                                      //释放队列

        private SoundPool m_poolObj;

        /// <summary>
        /// 播放器音量
        /// </summary>
        public float Volume
        {
            get { return m_volume; }
            set
            {
                m_volume = Mathf.Clamp(value, 0f, 1f);
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

        public int PlayList(SoundData data = null, SoundArgs args = null)
        {
            var id = PushSound(data, args);
            m_isAbort = false;
            return id;
        }

        public int PlayWait(SoundData data, SoundArgs args = null)
        {
            if (m_playingSounds.Count >= maxCount)
            {
                return PushSound(data, args);
            }
            else
            {
                return PlaySound(data, args);
            }
        }

        public int PlayForce(SoundData data, SoundArgs args = null)
        {
            if (m_playingSounds.Count >= maxCount)
            {
                //找到一个播放最久的踢掉
                KickoutOneSound(true);
            }
            return PlaySound(data, args);
        }

        public int Play(SoundData data, SoundArgs args = null)
        {

            if (m_playingSounds.Count < maxCount)
            {
                return PlaySound(data, args);
            }
            return -1;
        }

        public void Stop(int key)
        {
            if (m_playingSounds.TryGetValue(key,out var ctrl))
            {
                StopSound(key);
            }else
            {
                RemoveCommandFormReading(key);
            }
        }
        public void Stop()
        {
            foreach (var pair in m_playingSounds)
            {
                Stop(pair.Key);
                break;
            }
        }

        public void Pause(int key, float fadeOut = 0f)
        {
            if (m_playingSounds.TryGetValue(key, out var ctrl))
            {
                ctrl.Pause(fadeOut);
            }
        }
        public void Pause(float fadeOut = 0f)
        {
            foreach (var pair in m_playingSounds)
            {
                Pause(pair.Key, fadeOut);
                break;
            }
        }
        public void PauseAll(float fadeOut = 0f)
        {
            foreach (var pair in m_playingSounds)
            {
                Pause(pair.Key, fadeOut);
            }
        }

        public void Resume(int key, float fadeIn = 0f)
        {
            if (m_playingSounds.TryGetValue(key, out var ctrl))
            {
                ctrl.Resume(fadeIn);
            }
        }
        public void Resume(float fadeIn = 0f)
        {
            foreach (var pair in m_playingSounds)
            {
                Resume(pair.Key, fadeIn);
                break;
            }
        }

        public void ResumeAll(float fadeIn = 0f)
        {
            foreach (var pair in m_playingSounds)
            {
                Resume(pair.Key, fadeIn);
            }
        }

        /// <summary>
        /// 停止所有正作播放的,但是不会清空播放列表
        /// </summary>
        public void StopAll()
        {
            foreach (var pairs in m_playingSounds)
            {
                var ctrl = pairs.Value;
                ctrl.Stop();
                GetOrCreateSoundPool().Release(ctrl);
            }
            m_playingSounds.Clear();
            m_isAbort = true;
        }

        /// <summary>
        /// 清空播放列表
        /// </summary>
        public void ClearList()
        {
            m_readingSounds.Clear();
        }


        public void ClearAll()
        {
            StopAll();
            ClearList();
        }

        public void DisposeAll()
        {
            ClearAll();
            m_poolObj.Dispose();
        }
        
        private void Update()
        {
            //轮询
            PollState();
            PollSound();
        }
       
        private void KickoutOneSound(bool iskickLongTime)
        {
            int ctrlKey = -1;
            if (iskickLongTime)
            {
                float longTime = 0f;
                foreach (var pair in m_playingSounds)
                {
                    if (pair.Value.NormalizedTime >= longTime)
                    {
                        ctrlKey = pair.Key;
                    }
                }
            }
            else
            {
                foreach(var pair in m_playingSounds)
                {
                    ctrlKey = pair.Key;
                    break;
                }
            }

            if (ctrlKey > 0)
            {
                StopSound(ctrlKey);
            }
        }

        private int PushSound(SoundData data, SoundArgs args)
        {
            if (data == null)
                return -1;
            if (data.clip == null)
                return -1;

            SoundCommand command = NewCommand(data, args);
            m_readingSounds.Add(command);

            return command.id;
        }

        private void PollState()
        {
            //检查播放队列的播放状态
            if (m_playingSounds.Count > 0)
            {
                foreach (var soundPair in m_playingSounds)
                {
                    var soundCtrl = soundPair.Value;
                    if (!soundCtrl.IsLoop && soundCtrl.NormalizedTime >= 1f)
                    {
                        m_releaseSounds.Enqueue(soundPair.Key);
                    }
                }
            }
        }

        private void PollSound()
        {
            //被StopAll后,无法在继续播放,除非手动Play
            if (!m_isAbort)
            {
                //如果准备队列中还有音源,出队播放
                if (m_readingSounds.Count > 0)
                {
                    var command = m_readingSounds[0];
                    PlaySound(command);
                    m_readingSounds.RemoveAt(0);
                }
            }

            //如果释放队列中有音源,出队释放
            if (m_releaseSounds.Count > 0)
            {
                SoundController soundCtrl = null;
                int soundId = m_releaseSounds.Dequeue();
                if (m_playingSounds.TryGetValue(soundId, out soundCtrl))
                {
                    //TODO:冗余代码
                    ReleaseSound(soundCtrl);
                    m_playingSounds.Remove(soundId);
                }

            }
        }

        private void UpdateSpeed()
        {
            foreach (var pair in m_playingSounds)
            {
                pair.Value.Pitch = Speed;
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


        private int PlaySound(SoundCommand command)
        {
            if (command == null)
                return -1;
            if (command.data == null)
                return -1;
            if (command.data.clip == null)
                return -1;

            //入队并播放
            var id = command.id;
            var data = command.data;
            var args = command.args;

            var ctrl = GetOrCreateSoundController(command);
            ctrl.name = string.Format("{0}|{1}", data.clip.name, id);
            ctrl.Volume = args.volume * Volume;
            ctrl.Mute = args.mute && Mute;
            ctrl.IsLoop = args.isLoop;

            ctrl.Play(data.clip, args.delay);
            m_playingSounds.Add(id, ctrl);
            m_isAbort = false;

            return id;
        }

        private int PlaySound(SoundData data ,SoundArgs args)
        {
            return PlaySound(NewCommand(data, args));
        }

        private void StopSound(int key)
        {
            if (m_playingSounds.TryGetValue(key, out var ctrl))
            {
                ctrl.Stop();
                m_releaseSounds.Enqueue(key);
                m_playingSounds.Remove(key);
            }
        }

        private void ReleaseSound(SoundController ctrl)
        {
            ctrl.Stop();
            GetOrCreateSoundPool().Release(ctrl);
        }

        private string GetPoolObjectKey(SoundCommand command)
        {
            return string.Format("{0}", command.ToString());
        }

        private SoundController GetOrCreateSoundController(SoundCommand command)
        {
            var key = GetPoolObjectKey(command);
            return GetOrCreateSoundController(key);
        }

        private SoundController GetOrCreateSoundController(string key)
        {
            var pool = GetOrCreateSoundPool();
            SoundController ctrl = pool.GetOrCreate(key);
            ctrl.transform.SetParent(transform);

            return ctrl;
        }

        private SoundPool GetOrCreateSoundPool()
        {
            if (m_poolObj == null)
            {
                GameObject poolGobj = new GameObject("SoundPool");
                m_poolObj = poolGobj.AddComponent<SoundPool>();
                poolGobj.transform.SetParent(transform);
            }
            return m_poolObj;
        }

        private SoundCommand NewCommand(SoundData data, SoundArgs args)
        {
            var command = new SoundCommand();
            command.id = ++m_id;
            command.data = data;
            command.args = args ?? DEFAULT_ARGS;

            return command;
        }

        private SoundCommand FindCommandFormReading(int id)
        {
            SoundCommand ret = null;

            foreach(var command in m_readingSounds)
            {
                if (id == command.id)
                {
                    return command;
                }
            }
            return ret;
        }

        private void RemoveCommandFormReading(int id)
        {
            for( int i = m_readingSounds.Count - 1; i >= 0 ; i--)
            {
                var command = m_readingSounds[i];
                if (id == command.id)
                {
                    m_readingSounds.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
