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

        public float StartTime { get => _startTime; protected set { _startTime = value; }}
        public float DurationTime { get => _durationTime; protected set { _durationTime = value; } }

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

        public virtual void Initialize(string[] info,string[] args)
        {
            if (info != null && info.Length > 0)
            {
                if (info.Length > 1) float.TryParse(info[0], out _startTime);
                if (info.Length > 2) float.TryParse(info[1], out _durationTime);
            }
            Args = args;
            OnCreate(info,args);
        }

        public virtual void Seek(int startFrame) { }

        public virtual void Reset() { }

        public virtual void Start(object owner)
        {
            IsExecuting = true;
            if (!Enabled)
                return;

            onStart?.Invoke();
            OnStart(owner);
        }

        public virtual void Update(int tickFrame)
        {
            if (!Enabled)
                return;

            OnUpdate(tickFrame);
        }

        public virtual void End()
        {
            IsExecuting = false;
            if (!Enabled)
                return;

            onEnd?.Invoke();
            OnEnd();

        }

        public virtual void Dispose()
        {
            OnDestroy();
        }

        protected virtual void OnStart(object owner) {}
        protected virtual void OnUpdate(int tickFrame){}
        protected virtual void OnEnd(){}

        protected virtual void OnCreate(string[] info, string[] args) { }
        protected virtual void OnDestroy() { }
    }

}
