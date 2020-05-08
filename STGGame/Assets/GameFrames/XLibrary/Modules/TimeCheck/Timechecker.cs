using UnityEngine;

namespace XLibGame
{
    public class Timechecker
    {
        public float stayTime;
        private float m_updateTick;
        public Timechecker()
        {
            UpdateTick();
        }

        public bool CheckTick()
        {
            if (stayTime > 0)
            {
                if (m_updateTick + stayTime <= Time.realtimeSinceStartup)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateTick()
        {
            m_updateTick = Time.realtimeSinceStartup;
        }
    }

}
