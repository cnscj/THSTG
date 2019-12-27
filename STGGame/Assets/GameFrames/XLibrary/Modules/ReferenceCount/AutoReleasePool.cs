using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XLibGame
{
    public class AutoReleasePool
    {
        public List<Reference> m_objectArray;
        public AutoReleasePool()
        {
            m_objectArray = new List<Reference>();
        }

        ~AutoReleasePool()
        {
            Clear();
        }
        // 将对象添加到自动释放池
        public void AddObject(Reference refObj)
        {
            m_objectArray.Add(refObj);
        }

        // 将对象从自动释放池中移除  
        public void RemoveObject(Reference refObj)
        {
            m_objectArray.Remove(refObj);
        }
        //
        public void Purge()
        {
            for (int i = m_objectArray.Count - 1; i >= 0; i--)
            {
                var iter = m_objectArray[i];
                if (iter.RefCount() >= iter.ManagedCount())
                {
                    iter.ManagedRelease();
                    iter.Release();
                    m_objectArray.Remove(iter);
                }
                else
                {
                    iter.ManagedRelease();
                    m_objectArray.Remove(iter);
                }
            }
        }

        // 将自动释放池中的对象释放掉
        public void Clear()
        {
            Purge();
            m_objectArray.Clear();
        }
    }

}
