using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibrary.Lang
{
    public class Bundle : IEnumerable
    {
        private Dictionary<string, object> __dict;

        public object this[string key]
        {
            get
            {
                if (__dict != null && __dict.ContainsKey(key))
                    return __dict[key];
                return null;
            }
            set
            {
                GetOrCreateDict()[key] = value;
            }
        }

        public void PutObject(string key, object value)
        {
            GetOrCreateDict().Add(key, value);
        }

        public object GetObject(string key)
        {
            if (__dict == null)
                return default;

            return __dict[key];
        }

        public void PutString(string key,string value)
        {
            GetOrCreateDict().Add(key, value);
        }

        public string GetString(string key)
        {
            if (__dict == null)
                return default;

            return __dict[key].ToString();
        }

        public void PutInt(string key, int value)
        {
            GetOrCreateDict().Add(key, value);
        }

        public int GetInt(string key)
        {
            if (__dict == null)
                return default;

            return __dict[key].ToInt();
        }

        public void PutBool(string key, bool value)
        {
            GetOrCreateDict().Add(key, value);
        }

        public bool GetBool(string key)
        {
            if (__dict == null)
                return default;

            return __dict[key].ToBool();
        }

        public void PutDouble(string key, double value)
        {
            GetOrCreateDict().Add(key, value);
        }

        public double GetDouble(string key)
        {
            if (__dict == null)
                return default;

            return __dict[key].ToDouble();
        }

        public bool IsContains(string key)
        {
            if (__dict == null)
                return false;

            return __dict.ContainsKey(key);
        }

        public void Remove(string key)
        {
            if (__dict == null)
                return;

            __dict.Remove(key);
        }

        public void Clear()
        {
            if (__dict == null)
                return;

            __dict.Clear();
        }

        private Dictionary<string, object> GetOrCreateDict()
        {
            __dict = __dict ?? new Dictionary<string, object>();
            return __dict;
        }

        public IEnumerator GetEnumerator()
        {
            if (__dict == null)
                yield break;

            foreach (var data in __dict.Values)
            {
                yield return data;
            }
        }
    }
}
