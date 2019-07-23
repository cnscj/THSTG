using System.Collections;
using System.Collections.Generic;
using THGame.Package;
using UnityEngine;

namespace THGame
{
    public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
    {
        public static readonly string poolname = "ObjectPoolRoot";
        public static GameObject objectPoolRoot;
        private static GameObjectPool m_gamePool = new GameObjectPool();

        private void Awake()
        {
            objectPoolRoot = new GameObject(poolname);


        }
    }
}

