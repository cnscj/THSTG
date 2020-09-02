using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class RockCamera : MonoBehaviour
    {
        private static RockCamera s_instance;
        [HideInInspector] public Vector4 rockArgs = Vector4.zero;

        private Vector3 m_rockPosition = Vector3.zero;
        private Vector3 m_rockRotation = Vector3.zero;

        private VectorMatrix m_startMatrix = new VectorMatrix();

        public static RockCamera GetInstance()
        {
            return s_instance;
        }

        public void Rock(Vector4 args)
        {
            enabled = true;
            rockArgs = args;
        }

        public void Recover()
        {
            m_startMatrix.Load(transform);
        }

        private void Awake()
        {
            s_instance = this;
        }

        void OnEnable()
        {
            m_startMatrix.Save(transform);
        }

        void OnDisable()
        {
            m_startMatrix.Load(transform);
        }

        private void LateUpdate()
        {
            m_rockPosition.x = rockArgs.x;
            m_rockPosition.y = rockArgs.y;
            m_rockPosition.z = rockArgs.z;
            m_rockRotation.z = rockArgs.w;

            transform.localPosition = m_startMatrix.localPosition + m_rockPosition;
            transform.localEulerAngles = m_startMatrix.localEulerAngles + m_rockRotation;
        }

    }

}

