using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public abstract class AbstractSkillTrigger : ISkillTrigger
    {
        private string _triggerType;
        private int _startTime;
        private int _durationTime;

        public int StartTime { get => _startTime; protected set { _startTime = value; } }

        public int DurationTime { get => _durationTime; protected set { _durationTime = value; } }

        public int EndTime => StartTime + DurationTime;

        public string TriggerType => _triggerType;

        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update(int tickTime)
        {
            throw new System.NotImplementedException();
        }

        public virtual void End()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Reset()
        {
            throw new System.NotImplementedException();
        }
    }

}
