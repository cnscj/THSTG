﻿using System;

namespace THGame
{
    [System.Serializable]
    public class SkillTimelineClip : SkillTimelineBehaviour
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
    }

}