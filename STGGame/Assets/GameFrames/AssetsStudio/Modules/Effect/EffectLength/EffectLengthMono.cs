using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASGame
{
    public class EffectLengthMono : MonoBehaviour
    {

        public float length = -2; //-2未知,-1无限,0~Max长度
        public Action<float> callback;

        public void Call()
        {
            if (length >= 0)
            {
                if (callback != null)
                {
                    Invoke("TryCallback", length);
                }
            }
        }

        void Start()
        {
            Call();
        }

        void TryCallback()
        {
            if (callback != null)
            {
                callback(length);
            }
        }

#if UNITY_EDITOR

#endif
    }
}
