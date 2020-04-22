
using System;

namespace XLibGame
{
    public class RefObj<T> : BaseRef
    {
        private T m_obj;
        private Action m_retainFunc;
        private Action m_releaseFunc;
        public RefObj(T obj, Action retFunc = null, Action relFunc = null)
        {
            m_obj = obj;
            m_retainFunc = retFunc;
            m_releaseFunc = relFunc;
        }

        public T GetObject()
        {
            return m_obj;
        }

        protected override void OnRetain()
        {
            m_retainFunc?.Invoke();
        }

        protected override void OnRelease()
        {
            m_releaseFunc?.Invoke();
            m_obj = default;
        }
    }

}
