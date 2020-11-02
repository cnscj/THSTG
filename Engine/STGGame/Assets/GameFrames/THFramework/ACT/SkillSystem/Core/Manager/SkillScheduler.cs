using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;

namespace THGame
{
    //采用时间片轮转的方式
    public class SkillScheduleJob : IComparable
    {
        public float time;
        public int CompareTo(object obj)
        {
            SkillScheduleJob job = obj as SkillScheduleJob;
            return time.CompareTo(job);
        }
    }

    public class SkillScheduleList
    {

    }

    public class SkillScheduler
    {
        //采用时间片轮转的方式
        public Action onExecute;
        public Action onFinish;
        private PriorityQueue<SkillScheduleJob> _scheduleJobs;//一个按时间优先度排序的队列,每次检测头部
        private Queue<SkillScheduleJob> _jobCache = new Queue<SkillScheduleJob>();


    }

}