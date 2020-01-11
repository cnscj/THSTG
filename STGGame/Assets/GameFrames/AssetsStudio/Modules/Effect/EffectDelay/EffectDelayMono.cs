using UnityEngine;

namespace ASGame
{
    public class EffectDelayMono : MonoBehaviour
    {
        public float delayTime = 1.0f;
        public bool isEnabling = false;

        void OnEnable()
        {
            if (!isEnabling && delayTime > 0f)
            {
                gameObject.SetActive(false);
                Invoke("DelayFunc", delayTime);
                isEnabling = true;
            }
        }

        void DelayFunc()
        {
            gameObject.SetActive(true);
            if (isEnabling)
            {
                CancelInvoke("DelayFunc");
                isEnabling = false;
            }
        }
    }

}