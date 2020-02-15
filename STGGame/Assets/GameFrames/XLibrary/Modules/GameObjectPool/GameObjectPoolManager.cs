using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    public class GameObjectPoolManager : BaseGameObjectPoolManager
    {
        private static GameObjectPoolManager _instance;
        private static object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static GameObjectPoolManager GetInstance()
        {
            if (_applicationIsQuitting)
            {
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (GameObjectPoolManager)FindObjectOfType(typeof(GameObjectPoolManager));

                    if (FindObjectsOfType(typeof(GameObjectPoolManager)).Length > 1)
                    {
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<GameObjectPoolManager>();
                        singleton.name = "(singleton) " + typeof(GameObjectPoolManager).ToString();

                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }

        void OnDestroy()
        {
            _applicationIsQuitting = true;
        }
    }
}
