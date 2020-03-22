using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;

namespace ASGame
{
    //缓冲数据信息
    public abstract class BaseAssetRef : IAssetRef
    {
        private int m_refCount;                 //引用次数

        public BaseAssetRef()
        {
            m_refCount = 1;
        }

        public int RefCount()
        {
            return m_refCount;
        }

        public void Retain()
        {
            ++m_refCount;
        }

        public void Release()
        {
            --m_refCount; 
            if (m_refCount == 0)
            {
                OnRelease();
            }
        }

        public void ReleaseDelay()
        {
            AssetRefManager.GetInstance().DelayRelease(this);
        }

        protected virtual void OnRelease()
        {

        }
    }

}
