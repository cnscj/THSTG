using System;
using System.Collections.Generic;
using UnityEngine;
namespace THGame
{
    public class SkillCdCache : MonoBehaviour                //CD冷却Cache
    {
        public float queryFrequentness = 0.1f;             //查询频度0.1

        private Dictionary<string, SkillCdCacheData> _cdDict;
        private Queue<SkillCdCacheData> _releaseQueue;
        private float _lastQueryTimeStamp;

        public void AddCd(string key,float cd, Action action = null)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return;

            if (_cdDict.ContainsKey(key))
                return;

            var data = GetOrCreateData();
            data.key = key;
            data.timeStamp = GetCurrTimeStamp();
            data.maxCd = cd;
            data.callback = action;

            GetOrCreateDict()[key] = data;
        }

        public void RemoveCd(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return;

            _cdDict.Remove(key);
        }

        public bool IsHaveCd(string key)
        {
            if (_cdDict == null || _cdDict.Count <= 0)
                return false;

            return _cdDict.ContainsKey(key);
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
                GetOrCreateReleaseQueue().Enqueue(data);
            }
        }

        public void Clear()
        {
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
            if (_cdDict == null || _cdDict.Count <= 0)
                return;

            foreach(var data in _cdDict.Values)
            {
                if (GetCurrTimeStamp() >= data.maxCd + data.timeStamp)
                {
                    data.callback?.Invoke();
                    GetOrCreateReleaseQueue().Enqueue(data);
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
                _cdDict.Remove(data.key);
                ReleaseData(data);
            }
        }

        protected Dictionary<string, SkillCdCacheData> GetOrCreateDict()
        {
            _cdDict = _cdDict ?? new Dictionary<string, SkillCdCacheData>();
            return _cdDict;
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