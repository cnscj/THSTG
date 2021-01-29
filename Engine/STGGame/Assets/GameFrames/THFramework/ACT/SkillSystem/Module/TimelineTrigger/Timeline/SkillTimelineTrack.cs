using System;

namespace THGame
{
    public abstract class SkillTimelineTrack
    {
        public event Action onStart;
        public event Action onEnd;

        private float _startTime;
        private float _durationTime;

        public string Name { get; set; }
        public int EndFrame => (StartFrame + DurationFrame - 1);
        public bool Enabled { get; set; } = true;
        public bool IsExecuting { get; protected set ; }

        public int StartFrame
        {
            get
            {
                return (int)Math.Max(0, SkillTimelineManager.GetInstance().frameRate * _startTime);
            }
            protected set
            {
                _startTime = value / SkillTimelineManager.GetInstance().frameRate;
            }
        }
        public int DurationFrame
        {
            get
            {
                return (int)Math.Max(1, SkillTimelineManager.GetInstance().frameRate * _durationTime);
            }
            protected set
            {
                _durationTime = value / SkillTimelineManager.GetInstance().frameRate;
            }
        }

        public SkillTimelineTrack(float startTime = 0 , float durationTime = -1)
        {
            _startTime = startTime;
            _durationTime = durationTime;
        }

        public virtual void Parse(string[] args) { }

        public virtual void Seek(int startFrame) { }

        public virtual void Reset() { }

        public void Start(SkillTimelineDirector director)
        {
            IsExecuting = true;
            if (!Enabled)
                return;

            onStart?.Invoke();
            OnStart(director);
        }

        public void Update(int tickFrame)
        {
            if (!Enabled)
                return;

            OnUpdate(tickFrame);
        }

        public void End()
        {
            IsExecuting = false;
            if (!Enabled)
                return;

            onEnd?.Invoke();
            OnEnd();

        }

        protected virtual void OnStart(SkillTimelineDirector director) {}
        protected virtual void OnUpdate(int tickFrame){}
        protected virtual void OnEnd(){}
    }

}
