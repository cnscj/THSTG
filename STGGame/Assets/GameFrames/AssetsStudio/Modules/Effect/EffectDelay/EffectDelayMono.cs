using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class EffectDelayMono : MonoBehaviour
    {
        public float delayTime = 0.0f;

        void Awake()
        {
            gameObject.SetActive(false);
            Invoke("DelayInvokeDo", delayTime);
        }

        void DelayInvokeDo()
        {
            gameObject.SetActive(true);
        }
    }

}