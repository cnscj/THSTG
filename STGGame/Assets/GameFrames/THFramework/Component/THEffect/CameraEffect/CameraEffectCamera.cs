using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class CameraEffectCamera : MonoBehaviour
    {
        public struct CameraInitInfo
        {
            public Vector3 startPosition;
            public Vector3 startEulerAngles;
        }
        private static CameraEffectCamera s_instance;
        public Transform[] cameras;
        public Dictionary<Transform, CameraInitInfo> cameraMap = new Dictionary<Transform, CameraInitInfo>();
        
        public static CameraEffectCamera GetInstance()
        {
            return s_instance;
        }

        private void Awake()
        {
            s_instance = this;
        }

        void OnEnable()
        {
            s_instance = this;
        }

        private void Start()
        {
            InitCamera();
        }

        void InitCamera()
        {
            cameraMap.Clear();
            foreach (var iCamera in cameras)
            {
                CameraInitInfo startRecord = new CameraInitInfo();
                startRecord.startPosition = iCamera.localPosition;
                startRecord.startEulerAngles = iCamera.localEulerAngles;
                cameraMap.Add(iCamera, startRecord);
            }
        }
    }

}
