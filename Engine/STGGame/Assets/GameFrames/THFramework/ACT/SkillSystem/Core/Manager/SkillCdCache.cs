using System;
using System.Collections.Generic;
using UnityEngine;
namespace THGame
{
    //当个CD拥有使用次数,当达到使用次数时才能真正进入冷却,或者可能冷却独立算
    public class SkillCdCache : MonoBehaviour                   //CD冷却Cache
    {
        public float queryFrequentness = 0.1f;                  //查询频度0.1

        private Dictionary<string, SkillCdCacheData> _cdDict;

        private HashSet<SkillCdCacheData> _cdingDict;
        private Queue<SkillCdCacheData> _releaseQueue;

        private float _lastQueryTimeStamp;

        public SkillCdCacheData AddCd(string key, float cd, Action action = null)
        {
            var data = AddCd(key);
            data.maxCd = cd;
            data.callback = action;
            data.timeStamp = GetCurrTimeStamp();

            if (!_cdingDict.Contains(data))
            {
                _cdingDict.Add(data);
            }
            return data;
        }

        public SkillCdCacheData AddCd(string key)
        {
            SkillCdCacheData data;
            if (!_cdDict.TryGetValue(key, out data))
            {
                data = GetOrCreateData();
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
                _cdingDict?.Remove(data);
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

        public void ClearCd(string key,bool isCallback = true)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return;

            if (_cdDict.TryGetValue(key, out var data))
            {
                if (isCallback) data.callback?.Invoke();
                data.usedTimes = 0;
                data.timeStamp = -1;

                _cdingDict?.Remove(data);
            }
        }

        public void Clear()
        {
            _cdingDict?.Clear();
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
            if (_cdingDict == null || _cdingDict.Count <= 0)
                return;

            foreach(var data in _cdingDict)
            {
                if (GetCurrTimeStamp() >= data.maxCd + data.timeStamp)
                {
                    data.callback?.Invoke();
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
                _cdingDict.Remove(data);
            }
        }

        protected Dictionary<string, SkillCdCacheData> GetOrCreateDict()
        {
            _cdDict = _cdDict ?? new Dictionary<string, SkillCdCacheData>();
            return _cdDict;
        }

        protected HashSet<SkillCdCacheData> GetOrCreateCdingDict()
        {
            _cdingDict = _cdingDict ?? new HashSet<SkillCdCacheData>();
            return _cdingDict;
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