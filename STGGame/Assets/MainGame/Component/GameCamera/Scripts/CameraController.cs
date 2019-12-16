
using UnityEngine;

namespace STGGame
{
    //摄像机K帧用
    public class CameraController : MonoBehaviour
    {
        public Vector3 mainPos;
        public Vector3 mainRot;

        public Vector3 mapPos;
        public Vector3 mapRot;


        private void Start()
        {
            mainPos = CameraManager.GetInstance().mainCamera.transform.localPosition;
            mainRot = CameraManager.GetInstance().mainCamera.transform.localEulerAngles;

            mapPos = CameraManager.GetInstance().mapCamera.transform.localPosition;
            mapRot = CameraManager.GetInstance().mapCamera.transform.localEulerAngles;
        }
        private void Update()
        {
            if (CameraManager.GetInstance().mainCamera)
            {
                CameraManager.GetInstance().mainCamera.transform.localPosition = mainPos;
                CameraManager.GetInstance().mainCamera.transform.localEulerAngles = mainRot;
            }
            if (CameraManager.GetInstance().mapCamera)
            {
                CameraManager.GetInstance().mapCamera.transform.localPosition = mapPos;
                CameraManager.GetInstance().mapCamera.transform.localEulerAngles = mapRot;
            }

        }
    }
}