
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

        public bool canInterruptPrepare; //可被中断前摇
        public bool canInterruptEding;   //可被中断后摇摇
    }

}
