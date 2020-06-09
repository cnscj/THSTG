using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public Transform[] cameraTransform;
        public Camera[] cameras;

        void Awake()
        {
            if (cameras == null || cameras.Length <= 0)
            {
                cameras = new Camera[] { Camera.main };
            }
            if (cameraTransform == null || cameraTransform.Length <= 0)
            {
                cameraTransform = new Transform[] { Camera.main.transform };
            }
        }
    }

}
