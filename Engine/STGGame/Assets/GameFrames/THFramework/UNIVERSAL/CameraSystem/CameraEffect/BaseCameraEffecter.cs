using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class BaseCameraEffecter : MonoBehaviour
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

