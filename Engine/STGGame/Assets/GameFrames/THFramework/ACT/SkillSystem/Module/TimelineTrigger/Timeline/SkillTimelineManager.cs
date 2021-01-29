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
    }

}
