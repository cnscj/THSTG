using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class EffectedCamera : MonoSingleton<EffectedCamera>
    {
        public Transform[] cameraTransforms;
        public Camera[] cameras;

        private Dictionary<Transform, VectorMatrix> m_materixBackup;

        //保存矩阵
        public Dictionary<Transform, VectorMatrix> SaveMatrixs(Dictionary<Transform, VectorMatrix>  backupMaterix)
        {
            backupMaterix = backupMaterix ?? new Dictionary<Transform, VectorMatrix>();
            foreach (var transform in cameraTransforms)
            {
                VectorMatrix matrix = null;
                if (!backupMaterix.TryGetValue(transform,out matrix))
                {
                    matrix = new VectorMatrix();
                    backupMaterix[transform] = matrix;
                }
                matrix.Save(transform);
            }
            return backupMaterix;
        }
        public Dictionary<Transform, VectorMatrix> SaveMatrixs()
        {
            m_materixBackup = SaveMatrixs(m_materixBackup);
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

        public void LoadMaterixs(Dictionary<Transform, VectorMatrix> materixs)
        {
            m_materixBackup = materixs;
            if (m_materixBackup != null)
            {
                foreach (var transform in cameraTransforms)
                {
                    if (m_materixBackup.TryGetValue(transform, out var matrix))
                    {
                        matrix.Load(transform);
                    }
                }
                m_materixBackup.Clear();
            }
        }

        //回滚矩阵
        public void BackMatrixs()
        {
            LoadMaterixs(m_materixBackup);
        }

    }
}
