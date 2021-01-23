using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public abstract class SkillTimelineTrack
    {
        private int _startTime;
        private int _durationTime;

        public int StartTime { get => _startTime; protected set { _startTime = value; } }

        public int DurationTime { get => _durationTime; protected set { _durationTime = value; } }

        public int EndTime => StartTime + DurationTime;

        public SkillTimelineTrack(int startFrame , int durationTime)
        {
            _startTime = startFrame;
            _durationTime = durationTime;
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
