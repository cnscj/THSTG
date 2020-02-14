
using System;

namespace XLibGame
{
    public class RefObj<T> : BaseRef
    {
        private T m_obj;
        private Action m_releaseFunc;
        public RefObj(T obj, Action relFunc = null)
        {
            m_obj = obj;
            m_releaseFunc = relFunc;
        }

        public T GetObject()
        {
            return m_obj;
        }

        protected override void OnRelease()
        {
            m_releaseFunc?.Invoke();
            m_obj = default;
        }
    }

}
