using UnityEngine.Playables;

namespace THGame.Skill
{
    public class SkillTriggerPlayableBehaviour : PlayableBehaviour
    {
        public SkillTriggerPlayableClip clip;
        private AbstractSkillTrigger _trigger;
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;

            _trigger?.Start(director.gameObject);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _trigger?.Update((int)info.deltaTime);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            _trigger?.End();
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