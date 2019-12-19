
using UnityEngine;

namespace THGame
{
    //决定发射后,单个物体一系列行为,如停留,追踪等
    public class ObjectEmitController : MonoBehaviour
    {

        public Vector3 moveSpeed = Vector3.zero;
        public Vector3 rotationSpeed = Vector3.zero;

        void Update()
        {
            transform.localPosition += moveSpeed * Time.deltaTime;
            transform.localEulerAngles += rotationSpeed * Time.deltaTime;
        }
    }
}

