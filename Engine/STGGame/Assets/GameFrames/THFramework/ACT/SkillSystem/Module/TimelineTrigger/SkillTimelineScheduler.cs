using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillTimelineScheduler : MonoBehaviour 
    {
        public float frameScale = 1f;
        private SkillTimelineSequence _skillTimelineSequence = new SkillTimelineSequence(0,1);
        private int _startTime;
        private int _lastFrameCount;

        public bool IsCompleted => GetCurFrameCount() >= _skillTimelineSequence.EndTime;

        public void AddTrack(SkillTimelineTrack track)
        {
            _skillTimelineSequence.AddTrack(track);
        }

        public void RemoveTrack(SkillTimelineTrack track)
        {
            _skillTimelineSequence.RemoveTrack(track);
        }

        public void Schedule(int offset)
        {
            _startTime = offset + GetGameFrameCount();

        }

        private void Update()
        {
            if (_startTime <= 0)
                return;

            int curFrameCount = GetCurFrameCount();

            if (curFrameCount < 0)
                return;

            if (curFrameCount == _lastFrameCount)
                return;

            if (IsCompleted)
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
            int curFrameCount = GetGameFrameCount() - _startTime;
            curFrameCount = (int)(curFrameCount * frameScale);

            return curFrameCount;
        }
    }
}

