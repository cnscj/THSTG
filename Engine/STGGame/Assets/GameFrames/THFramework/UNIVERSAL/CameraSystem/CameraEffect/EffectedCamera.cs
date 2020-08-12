using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class EffectedCamera : MonoSingleton<EffectedCamera>
    {
        public class VectorMatrix
        {
            public Vector3 localPosition;
            public Vector3 localEulerAngles;
            public Vector3 localScale;

            public VectorMatrix()
            {

            }
            public VectorMatrix(Transform transform)
            {
                Save(transform);
            }

            public void Save(Transform transform)
            {
                this.localPosition = transform.localPosition;
                this.localEulerAngles = transform.localEulerAngles;
                this.localScale = transform.localScale;
            }

            public void Load(Transform transform)
            {
                transform.localPosition = this.localPosition;
                transform.localEulerAngles = this.localEulerAngles;
                transform.localScale = this.localScale;
            }

            public void Clear()
            {
                this.localPosition = Vector3.zero;
                this.localEulerAngles = Vector3.zero;
                this.localScale = Vector3.one;
            }
        }
        public Transform[] cameraTransforms;
        public Camera[] cameras;

        private Dictionary<Transform, VectorMatrix> m_materixBackup;

        //保存矩阵
        public Dictionary<Transform, VectorMatrix> SaveMatrixs()
        {
            m_materixBackup = m_materixBackup ?? new Dictionary<Transform, VectorMatrix>();
            foreach (var transform in cameraTransforms)
            {
                VectorMatrix matrix = null;
                if (!m_materixBackup.TryGetValue(transform,out matrix))
                {
                    matrix = new VectorMatrix();
                    m_materixBackup[transform] = matrix;
                }
                matrix.Save(transform);
            }

            return m_materixBackup;
        }

        public void TranslateMatrixs(Vector3 position, Vector3 eulerAngle, Vector3 scale)
        {
            if (m_materixBackup != null)
            {
                foreach (var transform in cameraTransforms)
                {
                    if (m_materixBackup.TryGetValue(transform, out var matrix))
                    {
                        transform.localPosition = matrix.localPosition + position;
                        transform.localEulerAngles = matrix.localEulerAngles + eulerAngle;
                        transform.localScale = matrix.localScale + scale;
                    }
                }
            }
        }

        //还原矩阵
        public void BackMatrixs()
        {
            if (m_materixBackup != null)
            {
                foreach(var transform in cameraTransforms)
                {
                    if (m_materixBackup.TryGetValue(transform, out var matrix))
                    {
                        matrix.Load(transform);
                    }
                }
                m_materixBackup.Clear();
            }
        }

    }
}
