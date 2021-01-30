using System;

namespace THGame
{
    public abstract class SkillTimelineTrack
    {
        public event Action onStart;
        public event Action onEnd;

        protected float _startTime;
        protected float _durationTime;

        public string Name { get; set; }
        public string[] Args { get; protected set; }
        public int EndFrame => (StartFrame + DurationFrame - 1);
        public bool Enabled { get; set; } = true;
        public bool IsExecuting { get; protected set ; }

        public int StartFrame
        {
            get
            {
                return Math.Max(0, SkillTimelineManager.GetInstance().Time2Frame(_startTime));
            }
            protected set
            {
                _startTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }
        public int DurationFrame
        {
            get
            {
                return Math.Max(1, SkillTimelineManager.GetInstance().Time2Frame(_durationTime));
            }
            protected set
            {
                _durationTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }

        public SkillTimelineTrack(float startTime = 0 , float durationTime = -1)
        {
            _startTime = startTime;
            _durationTime = durationTime;
        }

        public virtual void Parse(string[] args)
        {
            Args = args;
            OnPares(args);
        }

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

        protected virtual void OnPares(string[] args) { }
    }

}
