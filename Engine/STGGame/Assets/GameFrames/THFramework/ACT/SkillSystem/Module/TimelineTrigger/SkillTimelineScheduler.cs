using System;
using UnityEngine;

namespace THGame
{
    public class SkillTimelineScheduler : MonoBehaviour 
    {
        //TODO:
        public float frameScale = 1f;
        public int frameRate = 24;
        public event Action onCompleted;

        private SkillTimelineSequence _skillTimelineSequence = new SkillTimelineSequence();
        private int _startFrame;
        private int _offsetFrame;
        private int _lastFrameCount;

        public bool IsCompleted => GetCurFrameCount() > _skillTimelineSequence.EndFrame;

        public SkillTimelineScheduler()
        {
            _skillTimelineSequence.onEnd += OnSequenceCompleted;
        }

        public void AddTrack(SkillTimelineTrack track)
        {
            _skillTimelineSequence.AddTrack(track);
        }

        public void RemoveTrack(SkillTimelineTrack track)
        {
            _skillTimelineSequence.RemoveTrack(track);
        }

        public void Schedule(int offsetFrame = 0)
        {
            _offsetFrame = Mathf.Max(0, offsetFrame);
            _startFrame = GetGameFrameCount();
            _skillTimelineSequence.Play(offsetFrame);
        }

        private void Update()
        {
            if (_skillTimelineSequence.TotalCount <= 0)
                return;

            if (IsCompleted)
                return;

            int curFrameCount = GetCurFrameCount();
            if (curFrameCount < 0)
                return;

            if (curFrameCount > 0 && curFrameCount == _lastFrameCount)
                return;

            _skillTimelineSequence.Update(curFrameCount);

            _lastFrameCount = curFrameCount;
        }

        private int GetGameFrameCount()
        {
            return Time.frameCount;
        }

        private int GetCurFrameCount()
        {
            int curFrameCount = GetGameFrameCount() - _startFrame;
            curFrameCount = (int)(curFrameCount * frameScale);

            return curFrameCount + _offsetFrame;
        }

        private void OnSequenceCompleted()
        {
            onCompleted?.Invoke();
        }
    }
}

