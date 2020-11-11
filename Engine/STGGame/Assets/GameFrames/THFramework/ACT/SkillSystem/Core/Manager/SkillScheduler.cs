using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;

namespace THGame
{
    public class SkillScheduleJob
    {
        public readonly int time;            //第几帧开始
        public readonly int duration;        //执行时长

        public Action onStart;
        public Action<int> onUpdate;
        public Action onEnd;
        public SkillScheduleJob(int startTime,int length)
        {
            time = startTime;
            duration = length;
        }

        public virtual void Star()
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

    public class SkillScheduleList
    {
        public int startFrame;       //开始帧
        public int durationFrame;    //最长时长
        public Action onStart;
        public Action onEnd;

        private SortedDictionary<int, HashSet<SkillScheduleJob>> _scheduleJobs = new SortedDictionary<int, HashSet<SkillScheduleJob>>();
        private HashSet<SkillScheduleJob> _schedulingJobs = new HashSet<SkillScheduleJob>();
        private Queue<SkillScheduleJob> _scheduledJobs = new Queue<SkillScheduleJob>();

        public void AddJob(SkillScheduleJob job)
        {
            if (job == null)
                return;

            HashSet<SkillScheduleJob> jobSet;
            if (!_scheduleJobs.TryGetValue(job.time,out jobSet))
            {
                jobSet = new HashSet<SkillScheduleJob>();
                _scheduleJobs.Add(job.time, jobSet);
            }
            jobSet.Add(job);

            RefreshList();
        }

        public void RemoveJob(SkillScheduleJob job)
        {
            if (job == null)
                return;

            if (_scheduleJobs.TryGetValue(job.time,out var jobSet))
            {
                jobSet.Remove(job);
                if (jobSet.Count <= 0)
                {
                    _scheduleJobs.Remove(job.time);
                }
            }

            RefreshList();
        }

        private void RefreshList()  //FIXME:这里从字典中找出最长一段性能不是很好
        {
            //找出最长的结束帧
            durationFrame = -1;
            foreach (var jobSet in _scheduleJobs.Values)
            {
                foreach (var job in jobSet)
                {
                    durationFrame = Math.Max(durationFrame, (job.time + job.duration));
                }
            }
        }

        public void Start()
        {
            startFrame = SkillScheduler.GetFrameCount();
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
                    job.Star();
                }
            }

            //持续执行
            if (_schedulingJobs.Count > 0)
            {
                foreach(var job in _schedulingJobs)
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
            while(_scheduledJobs.Count > 0)
            {
                var job = _scheduledJobs.Dequeue();
                _schedulingJobs.Remove(job);
            }
        }
    }

    public class SkillScheduler
    {
        public static int GetFrameCount() { return Time.frameCount; }

        private HashSet<SkillScheduleList> _scheduleLists = new HashSet<SkillScheduleList>();
        private Queue<SkillScheduleList> _scheduledLists = new Queue<SkillScheduleList>();

        public void Schedule(SkillScheduleList list)
        {
            if (_scheduleLists.Contains(list))
                return;

            _scheduleLists.Add(list);
            list.Start();
        }

        public void UnSchedule(SkillScheduleList list)
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

            foreach(var list in _scheduleLists)
            {
                int curFrameCount = SkillScheduler.GetFrameCount();
                list.Update(curFrameCount);

                if (curFrameCount >= list.startFrame + list.durationFrame)
                {
                    _scheduledLists.Enqueue(list);
                    list.End();
                }
            }

            while(_scheduledLists.Count > 0)
            {
                var list = _scheduledLists.Dequeue();
                _scheduleLists.Remove(list);

            }
        }
    }

}