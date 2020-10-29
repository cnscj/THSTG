
using System;

namespace THGame
{
    [System.Serializable]
    public class SkillInfo
    {
        public float cdTime;
        public int invalidFrame;
        public bool canBeInterrupted;
        public string skillExStr;
        public Tuple<string, int>[] preconditions;
    }

}
