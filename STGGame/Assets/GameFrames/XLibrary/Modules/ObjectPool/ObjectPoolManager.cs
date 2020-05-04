using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
    {
        private Dictionary<Type, BaseObjectPool> m_objPool = new Dictionary<Type, BaseObjectPool>();
        public BaseObjectPool Create<T>() where T : BaseObjectPool
        {
            //TODO:
            return null;
        }
        private void Update()
        {
            foreach(var pool in m_objPool.Values)
            {
                pool.Update();
            }
        }
    }
}
