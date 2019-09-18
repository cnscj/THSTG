using UnityEngine;
using System.Collections;
using System.IO;

namespace ASGame
{
    public class EffectManager
    {
        private const string s_effectPath = "effects";
        public delegate void LevelChangedDelegate(int level);
        public LevelChangedDelegate levelChangedCallback;
        private static EffectManager s_instance;
        private int m_limitLevel = 10;

        public static EffectManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new EffectManager();
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

