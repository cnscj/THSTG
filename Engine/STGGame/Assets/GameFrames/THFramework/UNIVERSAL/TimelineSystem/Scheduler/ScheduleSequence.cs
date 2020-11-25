using System;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;

namespace THGame
{
    public class ScheduleSequence
    {
        public int startFrame;       //开始帧
        public int durationFrame;    //最长时长

        public int EndTime => startFrame + durationFrame;

        public Action onStart;
        public Action onEnd;

        private SortedDictionary<int, HashSet<ScheduleTrack>> _scheduleJobs = new SortedDictionary<int, HashSet<ScheduleTrack>>();
        private MaxHeap<ScheduleTrack, int> _scheduleJobsEndTime = new MaxHeap<ScheduleTrack, int>();
        private HashSet<ScheduleTrack> _schedulingJobs = new HashSet<ScheduleTrack>();
        private Queue<ScheduleTrack> _scheduledJobs = new Queue<ScheduleTrack>();

        public void AddJob(ScheduleTrack job)
        {
            if (job == null)
                return;

            HashSet<ScheduleTrack> jobSet;
            if (!_scheduleJobs.TryGetValue(job.time, out jobSet))
            {
                jobSet = new HashSet<ScheduleTrack>();
                _scheduleJobs.Add(job.time, jobSet);
            }
            jobSet.Add(job);

            _scheduleJobsEndTime.Add(job, job.EndTime);
            durationFrame = _scheduleJobsEndTime.Max.Key.EndTime;
        }

        public void RemoveJob(ScheduleTrack job)
        {
            if (job == null)
                return;

            if (_scheduleJobs.TryGetValue(job.time, out var jobSet))
            {
                jobSet.Remove(job);
                if (jobSet.Count <= 0)
                {
                    _scheduleJobs.Remove(job.time);
                }
            }

            _scheduleJobsEndTime.Remove(job);
            durationFrame = (_scheduleJobsEndTime.Count > 0 ?_scheduleJobsEndTime.Max.Key.EndTime : 0);
        }


        public void Start()
        {
            startFrame = Time.frameCount;
            onStart?.Invoke();
        }

        public void End()
        {
            onEnd?.Invoke();
        }

        public void Update(int timeTick)
        {
            if (_scheduleJobs == null || _scheduleJobs.Count <= 0)
                return;

            int curFrame = timeTick - startFrame;

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