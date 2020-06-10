using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class EffectedCamera : MonoSingleton<EffectedCamera>
    {
        protected class CameraMatrix
        {
            public Vector3 localPosition;
            public Vector3 localEulerAngles;

        }
        public Transform[] cameraTransform;
        public Camera[] cameras;

        private Dictionary<Transform, CameraMatrix> m_materixBackup;

        //保存矩阵
        public void SaveMatrixs()
        {
            m_materixBackup = m_materixBackup ?? new Dictionary<Transform, CameraMatrix>();
            foreach (var transform in cameraTransform)
            {
                CameraMatrix matrix = new CameraMatrix();
                matrix.localPosition = transform.localPosition;
                matrix.localEulerAngles = transform.localEulerAngles;

                m_materixBackup.Add(transform, matrix);
            }
        }

        //还原矩阵
        public void BackMatrixs()
        {
            if (m_materixBackup != null)
            {
                foreach(var transform in cameraTransform)
                {
                    if (m_materixBackup.TryGetValue(transform, out var matrix))
                    {
                        transform.localPosition = matrix.localPosition;
                        transform.localEulerAngles = matrix.localEulerAngles;
                    }
                }
                m_materixBackup.Clear();
            }
        }

    }
}
