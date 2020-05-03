using System.Collections.Generic;

namespace THGame
{
    public class SoundObjectCache<T> where T :class, new()
    {
        public int maxCount = -1;
        private HashSet<T> m_recordDict = new HashSet<T>();
        private Queue<T> m_available = new Queue<T>();

        public T GetOrCreate()
        {
            if (m_available.Count <= 0)
            {
                var newObj = new T();
                m_available.Enqueue(newObj);
                m_recordDict.Add(newObj);
            }
            return m_available.Dequeue();
        }

        public void Release(T args)
        {
            if (!m_recordDict.Contains(args))
                return;

            if (maxCount > 0 && m_available.Count >= maxCount)
                return;

            m_available.Enqueue(args);
        }

        public void Fill(int count)
        {
            if (maxCount > 0)
            {
                while(m_available.Count < maxCount && m_available.Count < count)
                {
                    Release(GetOrCreate());
                }
            }
            else
            {
                while (m_available.Count < count)
                {
                    Release(GetOrCreate());
                }
            }
        }

        public void Clear()
        {
            if (m_available != null)
            {
                m_available.Clear();
            }
        }

    }

}
