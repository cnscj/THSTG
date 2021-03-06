﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace XLibrary.Lang
{
    public class LFUCache<TKey, TValue>
    {
        internal sealed class LFUNode
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }
            public int Frequency { get; set; } = 0;
        }

        public event EventHandler<string> Log;

        Dictionary<TKey, LFUNode> _dictionary = new Dictionary<TKey, LFUNode>();
        Dictionary<int, LinkedList<LFUNode>> _linkedListDict = new Dictionary<int, LinkedList<LFUNode>>();
        private readonly int capacity;
        private SpinLock spinLock = new SpinLock();
        private SortedSet<int> frequencySortedSet = new SortedSet<int>();

        /// <summary>
        /// LFU 缓存容器
        /// </summary>
        /// <param name="capacity">缓存容量</param>
        public LFUCache(int capacity = 100)
        {
            if (capacity < 1)
            {
                throw new ArgumentException(nameof(capacity));
            }

            Log?.Invoke(this, $"创建 LFU 缓存容器[{this.GetHashCode():X}]：Capacity={capacity}");
            this.capacity = capacity;
        }

        #region 实体操作

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Add(TKey key, TValue value)
        {
            var lockSeed = false;
            if (!this.spinLock.IsHeldByCurrentThread)
            {
                this.spinLock.Enter(ref lockSeed);
            }

            LFUNode currentEntity = null;
            if (_dictionary.ContainsKey(key))
            {
                Log?.Invoke(this, $"覆盖已有的键：{key}={value}");
                currentEntity = _dictionary[key];
                currentEntity.Value = value;

                RemoveFromSet(currentEntity);
                currentEntity.Frequency++;
                AddToSet(currentEntity);
            }
            else
            {
                Log?.Invoke(this, $"新增缓存：{key}={value}");
                if (_dictionary.Count == capacity)
                {
                    Log?.Invoke(this, $"缓存过多，需要删除最不常用缓存...");
                    var removeEntity = GetLeastFrequentEntity();
                    if (removeEntity != null)
                    {
                        Remove(removeEntity.Key);
                    }
                }

                if (_dictionary.Count < capacity)
                {
                    currentEntity = new LFUNode();
                    currentEntity.Key = key;
                    currentEntity.Value = value;
                    this._dictionary.Add(key, currentEntity);
                    AddToSet(currentEntity);
                }
            }

            if (lockSeed)
            {
                this.spinLock.Exit();
            }

        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void Remove(TKey key)
        {
            if (!this._dictionary.ContainsKey(key))
            {
                Log?.Invoke(this, $"无法删除不存在的Key：{key}");
                return;
            }

            var lockSeed = false;
            if (!this.spinLock.IsHeldByCurrentThread)
            {
                this.spinLock.Enter(ref lockSeed);
            }

            var currentEntity = this._dictionary[key];
            this._dictionary.Remove(key);
            Log?.Invoke(this, $"删除缓存：{currentEntity.Key}={currentEntity.Value}");

            RemoveFromSet(currentEntity);
            if (lockSeed)
            {
                this.spinLock.Exit();
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            if (!this._dictionary.ContainsKey(key))
            {
                Log?.Invoke(this, $"使用Key不存在的缓存：{key}");
                return default;
            }

            var lockSeed = false;
            if (!this.spinLock.IsHeldByCurrentThread)
            {
                this.spinLock.Enter(ref lockSeed);
            }

            var currentEntity = this._dictionary[key];
            Log?.Invoke(this, $"使用缓存：{currentEntity.Key}={currentEntity.Value}");

            RemoveFromSet(currentEntity);
            currentEntity.Frequency++;
            AddToSet(currentEntity);

            if (lockSeed)
            {
                this.spinLock.Exit();
            }

            return currentEntity.Value;
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _linkedListDict.Clear();
            _dictionary.Clear();
        }
        #endregion

        #region 频率操作

        /// <summary>
        /// 获取最不常用缓存
        /// </summary>
        /// <returns></returns>
        private LFUNode GetLeastFrequentEntity()
        {
            if (frequencySortedSet.Count == 0)
                return default;
            var leastFrequency = frequencySortedSet.Min;
            var result = _linkedListDict.ContainsKey(leastFrequency) ?
                _linkedListDict[leastFrequency].First() :
                null;
            Log?.Invoke(this, $"最不常用缓存：{(result == null ? "[null]" : $"{result.Key.ToString()} (频率={leastFrequency})")}");
            return result;
        }

        /// <summary>
        /// 从频率Set移除缓存
        /// </summary>
        /// <param name="currentEntity"></param>
        private void RemoveFromSet(LFUNode currentEntity)
        {
            if (!_linkedListDict.ContainsKey(currentEntity.Frequency))
                return;
            var frequencySet = _linkedListDict[currentEntity.Frequency];
            frequencySet.Remove(currentEntity);
            Log?.Invoke(this, $"从频率Set移除缓存：{$"{currentEntity.Key.ToString()} (频率={currentEntity.Frequency})"}");
            if (frequencySortedSet.Min == currentEntity.Frequency && frequencySet.Count == 0)
            {
                Log?.Invoke(this, $"同时移除当前的空频率Set：{currentEntity.Frequency}");
                _linkedListDict.Remove(currentEntity.Frequency);
                frequencySortedSet.Remove(frequencySortedSet.Min);
            }
        }

        /// <summary>
        /// 添加缓存到Set
        /// </summary>
        /// <param name="currentEntity"></param>
        private void AddToSet(LFUNode currentEntity)
        {
            if (!_linkedListDict.ContainsKey(currentEntity.Frequency))
            {
                Log?.Invoke(this, $"新建频率Set：{currentEntity.Frequency}");
                _linkedListDict.Add(currentEntity.Frequency, new LinkedList<LFUNode>());
                frequencySortedSet.Add(currentEntity.Frequency);
            }

            if (!_linkedListDict[currentEntity.Frequency].Contains(currentEntity))
            {
                Log?.Invoke(this, $"向频率Set里增加缓存：{$"{currentEntity.Key.ToString()} (频率={currentEntity.Frequency})"}");
                _linkedListDict[currentEntity.Frequency].AddLast(currentEntity);
            }
        }

        /// <summary>
        /// 获取频率集合
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, List<TKey>> GetFrequencyList()
        {
            var lockSeed = false;
            if (!this.spinLock.IsHeldByCurrentThread)
            {
                this.spinLock.Enter(ref lockSeed);
            }

            var result = new Dictionary<int, List<TKey>>();
            foreach (var frequency in frequencySortedSet)
            {
                var cacheKeys = _linkedListDict[frequency].Select(entity => entity.Key).ToList();
                result.Add(frequency, cacheKeys);
            }

            if (lockSeed)
            {
                this.spinLock.Exit();
            }

            Log?.Invoke(this, $"获取EntityDictionary列表：\n\t{string.Join("\n\t", this._dictionary.Keys)}");
            Log?.Invoke(this, $"获取频率字典列表：\n\t{string.Join("\n\t", result.Select(pair => $"频率={pair.Key} : {string.Join("、", pair.Value)}"))}");

            return result;
        }
        #endregion
    }

}
