using System;
namespace THGame
{
    //按键状态
    public class SkillInputStateInfo
    {
        public IComparable keyCode;
        public int state;

        public float durationTime;
        public float timeStamp;
        public bool callbackEnabled;
    }
}

