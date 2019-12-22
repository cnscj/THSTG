using UnityEngine;
using System.Collections;

namespace THGame
{

    public class CameraView : MonoBehaviour
    {
        private Camera theCamera;

        //距离摄像机8.5米 用黄色表示
        public float upperDistance = 8.5f;
        //距离摄像机12米 用红色表示
        public float lowerDistance = 12.0f;
        //自动获取
        public bool isAutoDistance = true;


        void Start()
        {
            if (!theCamera)
            {
                theCamera = Camera.main;
            }
            if (isAutoDistance)
            {
                upperDistance = theCamera.farClipPlane;
                lowerDistance = theCamera.nearClipPlane;
            }
        }

        void Update()
        {
            FindCorners(upperDistance, Color.yellow);
            FindCorners(lowerDistance, Color.red);

        }
        void FindCorners(float distance,Color color)
        {
            Vector3[] corners = GetCorners(distance);

            // for debugging
            Debug.DrawLine(corners[0], corners[1], color); // UpperLeft -> UpperRight
            Debug.DrawLine(corners[1], corners[3], color); // UpperRight -> LowerRight
            Debug.DrawLine(corners[3], corners[2], color); // LowerRight -> LowerLeft
            Debug.DrawLine(corners[2], corners[0], color); // LowerLeft -> UpperLeft
        }

        Vector3[] GetCorners(float distance)
        {
            Transform tx = theCamera.transform;
            Vector3[] corners = new Vector3[4];

            float height = 0;
            bool orthographic = theCamera.orthographic;  //是否是正交相机
            if (orthographic)
            {
                height = theCamera.orthographicSize;
            }
            else
            {
                float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
                height = distance * Mathf.Tan(halfFOV);
            }

            float aspect = theCamera.aspect;
            float width = height * aspect;

            // UpperLeft
            corners[0] = tx.position - (tx.right * width);
            corners[0] += tx.up * height;
            corners[0] += tx.forward * distance;

            // UpperRight
            corners[1] = tx.position + (tx.right * width);
            corners[1] += tx.up * height;
            corners[1] += tx.forward * distance;

            // LowerLeft
            corners[2] = tx.position - (tx.right * width);
            corners[2] -= tx.up * height;
            corners[2] += tx.forward * distance;

            // LowerRight
            corners[3] = tx.position + (tx.right * width);
            corners[3] -= tx.up * height;
            corners[3] += tx.forward * distance;

            return corners;
        }
    }

}
