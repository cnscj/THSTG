using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public interface IObjectPool
    {
        T GetOrCreate<T>() where T : class, new();

        void Release<T>(T obj) where T : class, new();

        void Update();

        void Clear();

    }
}
