using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;

namespace THGame
{
    public class ScheduleSequence : AbstractScheduleTrack
    {
        public Action onStart;
        public Action onEnd;

        private SortedDictionary<int, HashSet<AbstractScheduleTrack>> _scheduleJobs = new SortedDictionary<int, HashSet<AbstractScheduleTrack>>();
        private MaxHeap<AbstractScheduleTrack, int> _scheduleJobsEndTime = new MaxHeap<AbstractScheduleTrack, int>();
        private HashSet<AbstractScheduleTrack> _schedulingJobs = new HashSet<AbstractScheduleTrack>();
        private Queue<AbstractScheduleTrack> _scheduledJobs = new Queue<AbstractScheduleTrack>();

        public ScheduleSequence() : base(0, 1) { }
        public ScheduleSequence(int start, int length) : base(start, length)
        {

        }

        public void AddJob(AbstractScheduleTrack job)
        {
            if (job == null)
                return;

            HashSet<AbstractScheduleTrack> jobSet;
            if (!_scheduleJobs.TryGetValue(job.StartTime, out jobSet))
            {
                jobSet = new HashSet<AbstractScheduleTrack>();
                _scheduleJobs.Add(job.StartTime, jobSet);
            }
            jobSet.Add(job);

            _scheduleJobsEndTime.Add(job, job.EndTime);
            StartTime = _scheduleJobsEndTime.Max.Key.EndTime;
        }

        public void RemoveJob(AbstractScheduleTrack job)
        {
            if (job == null)
                return;

            if (_scheduleJobs.TryGetValue(job.StartTime, out var jobSet))
            {
                jobSet.Remove(job);
                if (jobSet.Count <= 0)
                {
                    _scheduleJobs.Remove(job.StartTime);
                }
            }

            _scheduleJobsEndTime.Remove(job);
            DurationTime = (_scheduleJobsEndTime.Count > 0 ?_scheduleJobsEndTime.Max.Key.EndTime : 0);
        }


        public override void Start()
        {
            StartTime = Time.frameCount;
            onStart?.Invoke();
        }

        public override void End()
        {
            onEnd?.Invoke();
        }

        public override void Update(int timeTick)
        {
            if (_scheduleJobs == null || _scheduleJobs.Count <= 0)
                return;

            int curFrame = timeTick - StartTime;

            //帧列表
            if (_scheduleJobs.TryGetValue(curFrame, out var jobList))
            {
                //扔进执行表执行
                foreach (var job in jobList)
                {
                    _schedulingJobs.Add(job);
                    job.Start();
                }
            }

            //持续执行
            if (_schedulingJobs.Count > 0)
            {
                foreach (var job in _schedulingJobs)
                {
                    //每帧执行
                    job.Update(curFrame);

                    //检查结束
                    if (curFrame >= job.EndTime)
                    {
                        _scheduledJobs.Enqueue(job);
                        job.End();
                    }
                }
            }

            //检查是否结束
            while (_scheduledJobs.Count > 0)
            {
                var job = _scheduledJobs.Dequeue();
                _schedulingJobs.Remove(job);
            }
        }
    }
}