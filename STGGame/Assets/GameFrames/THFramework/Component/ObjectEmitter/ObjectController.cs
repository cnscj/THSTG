using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    //TODO:决定发射后,单个物体一系列行为,如停留,追踪等
    public class ObjectController : MonoBehaviour
    {
        public Vector3 speed = new Vector3(0,0,0);

        void Start()
        {

        }

        void Update()
        {
            transform.localPosition += speed;
        }
    }
}

