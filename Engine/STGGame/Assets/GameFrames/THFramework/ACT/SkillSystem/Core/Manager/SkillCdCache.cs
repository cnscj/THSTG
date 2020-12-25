using System;
using System.Collections.Generic;
using UnityEngine;
namespace THGame
{
    //当个CD拥有使用次数,当达到使用次数时才能真正进入冷却,或者可能冷却独立算
    public class SkillCdCache : MonoBehaviour                   //CD冷却Cache
    {
        public float queryFrequentness = 0.1f;                  //查询频度0.1
        public event Action<SkillCdCacheData> OnCdDone;         //

        private Dictionary<IComparable, SkillCdCacheData> _cdDict;

        private HashSet<SkillCdCacheData> _tickDict;
        private Queue<SkillCdCacheData> _releaseQueue;

        private float _lastQueryTimeStamp;

        public SkillCdCacheData TickCd(IComparable key)
        {
            var data = AddCd(key);
            var tickDict = GetOrCreateTickDict();
            if (!tickDict.Contains(data))
            {
                data.usedTimes = 0;
                data.timeStamp = GetCurrTimeStamp();
                tickDict.Add(data);
            }

            return data;
        }

        public SkillCdCacheData TickCd(IComparable key, float maxCd, int maxTimes = 1)
        {
            var data = AddCd(key);
            data.maxCd = maxCd;
            data.maxTimes = maxTimes;
            
            return TickCd(key);
        }

        public SkillCdCacheData AddCd(IComparable key)
        {
            var cdCahce = GetOrCreateDict();
            if (!cdCahce.TryGetValue(key, out SkillCdCacheData data))
            {
                data = GetOrCreateData();
                data.key = key;

                GetOrCreateDict()[key] = data;
            }

            return data;
        }

        public void RemoveCd(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return;

            if (_cdDict.TryGetValue(key, out SkillCdCacheData data))
            {
                _tickDict?.Remove(data);
                _cdDict.Remove(key);
            }
        }

        public SkillCdCacheData GetCdData(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return default;

            return GetOrCreateDict()[key];
        }

        public bool IsInCd(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return false;

            if (_cdDict.TryGetValue(key,out var data))
            {
                return GetCurrTimeStamp() < data.maxCd + data.timeStamp;
            }

            return false;
        }

        public float GetCdTime(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return 0;

            if (_cdDict.TryGetValue(key, out var data))
            {
                return data.maxCd;
            }

            return 0;
        }

        public float GetCdEndTime(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return 0;

            if (_cdDict.TryGetValue(key, out var data))
            {
                return data.timeStamp + data.maxCd;
            }

            return 0;
        }

        public float GetCdBeginTime(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return 0;

            if (_cdDict.TryGetValue(key, out var data))
            {
                return data.timeStamp;
            }

            return 0;
        }

        public void ClearCd(string key, bool isCallback = true)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return;

            if (_cdDict.TryGetValue(key, out var data))
            {
                if (isCallback) OnCdDone?.Invoke(data);
                data.usedTimes = 0;
                data.timeStamp = -1;

                _tickDict?.Remove(data);
            }
        }

        public void Clear()
        {
            _tickDict?.Clear();
            _releaseQueue?.Clear();
            _cdDict?.Clear();
        }

        public void Update()
        {
            if (GetCurrTimeStamp() < _lastQueryTimeStamp + queryFrequentness)
                return;

            QueryCache();

            _lastQueryTimeStamp = GetCurrTimeStamp();
        }

        protected void QueryCache()
        {
            UpdateExect();
            UpdateRelease();
        }

        protected void UpdateExect()
        {
            if (_tickDict == null || _tickDict.Count <= 0)
                return;

            foreach(var data in _tickDict)
            {
                if (GetCurrTimeStamp() >= data.maxCd + data.timeStamp)
                {
                    OnCdDone?.Invoke(data);
                    data.timeStamp = GetCurrTimeStamp();
                    data.usedTimes++;
                    if (data.usedTimes >= data.maxTimes)
                    {
                        GetOrCreateReleaseQueue().Enqueue(data);
                    }
                }
            }
        }

        protected void UpdateRelease()
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return;

            if (_releaseQueue == null)
                return;

            while(_releaseQueue.Count > 0)
            {
                var data = _releaseQueue.Dequeue();
                _tickDict.Remove(data);
            }
        }

        protected Dictionary<IComparable, SkillCdCacheData> GetOrCreateDict()
        {
            _cdDict = _cdDict ?? new Dictionary<IComparable, SkillCdCacheData>();
            return _cdDict;
        }

        protected HashSet<SkillCdCacheData> GetOrCreateTickDict()
        {
            _tickDict = _tickDict ?? new HashSet<SkillCdCacheData>();
            return _tickDict;
        }

        protected Queue<SkillCdCacheData> GetOrCreateReleaseQueue()
        {
            _releaseQueue = _releaseQueue ?? new Queue<SkillCdCacheData>();
            return _releaseQueue;
        }

        protected SkillCdCacheData GetOrCreateData()
        {
            return new SkillCdCacheData();
        }

        protected void ReleaseData(SkillCdCacheData data)
        {
            return;
        }

        protected float GetCurrTimeStamp()
        {
            return Time.fixedTime;
        }
    }


}