using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SoundPlayer : MonoBehaviour
    {
        private static readonly SoundArgs DEFAULT_ARGS = new SoundArgs();
        public int maxCount = 6;               //最大同时播放个数,-1无限制

        private float m_volume = 1f;
        private bool m_mute = false;
        private float m_speed = 1f;
        private bool m_isAbort = false;
        private int m_id = 0;

        private LinkedList<SoundCommand> m_readingSounds = new LinkedList<SoundCommand>();                                      //准备队列
        private Dictionary<int,KeyValuePair<SoundArgs,SoundController>> m_playingSounds = new Dictionary<int, KeyValuePair<SoundArgs, SoundController>>();           //播放队列
        private Queue<int> m_releaseSounds = new Queue<int>();                                                                  //释放队列

        private Dictionary<string, int> m_curTagCount = new Dictionary<string, int>();  //当前最大个数
        private Dictionary<string, int> m_maxTagCount = new Dictionary<string, int>();  //最大同类型播放个数
        private Coroutine m_fadeVolumeCoroutine = null;
        private SoundPool m_poolObj;
        private SoundArgsCache m_argsCache;

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
        /// 正作播放的音效个数
        /// </summary>
        public int Count
        {
            get { return m_playingSounds.Count; }
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

        ///////////////////
 
        public int PlayList(SoundData data = null, SoundArgs args = null)
        {
            if (!IsTagCanPlay(args))
                return -1;

            var id = PushSound(data, args);
            m_isAbort = false;
            return id;
        }

        public int PlayWait(SoundData data, SoundArgs args = null)
        {
            if (!IsTagCanPlay(args))
                return -1;

            if (maxCount >= 0 && m_playingSounds.Count >= maxCount)
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
            if (!IsTagCanPlay(args))
                return -1;

            while (maxCount >= 0 && m_playingSounds.Count >= maxCount)
            {
                //找到一个播放最久的踢掉
                KickoutOneSound(true);
            }

            return PlaySound(data, args);
        }

        public int Play(SoundData data, SoundArgs args = null)
        {
            if (!IsTagCanPlay(args))
                return -1;

            if (maxCount < 0 || m_playingSounds.Count < maxCount)
            {
                return PlaySound(data, args);
            }
            return -1;
        }

        public void Stop(int key)
        {
            if (m_playingSounds.ContainsKey(key))
            {
                StopSound(key);
            }
            else
            {
                RemoveCommandFromReading(key);
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
            if (m_playingSounds.TryGetValue(key, out var pair))
            {
                pair.Value.Pause(fadeOut);
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
            if (m_playingSounds.TryGetValue(key, out var pair))
            {
                pair.Value.Resume(fadeIn);
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
            foreach (var pair in m_playingSounds)
            {
                var ctrl = pair.Value.Value;
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
            ClearCommandFromReading();
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
        ///
        public void SetTagMaxCount(string key, int count)
        {
            if(!string.IsNullOrEmpty(key))
            {
                m_maxTagCount[key] = count;
            }
        }
        public int GetTagMaxCount(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (m_maxTagCount.TryGetValue(key,out var count))
                {
                    return count;
                }
            }
            return maxCount >= 0 ? maxCount : int.MaxValue;
        }
        public void TweenVolume(float from, float to, float fadeTime)
        {
            if (m_fadeVolumeCoroutine != null)
            {
                StopCoroutine(m_fadeVolumeCoroutine);
                m_fadeVolumeCoroutine = null;
            }
            m_fadeVolumeCoroutine = StartCoroutine(TweenFadeVolume(from, to, fadeTime));
        }

        public int[] GetSounds(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return null;

            List<int> ret = new List<int>();
            foreach(var playingPair in m_playingSounds)
            {
                if (!string.IsNullOrEmpty(playingPair.Value.Key.tag))
                {
                    if (playingPair.Value.Key.tag == tag)
                    {
                        ret.Add(playingPair.Key);
                    }
                }
            }

            foreach (var reading in m_readingSounds)
            {
                if (!string.IsNullOrEmpty(reading.args.tag))
                {
                    if (reading.args.tag == tag)
                    {
                        ret.Add(reading.id);
                    }
                }
            }

            return ret.ToArray();
        }

        public SoundArgs GetOrCreateArgs()
        {
            return GetOrCreateSoundArgsCache().GetOrCreate();
        }
            
        private void Update()
        {
            //轮询
            PollState();
            PollSound();
        }
        private void OnDestroy()
        {
            if (m_fadeVolumeCoroutine != null)
            {
                StopCoroutine(m_fadeVolumeCoroutine);
                m_fadeVolumeCoroutine = null;
            }
        }

        private void KickoutOneSound(bool isKickLongTime)
        {
            int ctrlKey = -1;
            int pauseCtrlKey = -1;
            int longCtrlKey = -1;
            int lastCtrlKey = -1;
            float longTime = 0f;
            //如果有暂停的,先干掉暂停的
            foreach (var pair in m_playingSounds)
            {
                if (pair.Value.Value.IsPause)
                {
                    pauseCtrlKey = pair.Key;
                    break;
                }

                if (isKickLongTime)
                {
                    var ctrl = pair.Value.Value;
                    if (ctrl.NormalizedTime >= longTime)
                    {
                        longCtrlKey = pair.Key;
                    }
                }

                lastCtrlKey = pair.Key;
            }

            if (pauseCtrlKey > 0)
            {
                ctrlKey = pauseCtrlKey;
            }
            else
            {
                if (isKickLongTime)
                {
                    ctrlKey = longCtrlKey;
                }
                else
                {
                    ctrlKey = lastCtrlKey;
                }     
            }

            if (ctrlKey > 0)
            {
                ReleaseSound(ctrlKey);
            }
        }

        private int PushSound(SoundData data, SoundArgs args)
        {
            if (data == null)
                return -1;
            if (data.clip == null)
                return -1;

            SoundCommand command = NewCommand(data, args);
            m_readingSounds.AddLast(command);

            return command.id;
        }

        private void PollState()
        {
            //检查播放队列的播放状态
            if (m_playingSounds.Count > 0)
            {
                foreach (var soundPair in m_playingSounds)
                {
                    var soundCtrl = soundPair.Value.Value;
                    var soundArgs = soundPair.Value.Key;
                    if (!soundCtrl.IsLoop)
                    {
                        if (soundCtrl.IsPlaying)
                        {
                            if (soundCtrl.NormalizedTime < 1f)
                            {
                                continue;
                            }
                        }

                        if (soundArgs.endTime >= 0f && soundCtrl.Time < soundArgs.endTime)
                        {
                            continue;
                        }
                        
                        //大量循环居然没有执行到这里
                        FinishSound(soundPair.Key);
                    }
                }
            }
        }

        private void PollSound()
        {
            //如果释放队列中有音源,出队释放
            while (m_releaseSounds.Count > 0)
            {
                int soundId = m_releaseSounds.Dequeue();
                ReleaseSound(soundId);
            }

            //被StopAll后,无法在继续播放,除非手动Play
            if (!m_isAbort)
            {
                //如果准备队列中还有音源,出队播放
                if (m_readingSounds.Count > 0)
                {
                    while (maxCount < 0 || m_playingSounds.Count < maxCount)
                    {
                        var command = m_readingSounds.First.Value;
                        m_readingSounds.RemoveFirst();
                        if (IsTagCanPlay(command))
                        {
                            PlaySound(command);
                        }
                    }
                }
            }
        }

        private void UpdateSpeed()
        {
            foreach (var pair in m_playingSounds)
            {
                pair.Value.Value.Pitch = Speed;
            }
        }

        private void UpdateVolume()
        {
            foreach(var pair in m_playingSounds)
            {
                pair.Value.Value.Volume = Volume;
            }
        }

        private void UpdateMute()
        {
            foreach (var pair in m_playingSounds)
            {
                pair.Value.Value.Mute = Mute;
            }
        }

        private int GetCurTagCount(string key)
        {
            if (m_curTagCount.TryGetValue(key,out var count))
            {
                return count;
            }
            return 0;
        }

        private bool IsTagCanPlay(SoundArgs args)
        {
            if (args != null)
            {
                if (!string.IsNullOrEmpty(args.tag))
                {
                    var curCount = GetCurTagCount(args.tag);
                    var maxCount = GetTagMaxCount(args.tag);
                    return curCount < maxCount;
                }
            }
            return true;
        }
        private bool IsTagCanPlay(SoundCommand command)
        {
            if (command != null)
            {
                return IsTagCanPlay(command.args);
            }
            return true;
        }

        private int PlaySound(SoundCommand command)
        {
            if (command == null)
                return -1;
            if (command.data == null)
                return -1;
            if (command.data.clip == null)
                return -1;
            if (!IsTagCanPlay(command?.args))
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
            ctrl.Time = args.startTime;

            ctrl.Play(data.clip, args.delay);
            m_playingSounds.Add(id, new KeyValuePair<SoundArgs, SoundController>(args, ctrl));
            if (!string.IsNullOrEmpty(args.tag))
            {
                if (m_curTagCount.ContainsKey(args.tag)) m_curTagCount[args.tag]++;
                else m_curTagCount[args.tag] = 1;
            }
            m_isAbort = false;

            return id;
        }

        private int PlaySound(SoundData data ,SoundArgs args)
        {
            return PlaySound(NewCommand(data, args));
        }

        private void StopSound(int key)
        {
            if (m_playingSounds.TryGetValue(key, out var pair))
            {
                ReleaseSound(pair.Value);
                m_playingSounds.Remove(key);
            }
        }

        private void FinishSound(int key)
        {
            if (m_playingSounds.TryGetValue(key, out var pair))
            {
                var args = pair.Key;
                args?.onCompleted?.Invoke();
                m_releaseSounds.Enqueue(key);   //因为在循环中不能直接释放,留到释放队列释放
            }
        }

        private void ReleaseSound(int key)
        {
            if (m_playingSounds.TryGetValue(key, out var pair))
            {
                pair.Value.Stop();
                if (!string.IsNullOrEmpty(pair.Key.tag))
                {
                    if (m_curTagCount.ContainsKey(pair.Key.tag)) m_curTagCount[pair.Key.tag]--;
                    else m_curTagCount[pair.Key.tag] = 1;
                }

                GetOrCreateSoundPool().Release(pair.Value);
                GetOrCreateSoundArgsCache().Release(pair.Key);
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
            string key = string.Format("Crtl");//string.Format("{0}", command?.data?.clip?.GetHashCode());
            return key;
        }

        private SoundArgsCache GetOrCreateSoundArgsCache()
        {
            if (m_argsCache == null)
            {
                m_argsCache = new SoundArgsCache();
            }
            return m_argsCache;
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

        private SoundCommand FindCommandFromReading(int id)
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

        private void RemoveCommandFromReading(int id)
        {
            for (LinkedListNode<SoundCommand> iterNode = m_readingSounds.Last; iterNode != null; iterNode = iterNode.Previous)
            {
                var command = iterNode.Value;
                if (id == command.id)
                {
                    m_readingSounds.Remove(iterNode);
                    GetOrCreateSoundArgsCache().Release(command.args);
                    break;
                }
            }
        }

        private void ClearCommandFromReading()
        {
            for (LinkedListNode<SoundCommand> iterNode = m_readingSounds.Last; iterNode != null; iterNode = iterNode.Previous)
            {
                var command = iterNode.Value;
                GetOrCreateSoundArgsCache().Release(command.args);
            }
            m_readingSounds.Clear();
        }

        private IEnumerator TweenFadeVolume(float from, float to, float fadeTime)
        {
            Volume = from;
            float time = 0f;
            while (time <= fadeTime)
            {
                Volume = (from > to) ? (from + (to - from) * Mathf.Pow(time / fadeTime, 3f)) : (from + (to - from) * (Mathf.Pow(time / fadeTime - 1f, 3f) + 1.0f));
                time += UnityEngine.Time.deltaTime;
                yield return null;
            }
            Volume = to;
            m_fadeVolumeCoroutine = null;
        }
    }
}
