using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillTriggerFactory<T> : ISkillTriggerFactory where T : AbstractSkillTrigger,new() 
    {
        public int maxNum = 40;
        private Queue<T> _triggerPool;

        public T GetOrCreateInstance()
        {
            var pool = GetOrCreatePool();
            T instance = null;
            if (pool.Count <= 0)
            {
                instance = new T();
                RecycleInstance(instance);
            }
            instance = pool.Dequeue();
            return instance;

        }

        public void RecycleInstance(T instance)
        {
            if (instance == null)
                return;

            var pool = GetOrCreatePool();
            if (maxNum > 0 && pool.Count >= maxNum)
                return;

            pool.Enqueue(instance);
        }

        private Queue<T> GetOrCreatePool()
        {
            _triggerPool = _triggerPool ?? new Queue<T>();
            return _triggerPool;
        }

    }
}
