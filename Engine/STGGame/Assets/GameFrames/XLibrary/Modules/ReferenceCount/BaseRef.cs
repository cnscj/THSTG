
namespace XLibGame
{
    public abstract class BaseRef : IRefCount
    {
        public int ManagedCount { get; private set; }
        public int RefCount { get; private set; }

        public BaseRef()
        {
            RefCount = 1;
            ManagedCount = 0;
        }
        
        public void Retain()
        {
            ++RefCount;
            OnRetain();
        }

        public void Release()
        {
            --RefCount;
            //如果计数器减为0，释放本类实例化对象占用的内存  
            if (RefCount == 0)    //不小于0是为了防止多次调用
            {
                OnRelease();
            }
            else if (RefCount < 0)
            {
                RefCount = 0;
            }
            
        }

        ///以下为管理器用
        public void ReleaseLater()
        {
            if (RefCount > ManagedCount)
            {
                AutoReleasePoolManager.GetInstance().AddObject(this);

                //打开使用内存管理器的标记  
                ++ManagedCount;
            }
        }

        public void ManagedRelease()
        {
            --ManagedCount;
        }

        /////////////////////
        protected virtual void OnRetain() {}
        protected virtual void OnRelease() {}
    }

}
