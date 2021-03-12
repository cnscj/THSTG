namespace THGame
{
    public abstract class AbstractSkillTrigger : SkillTimelineSequence
    {
        public string[] ArgsDesc
        {
            get
            {
#if UNITY_EDITOR
                return OnArgsDesc();
#else
        return null;
#endif
            }
        }
        
        public static AbstractSkillTrigger Create(SkillTimelineAsset asset)
        {
            var triggerFactor = SkillTriggerManager.GetInstance().GetFactory(asset.type);
            if (triggerFactor != null)
            {
                var trigger = triggerFactor.CreateTrigger();
                if (trigger != null)
                {
                    trigger.name = asset.name;
                    trigger.type = asset.type;

                    trigger.startTime = asset.startTime;
                    trigger.durationTime = asset.durationTime;

                    trigger.Initialize(null, asset.args);
                    return trigger;
                }
            }
            return default;
        }

        protected virtual string[] OnArgsDesc()
        {
            return default;
        }
    }

}
