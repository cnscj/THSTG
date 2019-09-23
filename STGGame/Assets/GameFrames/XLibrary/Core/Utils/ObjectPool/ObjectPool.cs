using System.Collections;
using System;

namespace XLibrary.ObjectPool
{
    /// <summary>
    /// IObjectPool 的默认实现。
    /// 作者：朱伟 sky.zhuwei@163.com 
    /// </summary>
    #region ObjectPool
    public class ObjectPool : IObjectPool
    {
        #region members
        private Type destType = null;
        private object[] ctorArgs = null;
        private int minObjCount = 0;
        private int maxObjCount = 0;
        private int shrinkPoint = 0;
        private Hashtable hashTableObjs = new Hashtable();
        private Hashtable hashTableStatus = new Hashtable(); //key - isIdle        其中key就是hashcode
        private ArrayList keyList = new ArrayList();
        private bool supportReset = false;
        #endregion

        #region IObjectPool 成员
        public event CallBackObjPool PoolShrinked;
        public event CallBackObjPool MemoryUseOut;

        public bool Initialize(Type objType, object[] cArgs, int minNum, int maxNum)
        {
            if (minNum < 1)
            {
                minNum = 1;
            }
            if (maxNum < 5)
            {
                maxNum = 5;
            }

            this.destType = objType;
            this.ctorArgs = cArgs;
            this.minObjCount = minNum;
            this.maxObjCount = maxNum;
            double cof = 1 - ((double)minNum / (double)maxNum);
            this.shrinkPoint = (int)(cof * minNum);

            //缓存的类型是否支持IPooledObjSupporter接口
            Type supType = typeof(IPooledObjSupporter);
            if (supType.IsAssignableFrom(objType))
            {
                this.supportReset = true;
            }

            this.InstanceObjects();

            return true;
        }

        private void InstanceObjects()
        {
            for (int i = 0; i < this.minObjCount; i++)
            {
                this.CreateOneObject();
            }
        }

        #region CreateOneObject ,DistroyOneObject
        private int CreateOneObject()
        {
            object obj = null;

            try
            {
                obj = Activator.CreateInstance(this.destType, this.ctorArgs);
            }
            catch (Exception ee) //分配内存失败！
            {
                this.maxObjCount = this.CurObjCount;
                if (this.minObjCount > this.CurObjCount)
                {
                    this.minObjCount = this.CurObjCount;
                }

                if (this.MemoryUseOut != null)
                {
                    this.MemoryUseOut();
                }

                return -1;
            }

            int key = obj.GetHashCode();
            this.hashTableObjs.Add(key, obj);
            this.hashTableStatus.Add(key, true);
            this.keyList.Add(key);

            return key;
        }

        private void DistroyOneObject(int key)
        {
            object target = this.hashTableObjs[key];
            IDisposable tar = target as IDisposable;
            if (tar != null)
            {
                tar.Dispose();
            }

            this.hashTableObjs.Remove(key);
            this.hashTableStatus.Remove(key);
            this.keyList.Remove(key);
        }
        #endregion

        public object RentObject()
        {
            lock (this)
            {
                object target = null;
                foreach (int key in this.keyList)
                {
                    if ((bool)this.hashTableStatus[key]) //isIdle
                    {
                        this.hashTableStatus[key] = false;
                        target = this.hashTableObjs[key];
                        break;
                    }
                }

                if (target == null)
                {
                    if (this.keyList.Count < this.maxObjCount)
                    {
                        int key = this.CreateOneObject();
                        if (key != -1)
                        {
                            this.hashTableStatus[key] = false;
                            target = this.hashTableObjs[key];
                        }
                    }
                }

                return target;
            }

        }

        #region GiveBackObject
        public void GiveBackObject(int objHashCode)
        {
            if (this.hashTableStatus[objHashCode] == null)
            {
                return;
            }

            lock (this)
            {
                this.hashTableStatus[objHashCode] = true;
                if (this.supportReset)
                {
                    IPooledObjSupporter supporter = (IPooledObjSupporter)this.hashTableObjs[objHashCode];
                    supporter.Reset();
                }

                if (this.CanShrink())
                {
                    this.Shrink();
                }
            }
        }

        //能够收缩对象池
        private bool CanShrink()
        {
            int idleCount = this.GetIdleObjCount();
            int busyCount = this.CurObjCount - idleCount;

            return (busyCount < this.shrinkPoint) && (this.CurObjCount > (this.minObjCount + (this.maxObjCount - this.minObjCount) / 2));
        }

        private void Shrink()
        {
            while (this.CurObjCount > this.minObjCount)
            {
                int destKey = -1;
                foreach (int key in this.keyList)
                {
                    if ((bool)this.hashTableStatus[key])
                    {
                        destKey = key;
                        break;
                    }
                }

                if (destKey != -1)
                {
                    this.DistroyOneObject(destKey);
                }
                else
                {
                    break;
                }
            }

            if (this.PoolShrinked != null)
            {
                this.PoolShrinked();
            }
        }
        #endregion

        public void Dispose()
        {
            Type supType = typeof(System.IDisposable);
            if (supType.IsAssignableFrom(this.destType))
            {
                ArrayList list = (ArrayList)this.keyList.Clone();
                foreach (int key in list)
                {
                    this.DistroyOneObject(key);
                }
            }

            this.hashTableStatus.Clear();
            this.hashTableObjs.Clear();
            this.keyList.Clear();
        }

        #region property
        public int MinObjCount
        {
            get
            {
                return this.minObjCount;
            }
        }

        public int MaxObjCount
        {
            get
            {
                return this.maxObjCount;
            }
        }

        public int CurObjCount
        {
            get
            {
                return this.keyList.Count;
            }
        }

        public int IdleObjCount
        {
            get
            {
                lock (this)
                {
                    return this.GetIdleObjCount();
                }
            }
        }

        private int GetIdleObjCount()
        {
            int count = 0;
            foreach (int key in this.keyList)
            {
                if ((bool)this.hashTableStatus[key])
                {
                    ++count;
                }
            }

            return count;
        }
        #endregion

        #endregion
    }
    #endregion
}
