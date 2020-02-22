﻿
namespace XLibGame
{
    public abstract class BaseRef : IRefCount
    {
        private int m_refCount;           //引用次数
        private int m_managedCount;       //自释放

        public BaseRef()
        {
            m_refCount = 1;
            m_managedCount = 0;
        }
        
        public void Retain()
        {
            ++m_refCount;
        }

        public void Release()
        {
            --m_refCount;
            //如果计数器减为0，释放本类实例化对象占用的内存  
            if (m_refCount == 0)
            {
                OnRelease();
            }
        }

        public int RefCount()
        {
            return m_refCount;
        }


        ///以下为管理器用
        public void ReleaseLater()
        {
            if (m_refCount > m_managedCount)
            {
                AutoReleasePoolManager.GetInstance().AddObject(this);

                //打开使用内存管理器的标记  
                ++m_managedCount;
            }
        }

        public void ManagedRelease()
        {
            --m_managedCount;
        }

        public int ManagedCount()
        {
            return m_managedCount;
        }

        //留给子类调用
        protected virtual void OnRelease()
        {

        }

    }

}