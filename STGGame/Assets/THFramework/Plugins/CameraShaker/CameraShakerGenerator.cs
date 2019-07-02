using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace THGame
{
    public class CameraShakerGenerator : MonoBehaviour
    {
        //struct 没有引用,赋值时为值传递,可能造成无法改值
        [System.Serializable]
        public class KeyExInfo
        {
            [SerializeField] public bool isShow;
            [SerializeField] public float keyTime;
            [SerializeField] public Vector3 shakePosition;
            [SerializeField] public Vector3 shakeAngle;
            [SerializeField] public float cycleTime;
            [SerializeField] public int cycleCount;
            [SerializeField] public bool fixShake;
        }

        class PairVec3
        {
            public Vector3 startPosition;
            public Vector3 startAngle;
        }
        //是高级还是低级的Index
        public int selectIndex = 0;

        //动画曲线
        public CameraShakerCurve animcurve = new CameraShakerCurve();
        public bool isAutoPlay = true;

        //简单的
        public AnimationCurve[] eachTimeLines = { AnimationCurve.Linear(0, 0, 1, 0), AnimationCurve.Linear(0, 0, 1, 0), AnimationCurve.Linear(0, 0, 1, 0) };
        public float cycTime = 0.12f;
        public int cycCount = 6;
        public bool isFixTime = false;

        //高级
        public AnimationCurve timeLine = AnimationCurve.Linear(0, 0, 1, 0);
        [SerializeField]public List<KeyExInfo> keyExInfos = new List<KeyExInfo>();
        public bool isShowAllFrames;

        private void Start()
        {
            if (isAutoPlay)
            {
                if (CameraShakerManager.GetInstance())
                {
                    CameraShakerManager.GetInstance().Shake(animcurve);
                }
            }
        }
    }
}
