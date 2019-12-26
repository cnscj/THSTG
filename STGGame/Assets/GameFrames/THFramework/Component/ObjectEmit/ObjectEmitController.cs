
using System;
using UnityEngine;

namespace THGame
{
    //决定发射后,单个物体一系列行为,如停留,追踪等
    public class ObjectEmitController : MonoBehaviour
    {
        public Func<Vector3> moveAccelerated;
        public Func<Vector3> rotationAccelerated;

        public Vector3 moveSpeed = Vector3.zero;
        public Vector3 rotationSpeed = Vector3.zero;

        void Update()
        {
            if (moveAccelerated != null) moveSpeed = moveAccelerated();
            if (rotationAccelerated != null) rotationSpeed = moveAccelerated();

            transform.position += moveSpeed * Time.deltaTime;
            transform.eulerAngles += rotationSpeed * Time.deltaTime;
        }
    }
}

