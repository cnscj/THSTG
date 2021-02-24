using System;

namespace THGame
{
    public class SkillTimelineClip : SkillTimelineAsset, SkillTimelineBehaviour
    {
        public event Action onStart;
        public event Action onEnd;

        public readonly SkillTimelineAsset asset;
        public readonly SkillTimelineBehaviour behaviour;

        public int EndFrame => (StartFrame + DurationFrame - 1);
        public bool Enabled { get; set; } = true;
        public bool IsExecuting { get; protected set ; }

        public int StartFrame
        {
            get
            {
                return Math.Max(0, SkillTimelineManager.GetInstance().Time2Frame(asset.startTime));
            }
            protected set
            {
                asset.startTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }
        public int DurationFrame
        {
            get
            {
                return Math.Max(1, SkillTimelineManager.GetInstance().Time2Frame(asset.durationTime));
            }
            protected set
            {
                asset.durationTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }

        public object Owner { get; set ; }

        public SkillTimelineClip()
        {
            this.asset = this;
            this.behaviour = this;
        }

        public SkillTimelineClip(SkillTimelineAsset asset, SkillTimelineBehaviour behaviour)
        {
            this.asset = asset;
            this.behaviour = behaviour;
        }

        public virtual void Initialize(string[] info,string[] args)
        {
            if (info != null && info.Length > 0)
            {
                if (info.Length > 1) float.TryParse(info[0], out asset.startTime);
                if (info.Length > 2) float.TryParse(info[1], out asset.durationTime);
            }
            asset.args = args;
            behaviour?.OnCreate(info,args);
        }

        public virtual void Seek(int startFrame) { }

        public virtual void Reset() { }

        public virtual void Start(object owner)
        {
            IsExecuting = true;
            if (!Enabled)
                return;

            onStart?.Invoke();
            behaviour?.OnStart(owner);
        }

        public virtual void Update(int tickFrame)
        {
            if (!Enabled)
                return;

            behaviour?.OnUpdate(tickFrame);
        }

        public virtual void End()
        {
            IsExecuting = false;
            if (!Enabled)
                return;

            onEnd?.Invoke();
            behaviour?.OnEnd();

        }

        public virtual void Dispose()
        {
            behaviour?.OnDestroy();
        }


        public virtual void OnStart(object owner){}
        public virtual void OnUpdate(int tickFrame){}
        public virtual void OnEnd(){}

        public virtual void OnCreate(string[] info, string[] args){}
        public virtual void OnDestroy(){}
    }

}
