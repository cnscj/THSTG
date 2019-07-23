using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace THGame
{
    public interface IPoolObject
    {
        void Init();

        void Release();
    }
}