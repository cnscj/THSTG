using System;
namespace THGame
{
    //按键状态
    public class SkillInputStateInfo
    {
        public static readonly float INTERVAL_SHOT_PRESS = 0.65f;    //这个数以下判定为短按,以上判断为长按

        public static readonly int KEYSTATE_NONE = 0x0;
        public static readonly int KEYSTATE_PRESS = 0x1;

        public IComparable keyCode;
        public int state;
        public float responTime = INTERVAL_SHOT_PRESS;
        public float releaseTimeout = -1f;

        public float durationTime;
        public float timeStamp;
        public bool callbackEnabled;
    }
}

