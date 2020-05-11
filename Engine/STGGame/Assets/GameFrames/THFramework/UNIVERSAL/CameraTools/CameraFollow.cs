using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform player;            // 跟随对象的初始位置

        public float distance = 0;          // 距离
        public float scrollSpeed = 3;       // 拉进/拉远视角的速度
        public float rotateSpeed = 2;       // 旋转速度

        private Vector3 offsetPosition;     // 相机与对象的位置偏移
        private bool isRotating = false;    // 是否正在旋转

        void Start()
        {
            if (player == null)
                return;

            transform.LookAt(player.position);
            offsetPosition = transform.position - player.position;
        }

        void Update()
        {
            if (player == null)
                return;

            transform.position = offsetPosition + player.position;
            RotateView(); // 处理视野的旋转
            ScrollView(); // 处理视野的拉近和拉远效果   
        }

        void ScrollView()
        {
            if (player == null)
                return;

            // 向后滑动返回负值（拉远视野） 向前滑动返回正值（拉近视野）
            distance = offsetPosition.magnitude;
            distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
            distance = Mathf.Clamp(distance, 5, 10);
            offsetPosition = offsetPosition.normalized * distance;
        }

        void RotateView()
        {
            if (player == null)
                return;

            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            {
                isRotating = true;
            }
            else
            {
                isRotating = false;
            }

            if (isRotating && Input.GetKey(KeyCode.Q))
            {
                transform.RotateAround(player.position, Vector3.up, -rotateSpeed);
            }
            else if (isRotating && Input.GetKey(KeyCode.E))
            {
                transform.RotateAround(player.position, Vector3.up, rotateSpeed);
            }
            offsetPosition = transform.position - player.position;
        }
    }

}

