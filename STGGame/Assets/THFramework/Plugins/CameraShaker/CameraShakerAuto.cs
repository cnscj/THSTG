using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class CameraShakerAuto : MonoBehaviour
    {
        struct PairVec3
        {
            public Vector3 startPosition;
            public Vector3 startAngle;
        }
        public AnimationCurve posX = AnimationCurve.Linear(0, 0, 1, 0);
        public AnimationCurve posY = AnimationCurve.Linear(0, 0, 1, 0);
        public AnimationCurve posZ = AnimationCurve.Linear(0, 0, 1, 0);

        public AnimationCurve angleX = AnimationCurve.Linear(0, 0, 1, 0);
        public AnimationCurve angleY = AnimationCurve.Linear(0, 0, 1, 0);
        public AnimationCurve angleZ = AnimationCurve.Linear(0, 0, 1, 0);

        //
        private Dictionary<Transform, PairVec3> m_startMap = new Dictionary<Transform, PairVec3>();
        float m_currentTime = 999999.0f;
        Vector3 m_finalPosition;
        Vector3 m_finalAngle;

        public void Shake(Transform[] relCamera)
        {
            m_startMap.Clear();
            foreach (var camera in relCamera)
            {
                PairVec3 startRecord = new PairVec3();
                startRecord.startPosition = camera.localPosition;
                startRecord.startAngle = camera.localEulerAngles;
                m_startMap.Add(camera, startRecord);
            }

            m_currentTime = 0f;
            m_finalPosition = Vector3.zero;
            m_finalAngle = Vector3.zero;
        }

        void Update()
        {
            if (m_startMap.Count > 0)
            {
                if (m_currentTime >= GetEndTime())
                {
                    return;
                }

                Calculate(m_currentTime);
                m_currentTime += Time.deltaTime;
                foreach(var camera in m_startMap.Keys)
                {
                    Vector3 startPosition = m_startMap[camera].startPosition;
                    Vector3 startAngle = m_startMap[camera].startAngle;
                    camera.localPosition = startPosition +  m_finalPosition;
                    camera.localEulerAngles = startAngle + m_finalAngle;
                }
            }
        }
        void Calculate(float curTime)
        {

            m_finalPosition.x = posX.Evaluate(curTime);
            m_finalPosition.y = posY.Evaluate(curTime);
            m_finalPosition.z = posZ.Evaluate(curTime);

            m_finalAngle.x = angleX.Evaluate(curTime);
            m_finalAngle.y = angleY.Evaluate(curTime);
            m_finalAngle.z = angleZ.Evaluate(curTime);
        }

        float GetEndTime()
        {
            float endTime = 0;

            if (posX.length > 0) endTime = Mathf.Max(endTime, posX.keys[posX.length - 1].time);
            if (posY.length > 0) endTime = Mathf.Max(endTime, posY.keys[posY.length - 1].time);
            if (posZ.length > 0) endTime = Mathf.Max(endTime, posZ.keys[posZ.length - 1].time);
            if (angleX.length > 0) endTime = Mathf.Max(endTime, angleX.keys[angleX.length - 1].time);
            if (angleY.length > 0) endTime = Mathf.Max(endTime, angleY.keys[angleY.length - 1].time);
            if (angleZ.length > 0) endTime = Mathf.Max(endTime, angleZ.keys[angleZ.length - 1].time);

            return endTime;
        }
    }
}
