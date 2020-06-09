using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class EffectedCamera : MonoSingleton<EffectedCamera>
    {
        [System.Serializable]
        public class CameraGroup
        {
            public string key;
            public Transform[] cameraTransform;
            public Camera[] cameras;
        }
        public static readonly string DEFAULT_KEY = "default";

        public CameraGroup[] cameraGroups;
        private Dictionary<string, CameraGroup> m_cameraGroups = new Dictionary<string, CameraGroup>();

        void Awake()
        {
            if (cameraGroups == null || cameraGroups.Length <= 0)
            {

            }
        }

    }
}
