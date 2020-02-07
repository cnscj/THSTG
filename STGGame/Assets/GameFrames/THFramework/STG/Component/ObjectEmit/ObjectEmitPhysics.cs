
using System;
using UnityEngine;

namespace THGame
{
    //决定发射后,单个物体一系列行为,如停留,追踪等
    public class ObjectEmitPhysics : MonoBehaviour
    {
        public Func<Vector3> moveSpeedFunc;
        public Func<Vector3> rotationSpeedFunc;

        public Vector3 moveSpeed = Vector3.zero;
        public Vector3 rotationSpeed = Vector3.zero;

        void Update()
        {
            if (moveSpeedFunc != null) moveSpeed = moveSpeedFunc();
            if (rotationSpeedFunc != null) rotationSpeed = rotationSpeedFunc();

            transform.position += moveSpeed * Time.deltaTime;
            transform.eulerAngles += rotationSpeed * Time.deltaTime;
        }
    }
}

