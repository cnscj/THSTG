using System;
namespace THGame
{
    //按键状态
    public class SkillInputStateInfo
    {
        public static readonly int KEYSTATE_NONE = 0x0;
        public static readonly int KEYSTATE_PRESS = 0x1;

        public IComparable keyCode;
        public int state;

        public float durationTime;
        public float timeStamp;
        public bool callbackEnabled;
    }
}

