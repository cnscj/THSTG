using UnityEngine;

namespace ASGame
{
    public class EffectLocker : MonoBehaviour
    {
        [Header("锁位置")] public bool lockPosition = true;
        [Header("锁旋转")] public bool lockRotation = true;
        [Header("锁大小")] public bool lockScale = false;

        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _startRotation = Vector3.zero;

        public void OnEnable()
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
                    transform.localScale = new Vector3(1/parentTrans.localScale.x, 1 / parentTrans.localScale.y, 1 / parentTrans.localScale.z);
                }
                
            }
        }
    }

}
