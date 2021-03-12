using System;

namespace THGame
{
    [System.Serializable]
    public class SkillTimelineClip : SkillTimelineBehaviour
    {
        public event Action onStart;
        public event Action onEnd;

        public readonly SkillTimelineAsset asset;
        public SkillTimelineBehaviour behaviour;

        public int EndFrame => (StartFrame + DurationFrame - 1);
        public bool Enabled { get; set; } = true;
        public bool IsExecuting { get; protected set ; }


        public int StartFrame
        {
            get
            {
                if (asset == null)
                    return 0;

                return Math.Max(0, SkillTimelineManager.GetInstance().Time2Frame(asset.startTime));
            }
            protected set
            {
                if (asset == null)
                    return ;

                asset.startTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }

        public int DurationFrame
        {
            get
            {
                if (asset == null)
                    return 0;

                return Math.Max(1, SkillTimelineManager.GetInstance().Time2Frame(asset.durationTime));
            }
            protected set
            {
                if (asset == null)
                    return;

                asset.durationTime = SkillTimelineManager.GetInstance().Frame2Time(value);
            }
        }

        public static SkillTimelineClip Create(SkillTimelineAsset asset, SkillTimelineBehaviour behaviour)
        {
            var clip = new SkillTimelineClip(asset, behaviour);
            return clip;
        }

        public SkillTimelineClip()
        {
            this.asset = this;
            this.behaviour = this;
        }

        protected SkillTimelineClip(SkillTimelineAsset asset, SkillTimelineBehaviour behaviour)
        {
            this.asset = asset;
            this.behaviour = behaviour;
        }

        public void AssetValues(SkillTimelineAsset timelineAsset)
        {
            type = timelineAsset.type;
            args = timelineAsset.args;

            startTime = (float)timelineAsset.startTime;
            durationTime = (float)timelineAsset.durationTime;
        }

        public virtual void Initialize(string[] info,string[] args)
        {
            if (info != null && info.Length > 0)
            {
                if (info.Length > 1) double.TryParse(info[0], out asset.startTime);
                if (info.Length > 2) double.TryParse(info[1], out asset.durationTime);
            }
            asset.args = args;
            behaviour?.OnCreate(info,args);
        }

        public virtual void Start(SkillTimelineContext context)
        {
            IsExecuting = true;
            if (!Enabled)
                return;

            onStart?.Invoke();
            behaviour?.OnStart(context);
        }

        public virtual void Update(SkillTimelineContext context)
        {
            if (!Enabled)
                return;

            behaviour?.OnUpdate(context);
        }

        public virtual void End(SkillTimelineContext context)
        {
            IsExecuting = false;
            if (!Enabled)
                return;

            onEnd?.Invoke();
            behaviour?.OnEnd(context);

        }

        public virtual void Dispose()
        {
            behaviour?.OnDestroy();
        }
    }

}
