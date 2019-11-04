
using UnityEngine;
using XLibrary.Package;

namespace STGPublic
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public Camera mainCamera;
        public Camera mapCamera;
        public Camera uiCamera;


        private void Start()
        {
           
        }
        private CameraManager()
        {

        }
    }
}