namespace THGame
{
    public class SkillTimelineBehaviour : SkillTimelineAsset
    {
        public virtual void OnStart(SkillTimelineContext context) { }
        public virtual void OnUpdate(SkillTimelineContext context) { }
        public virtual void OnEnd(SkillTimelineContext context) { }

        public virtual void OnCreate(string[] info, string[] args) { }
        public virtual void OnDestroy() { }
    }
}