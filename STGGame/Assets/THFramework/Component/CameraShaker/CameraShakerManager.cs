using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class CameraShakerManager : MonoBehaviour
    {
        private static CameraShakerManager s_instance;


        struct PairVec3
        {
            public Vector3 startPosition;
            public Vector3 startAngle;
        }
        public Transform[] cameras;

        //
        private CameraShakerCurve m_curve;
        private Dictionary<Transform, PairVec3> m_startMap = new Dictionary<Transform, PairVec3>();
        float m_currentTime = 999999.0f;
        Vector3 m_finalPosition;
        Vector3 m_finalAngle;

        public static CameraShakerManager GetInstance()
        {
            return s_instance;
        }

        public void Shake(CameraShakerCurve curCurve)
        {
            m_curve = curCurve;
            m_currentTime = 0f;
            m_finalPosition = Vector3.zero;
            m_finalAngle = Vector3.zero;
        }

        private void Start()
        {
            m_startMap.Clear();
            foreach (var iCamera in cameras)
            {
                PairVec3 startRecord = new PairVec3();
                startRecord.startPosition = iCamera.localPosition;
                startRecord.startAngle = iCamera.localEulerAngles;
                m_startMap.Add(iCamera, startRecord);
            }
        }

        void Update()
        {
            if (m_curve == null)
                return;

            if (m_startMap.Count <= 0)
                return;

            {
                if (m_currentTime >= m_curve.GetEndTime())
                {
                    m_curve = null;
                    return;
                }

                m_curve.Calculate(m_currentTime, ref m_finalPosition, ref m_finalAngle);
                m_currentTime += Time.deltaTime;

                foreach (var iCamera in m_startMap.Keys)
                {
                    Vector3 startPosition = m_startMap[iCamera].startPosition;
                    Vector3 startAngle = m_startMap[iCamera].startAngle;
                    iCamera.localPosition = startPosition + m_finalPosition;
                    iCamera.localEulerAngles = startAngle + m_finalAngle;
                }
            }
        }

        private void Awake()
        {
            s_instance = this;
        }
        private CameraShakerManager() { }
    }
}