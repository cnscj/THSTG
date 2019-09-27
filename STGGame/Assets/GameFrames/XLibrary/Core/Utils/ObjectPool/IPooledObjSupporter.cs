﻿using System;

namespace XLibrary.ObjectPool
{
    public interface IPooledObjSupporter : IDisposable
    {
        void Reset(); //恢复对象为初始状态，当IObjectPool.GiveBackObject时调用
    }
}