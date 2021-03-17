using UnityEngine.Playables;

namespace THGame.Skill
{
    public class SkillTriggerPlayableBehaviour : PlayableBehaviour
    {
        public SkillTriggerPlayableClip clip;
        private AbstractSkillTrigger _trigger;
        private SkillTimelineContext _context = new SkillTimelineContext();
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            _context.owner = director.gameObject;
            _trigger?.Start(_context);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _context.tick = (int)info.deltaTime;
            _trigger?.Update(_context);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            _trigger?.End(_context);
        }

        public new void OnPlayableCreate(Playable playable)
        {
            if (clip == null)
                return;

            var triggerFactor = SkillTriggerManager.GetInstance().GetFactory(clip.type);
            if (triggerFactor != null)
            {
                _trigger = triggerFactor.CreateTrigger();
                _trigger?.Initialize(null,clip.args);
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (clip == null)
                return;

            var triggerFactor = SkillTriggerManager.GetInstance().GetFactory(clip.type);
            if (triggerFactor != null)
            {
                _trigger?.Dispose();
                triggerFactor.RecycleTrigger(_trigger);
                _trigger = null;
            }
        }
    }
}