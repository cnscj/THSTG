using System;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class TimelineSequence
    {
        public int startFrame;       //开始帧
        public int durationFrame;    //最长时长

        public Action onStart;
        public Action onEnd;

        private SortedDictionary<int, HashSet<TimelineTrack>> _scheduleJobs = new SortedDictionary<int, HashSet<TimelineTrack>>();
        private HashSet<TimelineTrack> _schedulingJobs = new HashSet<TimelineTrack>();
        private Queue<TimelineTrack> _scheduledJobs = new Queue<TimelineTrack>();

        public void AddJob(TimelineTrack job)
        {
            if (job == null)
                return;

            HashSet<TimelineTrack> jobSet;
            if (!_scheduleJobs.TryGetValue(job.time, out jobSet))
            {
                jobSet = new HashSet<TimelineTrack>();
                _scheduleJobs.Add(job.time, jobSet);
            }
            jobSet.Add(job);

            RefreshMaxTime(job);
        }

        public void RemoveJob(TimelineTrack job)
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

            RefreshMaxTimes();//FIXME:这里从字典中找出最长一段性能不是很好
        }

        private void RefreshMaxTime(TimelineTrack job)
        {
            if (job == null)
                return;

            durationFrame = Math.Max(durationFrame, (job.time + job.duration));
        }

        private void RefreshMaxTimes()  
        {
            //找出最长的结束帧
            durationFrame = -1;
            foreach (var jobSet in _scheduleJobs.Values)
            {
                foreach (var job in jobSet)
                {
                    RefreshMaxTime(job);
                }
            }
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
                    if (curFrame >= (job.time + job.duration))
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