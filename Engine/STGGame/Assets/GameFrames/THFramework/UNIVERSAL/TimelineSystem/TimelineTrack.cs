using System;

namespace THGame
{
    public class TimelineTrack
    {
        public readonly int time;            //第几帧开始
        public readonly int duration;        //执行时长
        public int type;

        public int EndTime => time + duration;

        public TimelineTrack(int startTime, int length)
        {
            time = startTime;
            duration = length;
        }

        public virtual void Start()
        {
            
        }

        public virtual void Update(int tickTime)
        {
            
        }

        public virtual void End()
        {

        }

    }
}