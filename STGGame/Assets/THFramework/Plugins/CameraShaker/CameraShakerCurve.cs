using UnityEngine;

namespace THGame
{
    [System.Serializable]
    public class CameraShakerCurve
    {
        [System.Serializable]
        public class CameraShakerCurveVector3
        {
            public AnimationCurve x = AnimationCurve.Linear(0, 0, 0, 0);
            public AnimationCurve y = AnimationCurve.Linear(0, 0, 0, 0);
            public AnimationCurve z = AnimationCurve.Linear(0, 0, 0, 0);
        }
        public CameraShakerCurveVector3 position = new CameraShakerCurveVector3();
        public CameraShakerCurveVector3 rotation = new CameraShakerCurveVector3();

        public void Calculate(float curTime, ref Vector3 vPos, ref Vector3 vRot)
        {
            vPos.x = position.x.Evaluate(curTime);
            vPos.y = position.y.Evaluate(curTime);
            vPos.z = position.z.Evaluate(curTime);

            vRot.x = rotation.x.Evaluate(curTime);
            vRot.y = rotation.y.Evaluate(curTime);
            vRot.z = rotation.z.Evaluate(curTime);
        }

        public float GetEndTime()
        {
            float endTime = 0;

            if (position.x.length > 0) endTime = Mathf.Max(endTime, position.x.keys[position.x.length - 1].time);
            if (position.y.length > 0) endTime = Mathf.Max(endTime, position.y.keys[position.y.length - 1].time);
            if (position.z.length > 0) endTime = Mathf.Max(endTime, position.z.keys[position.z.length - 1].time);
            if (rotation.x.length > 0) endTime = Mathf.Max(endTime, rotation.x.keys[rotation.x.length - 1].time);
            if (rotation.y.length > 0) endTime = Mathf.Max(endTime, rotation.y.keys[rotation.y.length - 1].time);
            if (rotation.z.length > 0) endTime = Mathf.Max(endTime, rotation.z.keys[rotation.z.length - 1].time);

            return endTime;
        }

        public void SetCurves(AnimationCurve[] curves)
        {
            position.x = curves[0];
            position.y = curves[1];
            position.z = curves[2];
            rotation.x = curves[3];
            rotation.y = curves[4];
            rotation.z = curves[5];
        }

        public AnimationCurve[] GetCurves()
        {
            return new AnimationCurve[]{ position.x, position.y, position.z, rotation.x, rotation.y, rotation.z };
        }

        public AnimationCurve GetCurve(int index)
        {
            switch (index)
            {
                case 0: return position.x;
                case 1: return position.y;
                case 2: return position.z;
                case 3: return rotation.x;
                case 4: return rotation.x;
                case 5: return rotation.x;
            }
            return null;
        }
    }
}

