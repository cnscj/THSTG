using System;

namespace THGame
{
    public abstract class SkillTimelineTrack
    {
        public event Action onStart;
        public event Action onEnd;
        public event Action<int> onUpdate;

        private int _startTime;
        private int _durationTime;

        public int StartFrame { get => _startTime; protected set { _startTime = value; } }
        public int DurationFrame { get => _durationTime; protected set { _durationTime = value; } }
        public int EndFrame => (StartFrame + DurationFrame - 1);
        public bool Enabled { get; set; } = true;
        public bool IsExecuting { get; set ; }
        public string Name { get; set; }

        public SkillTimelineTrack(int startFrame , int durationTime)
        {
            _startTime = startFrame;
            _durationTime = durationTime;
        }

        public void Update(int tickTime)
        {
            if (!Enabled) return;

            if (tickTime == StartFrame)
            {
                onStart?.Invoke();
                OnStart();
            }

            onUpdate?.Invoke(tickTime);
            OnUpdate(tickTime);

            if (tickTime == EndFrame)
            {
                onEnd?.Invoke();
                OnEnd();
            }
        }

        protected virtual void OnStart(){}
        protected virtual void OnUpdate(int tickTime){}
        protected virtual void OnEnd(){}
    }

}
