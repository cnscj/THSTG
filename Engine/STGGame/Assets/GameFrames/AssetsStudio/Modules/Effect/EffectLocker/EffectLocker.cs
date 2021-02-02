using UnityEngine;

namespace ASGame
{
    public class EffectLocker : MonoBehaviour
    {
        [Header("锁旋转")] public bool lockRotation = true;
        [Header("锁位置")] public bool lockPosition = false;
        [Header("锁大小")] public bool lockScale = false;

        [HideInInspector] public bool isHadStartRecord = false;
        [HideInInspector] public bool isEnabledLockRecord = false;
        [HideInInspector] public bool isStartLockRecord = true;

        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _startRotation = Vector3.zero;

        public void OnEnable()
        {
            if (isEnabledLockRecord)
            {
                Record();
            }
        }

        //上下坐骑如果lockPosition会有问题
        //复用的时候如果之前发生过旋转也会有问题
        public void Start()
        {
            if (isHadStartRecord) return;
            if (isStartLockRecord)
            {
                Record();
                isHadStartRecord = true;
            }
        }

        public void Record()
        {
            _startPosition = transform.position;
            _startRotation = transform.eulerAngles;
        }

        void Update()
        {
            if (lockPosition)
            {
                transform.position = _startPosition;
            }

            if (lockRotation)
            {
                transform.eulerAngles = _startRotation;
            }

            if (lockScale)
            {
                var parentTrans = transform.parent;
                if (parentTrans != null)
                {
                    transform.localScale = new Vector3(1 / parentTrans.localScale.x, 1 / parentTrans.localScale.y, 1 / parentTrans.localScale.z);
                }

            }
        }
    }

}
