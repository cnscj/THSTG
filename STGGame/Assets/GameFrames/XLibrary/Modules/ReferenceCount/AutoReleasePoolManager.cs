using UnityEngine;
using System.Collections;
using XLibrary.Package;
using System.Collections.Generic;

namespace XLibGame
{
    public class AutoReleasePoolManager : MonoSingleton<AutoReleasePoolManager>
    {
        
        private List<AutoReleasePool> m_autoReleasePoolStack;//用于存放自动释放池的队列
        private AutoReleasePool m_curReleasePool;//指向自动释放池队列的末尾节点

        public AutoReleasePoolManager()
        {
            m_autoReleasePoolStack = new List<AutoReleasePool>();
            m_curReleasePool = null;
        }
        ~AutoReleasePoolManager()
        {
            Clear();
        }
        //添加对象
        public void AddObject(Reference refObj)
        {
            GetCurReleasePool().AddObject(refObj);
        }

        //移除对象
        public void RemoveObject(Reference refObj)
        {
            m_curReleasePool.RemoveObject(refObj);
        }

        // 清空所有的自动释放池  
        public void Clear()
        {
            m_autoReleasePoolStack.Clear();
        }

        // 增加一个自动释放池  
        private void Push()
        {
            AutoReleasePool pool = new AutoReleasePool();
            m_curReleasePool = pool;

            m_autoReleasePoolStack.Add(pool);
        }

        // 移除一个自动释放池，即移除当前自动释放池  
        private void Pop()
        {
            // 清理自动释放池队列，只剩下队列中的第一个自动释放池  
            // 剩下的这个自动释放池中的对象也要清理掉  
            // 这个函数便是自动释放池管理者，实现自动释放池内对象的实现了  
            if (m_curReleasePool == null)
            {
                return;
            }

            // 清理当前的自动释放池  
            m_curReleasePool.Clear();

            // 保持池中只有一个对象池
            int nCount = m_autoReleasePoolStack.Count;
            if (nCount > 1)
            {
                // 如果自动释放池队列中有超过一个自动释放池  
                // 将末端的自动释放池清理并移除
                m_autoReleasePoolStack.RemoveAt(m_autoReleasePoolStack.Count - 1);
                m_curReleasePool = m_autoReleasePoolStack[m_autoReleasePoolStack.Count - 1];
            }
        }
        private AutoReleasePool GetCurReleasePool()
        {
            if (m_curReleasePool == null)
            {
                Push();
            }

            return m_curReleasePool;
        }

        private void Update()
        {
            Pop();
        }
    }
}

