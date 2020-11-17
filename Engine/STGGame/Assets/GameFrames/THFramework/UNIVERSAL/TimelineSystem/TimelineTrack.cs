using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class TimelineTrack
    {
        public readonly int time;            //第几帧开始
        public readonly int duration;        //执行时长
        public int type;

        public int EndTime => time + duration;

        public Action onStart;
        public Action<int> onUpdate;
        public Action onEnd;
        public TimelineTrack(int startTime, int length)
        {
            time = startTime;
            duration = length;
        }

        public virtual void Start()
        {
            onStart?.Invoke();
        }

        public virtual void Update(int tickTime)
        {
            int curFrame = tickTime - time;
            onUpdate?.Invoke(curFrame);
        }

        public virtual void End()
        {
            onEnd?.Invoke();
        }

    }
}