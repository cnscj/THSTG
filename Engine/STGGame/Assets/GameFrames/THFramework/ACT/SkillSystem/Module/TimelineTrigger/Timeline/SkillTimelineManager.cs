using System;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class SkillTimelineManager : Singleton<SkillTimelineManager>
    {
        public float frameScale = 1f;
        public float frameRate => 1/Time.fixedDeltaTime;
        public int FrameTick
        {
            get
            {
                int finalFrame = (int)(Time.frameCount * frameScale);
                return finalFrame;
            }
        }

        public int Time2Frame(float time)
        {
            return (int)Math.Ceiling(frameRate * time); //向上取整一帧
        }

        public float Frame2Time(int frame)
        {
            return  frame / frameRate;
        }
    }

}
