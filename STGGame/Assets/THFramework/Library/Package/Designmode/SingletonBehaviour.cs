
using System;
using UnityEngine;
namespace THGame.Package
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T GetInstance()
        {
            if (_applicationIsQuitting) 
            {
                return null;
            }

            lock(_lock)
            {
                if (_instance == null)
                {
                    _instance = (T) FindObjectOfType(typeof(T));
                    
                    if ( FindObjectsOfType(typeof(T)).Length > 1 )
                    {
                        return _instance;
                    }
                    
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) "+ typeof(T).ToString();
                        
                        DontDestroyOnLoad(singleton);
                    }
                }
                return _instance;
            }
        }

        void OnDestroy () 
        {
            _applicationIsQuitting = true;
        }
    }
}

