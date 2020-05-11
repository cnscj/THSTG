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
        public Action onCompleted;
        private WaitForSeconds m_waitForSeconds;

        private void OnEnable()
        {
            if (enabled)
            {
                Call();
            }
        }

        private void Call()
        {
            if (length >= 0f)
            {
                m_waitForSeconds = m_waitForSeconds ?? new WaitForSeconds(length);
                StartCoroutine(CountDown());
            }
        }

        private IEnumerator CountDown()
        {
            yield return m_waitForSeconds;
            if (gameObject != null)
            {
                onCompleted?.Invoke();
            }
        }

#if UNITY_EDITOR
        public void Calculate()
        {
            length = EffectLengthTools.CalculatePlayEndTime(gameObject);
        }
#endif
    }
}
