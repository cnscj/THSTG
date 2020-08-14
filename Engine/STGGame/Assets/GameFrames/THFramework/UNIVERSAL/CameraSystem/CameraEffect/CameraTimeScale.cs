using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraTimeScale : BaseCameraEffecter<CameraTimeScale>
    {
        [Header("时间缩放系数(0~1）")]
        public float timeScale = 1f;
        private float m_lastTimeScale = 1f;

        private void Start()
        {
            m_lastTimeScale = timeScale;
        }

        private void Update()
        {
            if (!Mathf.Approximately(m_lastTimeScale, timeScale))
            {
                Time.timeScale = timeScale;
                m_lastTimeScale = timeScale;
            }
        }

        public void Recover()
        {
            timeScale = 1f;
        }

    }
}