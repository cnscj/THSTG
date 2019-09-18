using UnityEngine;
namespace ASGame
{
    public class ModelEffectManager
    {
        public delegate void LevelChangedDelegate(int level);
        public LevelChangedDelegate levelChangedCallback;
        private static ModelEffectManager s_instance;
        private int m_limitLevel = 10;

        public static ModelEffectManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new ModelEffectManager();
                }
                return s_instance;
            }
        }

        public int limitLevel
        {
            get
            {
                return m_limitLevel;
            }
            set
            {
                if (m_limitLevel != value)
                {
                    m_limitLevel = value;
                    if (levelChangedCallback != null)
                    {
                        levelChangedCallback.Invoke(m_limitLevel);
                    }
                }
            }
        }

      
    }
}