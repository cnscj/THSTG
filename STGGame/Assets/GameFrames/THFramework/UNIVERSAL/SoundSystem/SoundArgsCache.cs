using System.Collections.Generic;

namespace THGame
{
    public class SoundArgsCache
    {
        public int maxCount = -1;
        private HashSet<SoundArgs> m_recordDict = new HashSet<SoundArgs>();
        private Queue<SoundArgs> m_available = new Queue<SoundArgs>();

        public SoundArgs GetOrCreate()
        {
            if (m_available.Count <= 0)
            {
                var newArgs = new SoundArgs();
                m_available.Enqueue(newArgs);
                m_recordDict.Add(newArgs);
            }
            return m_available.Dequeue();
        }

        public void Release(SoundArgs args)
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
