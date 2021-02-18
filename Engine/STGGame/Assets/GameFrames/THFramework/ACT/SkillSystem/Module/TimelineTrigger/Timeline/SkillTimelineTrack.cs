using System;

namespace THGame
{
    public abstract class SkillTimelineTrack : SkillTimelineAsset, ISkillTimelinePlayable
    {
        public event Action onStart;
        public event Action onEnd;

        public int EndFrame => (StartFrame + DurationFrame - 1);
        public bool Enabled { get; set; } = true;
        public bool IsExecuting { get; protected set ; }

        public int StartFrame
        {
            get
            {
                return Math.Max(0, SkillTimelineManager.GetInstance().Time2Frame(startTime));
            }
            protected set
            {
                startTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }
        public int DurationFrame
        {
            get
            {
                return Math.Max(1, SkillTimelineManager.GetInstance().Time2Frame(durationTime));
            }
            protected set
            {
                durationTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }

        public object Owner { get; set ; }

        public SkillTimelineTrack(float startTime = 0 , float durationTime = -1)
        {
            this.startTime = startTime;
            this.durationTime = durationTime;
        }

        public virtual void Initialize(string[] info,string[] args)
        {
            if (info != null && info.Length > 0)
            {
                if (info.Length > 1) float.TryParse(info[0], out startTime);
                if (info.Length > 2) float.TryParse(info[1], out durationTime);
            }
            this.args = args;
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
