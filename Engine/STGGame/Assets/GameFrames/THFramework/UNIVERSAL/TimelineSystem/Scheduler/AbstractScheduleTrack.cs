using System;

namespace THGame
{
    public abstract class AbstractScheduleTrack : IScheduleJob
    {
        private int _startTime;            //第几帧开始
        private int _durationTime;        //执行时长

        public int StartTime { get => _startTime; protected set { _startTime = value; } }

        public int DurationTime { get => _durationTime; protected set { _durationTime = value; } }

        public int EndTime => StartTime + DurationTime;

        public AbstractScheduleTrack(int start, int length)
        {
            _startTime = start;
            _durationTime = length;
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