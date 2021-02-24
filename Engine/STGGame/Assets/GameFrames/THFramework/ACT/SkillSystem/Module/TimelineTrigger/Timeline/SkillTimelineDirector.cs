using System;
using UnityEngine;

namespace THGame
{
    public class SkillTimelineDirector : MonoBehaviour 
    {
        public event Action onCompleted;
        public SkillTimelineSequence timelineSequence;

        private int _startFrame;
        private int _offsetFrame;
        private int _fixFrame;
        private int _pauseFrameTick;

        public bool IsCompleted
        {
            get
            {
                if (timelineSequence == null)
                    return true;

                return GetCurFrameTick() > timelineSequence.EndFrame;
            }
        }
        public bool IsPause { get; protected set; } = false;

        public void Play(int offsetFrame = 0)
        {
            if (timelineSequence == null)
                return;

            IsPause = false;
            _offsetFrame = Mathf.Max(0, offsetFrame);
            _startFrame = GetFrameTick();
            _fixFrame = 0;

            int curFrameCount = GetCurFrameTick();
            if (curFrameCount < 0)
                return;

            var tickFrame = curFrameCount - timelineSequence.StartFrame;

            timelineSequence.Owner = this;
            timelineSequence.Seek(tickFrame);
        }

        public void Pause()
        {
            if (IsPause)
                return;

            IsPause = true;
            _fixFrame = 0;
            _pauseFrameTick = GetFrameTick();
        }

        public void Resume()
        {
            if (!IsPause)
                return;

            IsPause = false;
            _fixFrame = GetFrameTick() - _pauseFrameTick;

        }

        public void Stop()
        {
            if (timelineSequence == null)
                return;


            IsPause = true;
            timelineSequence.Reset();
        }

        private void Update()
        {
            if (timelineSequence == null)
                return;

            if (IsPause)
                return;

            if (IsCompleted)
                return;

            int curFrameCount = GetCurFrameTick();
            if (curFrameCount < 0)
                return;

            var tickFrame = curFrameCount - timelineSequence.StartFrame;
            if (tickFrame == timelineSequence.StartFrame)
            {
                timelineSequence.Start(gameObject);
            }
            timelineSequence.Update(tickFrame);
            if (curFrameCount >= timelineSequence.EndFrame)
            {
                timelineSequence.End();
                onCompleted?.Invoke();
            }
        }

        private int GetFrameTick()
        {
            return SkillTimelineManager.GetInstance().FrameTick;
        }

        private int GetCurFrameTick()
        {
            int curFrameCount = GetFrameTick() - _startFrame - _fixFrame;
            return curFrameCount + _offsetFrame;
        }
    }
}

