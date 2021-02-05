using System.Collections.Generic;

namespace THGame
{
    public class SkillTriggerFactory<T> : ISkillTriggerFactory where T : AbstractSkillTrigger, new() 
    {
        public int maxNum = 50;
        private Queue<T> _triggerPool;

        public string Type { get ; set ; }

        public AbstractSkillTrigger CreateTrigger()
        {
            var pool = GetOrCreatePool();
            T instance = null;
            if (pool.Count <= 0)
            {
                instance = new T();
                RecycleTrigger(instance);
            }
            instance = pool.Dequeue();
            return instance;
        }

        public void RecycleTrigger(AbstractSkillTrigger instance)
        {
            if (instance == null)
                return;

            var pool = GetOrCreatePool();
            if (maxNum > 0 && pool.Count >= maxNum)
                return;

            instance.Type = Type;
            pool.Enqueue((T)instance);
        }

        private Queue<T> GetOrCreatePool()
        {
            _triggerPool = _triggerPool ?? new Queue<T>();
            return _triggerPool;
        }

    }
}
