using System;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class SkillTimelineManager : Singleton<SkillTimelineManager>, ISkillTimelineBinder
    {
        
        private ISkillTimelineBinder mBinder;

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

        public int Time2Frame(double time)
        {
            return (int)Math.Ceiling(frameRate * time); //向上取整一帧
        }

        public double Frame2Time(int frame)
        {
            return  frame / frameRate;
        }

        public void SetBinder(ISkillTimelineBinder newBinder)
        {
            mBinder = newBinder ?? this;
        }

        public ISkillTimelineBinder GetBinder()
        {
            return mBinder ?? this;
        }

        public SkillTimelineBehaviour Conver(string type)
        {
            return default;
        }

        public void Rebind(SkillTimelineSequence sequence)
        {
            if (sequence == null)
                return;

            var binder = GetBinder();
            if (binder != null)
            {
                var behaviour = binder.Conver(sequence.type);
                if (behaviour != null)
                {
                    sequence.behaviour = behaviour;
                }
            }

            //递归遍历子节点
            if (sequence.sequences != null && sequence.sequences.Length > 0)
            {
                foreach (var childSequence in sequence.sequences)
                {
                    Rebind(childSequence);
                }
            }
        }
    }

}
