using System;
using UnityEngine;

namespace THGame
{
    public class SkillTimelineDirector : MonoBehaviour 
    {
        public event Action onCompleted;
        public SkillTimelinePlayable playable;

        private int _startFrame;
        private int _offsetFrame;
        private int _fixFrame;
        private int _pauseFrameTick;

        public bool IsCompleted
        {
            get
            {
                if (playable == null)
                    return true;

                return GetCurFrameTick() >= playable.EndFrame;
            }
        }

        public bool IsEnd
        {
            get
            {
                if (playable == null)
                    return true;

                return GetCurFrameTick() > playable.EndFrame;
            }
        }


        public bool IsPause { get; protected set; } = false;

        public void Play(int offsetFrame = 0)
        {
            if (playable == null)
                return;

            IsPause = false;
            _offsetFrame = Mathf.Max(0, offsetFrame);
            _startFrame = GetFrameTick();
            _fixFrame = 0;

            int curFrameCount = GetCurFrameTick();
            if (curFrameCount < 0)
                return;

            playable.Seek(curFrameCount);
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
            if (playable == null)
                return;

            IsPause = true;
            playable.Reset();
        }

        private void Update()
        {
            if (playable == null)
                return;

            if (IsPause)
                return;

            if (IsEnd)
                return;

            int curFrameCount = GetCurFrameTick();
            if (curFrameCount < 0)
                return;

            playable.Update(curFrameCount, this);
            if (IsCompleted) onCompleted?.Invoke();
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

