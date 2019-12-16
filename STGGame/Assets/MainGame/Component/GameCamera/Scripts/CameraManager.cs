
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace STGGame
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        //不用Camera是因为可能有些摄像机是组绑定的
        public Transform mainCamera;
        public Transform entityCamera;
        public Transform mapCamera;
        public Transform uiCamera;

        private Dictionary<string, Transform> m_cameraMap;

        public Transform GetCamera(string key)
        {
            return m_cameraMap[key];
        }
        private void Awake()
        {
            var cameraMasks = GetComponentsInChildren<CameraMark>();
            foreach(var mark in cameraMasks)
            {
                m_cameraMap[mark.gameObject.name] = mark.gameObject.transform;
            }

        }
        private void Start()
        {
           
        }
        private CameraManager()
        {
            m_cameraMap = new Dictionary<string, Transform>();
        }
    }
}