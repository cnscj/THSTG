using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class EffectDelayMono : MonoBehaviour
    {
        public float delayTime = 1.0f;


        void OnEnable()
        {
            gameObject.SetActive(false);
            Invoke("DelayFunc", delayTime);    
        }

        void DelayFunc()
        {
            gameObject.SetActive(true);
            
        }
    }

}