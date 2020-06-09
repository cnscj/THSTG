using UnityEngine;
using System.Collections;

namespace THGame
{
    [ExecuteInEditMode]
    public class CameraBloom : BaseCameraEffecter
    {
        public static CameraBloom Instance { get; protected set; }

        private void Awake()
        {
            Instance = this;
        }

      
    }
}