using System;

namespace THGame
{
    public interface IPooledObjSupporter : IDisposable
    {
        void Reset(); //恢复对象为初始状态，当IObjectPool.GiveBackObject时调用
    }
}