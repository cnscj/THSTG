using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class BaseCameraEffecter<T> : MonoSingleton<T> where T : MonoBehaviour
    {
        protected EffectedCamera GetEffectedCamera()
        {
            return EffectedCamera.GetInstance();
        }

        protected Camera[] GetCameras()
        {
            return GetEffectedCamera().cameras;
        }

        protected Transform[] GetTransforms()
        {
            return GetEffectedCamera().cameraTransforms;
        }
    }
}

