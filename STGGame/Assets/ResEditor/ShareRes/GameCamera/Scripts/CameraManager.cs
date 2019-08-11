
using UnityEngine;
using Unity.Entities;
using THGame;
using THGame.Package;

namespace STGGame
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public Camera mainCamera;
        public Camera uiCamera;


        private void Start()
        {
           
        }
        private CameraManager()
        {

        }
    }
}