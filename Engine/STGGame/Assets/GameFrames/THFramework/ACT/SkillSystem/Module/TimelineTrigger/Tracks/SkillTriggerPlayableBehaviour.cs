using System;
using UnityEngine;
using UnityEngine.Playables;

namespace THGame.Skill
{
    public class SkillTriggerPlayableBehaviour : PlayableBehaviour
    {
        public SkillTriggerPlayableClip clip;
        private PlayableDirector _director;
        private AbstractSkillTrigger _trigger;
        private SkillTimelineContext _context = new SkillTimelineContext();

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            _director = playable.GetGraph().GetResolver() as PlayableDirector;
            var curTime = _director != null ? _director.time : 0;;
            _context.owner = _director?.gameObject;
            _context.tick = (int)Math.Ceiling(curTime / Time.fixedDeltaTime);
            _trigger?.Start(_context);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            //这里是相对时间转化为绝对时间,而且只有在有Clip下才跑这里
            var curTime = _director != null ? _director.time : 0;
            _context.owner = _director?.gameObject;
            _context.tick = (int)Math.Ceiling(curTime / Time.fixedDeltaTime);
            _trigger?.Update(_context);
        }


        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            var director = playable.GetGraph().GetResolver() as PlayableDirector;
            var curTime = _director != null ? _director.time : 0;
            _context.owner = director?.gameObject;  //结束后无法访问
            _context.tick = (int)Math.Ceiling(curTime / Time.fixedDeltaTime);
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