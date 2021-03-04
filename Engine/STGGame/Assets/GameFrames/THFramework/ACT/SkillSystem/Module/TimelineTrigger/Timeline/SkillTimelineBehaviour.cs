namespace THGame
{
    public abstract class SkillTimelineBehaviour : SkillTimelineAsset
    {
        public virtual void OnStart(object owner) { }
        public virtual void OnUpdate(int tickFrame) { }
        public virtual void OnEnd() { }

        public virtual void OnCreate(string[] info, string[] args) { }
        public virtual void OnDestroy() { }
    }
}