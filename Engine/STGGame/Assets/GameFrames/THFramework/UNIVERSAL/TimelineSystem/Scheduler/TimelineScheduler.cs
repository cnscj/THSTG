using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class TimelineScheduler : MonoSingleton<TimelineScheduler>
    {
        public float frameScale = 1f;

        private HashSet<ScheduleSequence> _scheduleLists = new HashSet<ScheduleSequence>();
        private Queue<ScheduleSequence> _scheduledLists = new Queue<ScheduleSequence>();
        private int lastFrameCount;

        public void Schedule(ScheduleSequence list)
        {
            if (_scheduleLists.Contains(list))
                return;

            _scheduleLists.Add(list);
            list.Start();
        }

        public void UnSchedule(ScheduleSequence list)
        {
            if (!_scheduleLists.Contains(list))
                return;

            _scheduledLists.Enqueue(list);
        }

        public void Update()
        {
            //如果任务表执行完毕,则移除
            if (_scheduleLists == null || _scheduleLists.Count <= 0)
                return;

            int curFrameCount = Time.frameCount;
            curFrameCount = (int)(curFrameCount * frameScale);

            if (curFrameCount == lastFrameCount)
                return;

            foreach (var list in _scheduleLists)
            {
                list.Update(curFrameCount);

                if (curFrameCount >= list.EndTime)
                {
                    _scheduledLists.Enqueue(list);
                    list.End();
                }
            }

            while (_scheduledLists.Count > 0)
            {
                var list = _scheduledLists.Dequeue();
                _scheduleLists.Remove(list);
            }

            lastFrameCount = curFrameCount;
        }
    }
}