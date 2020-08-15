using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraTimeScale : BaseCameraEffecter
    {
        [Header("时间缩放系数(0~1）")]
        [Range(0,1)]public float timeScale = 1f;

        [HideInInspector]public bool isScaleable = true;

        private void Update()
        {
            if (!isScaleable) return;

            Time.timeScale = timeScale;
        }

        public void Recover()
        {
            timeScale = 1f;
            Time.timeScale = timeScale;
        }

    }
}