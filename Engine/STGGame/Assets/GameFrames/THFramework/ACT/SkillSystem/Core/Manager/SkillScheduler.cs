using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;

namespace THGame
{
    public class SkillScheduler
    {
        //采用时间片轮转的方式
        public class Job : IComparable
        {
            public float time;
            public int CompareTo(object obj)
            {
                Job job = obj as Job;
                return time.CompareTo(job);
            }
        }

        public Action onExecute;
        public Action onFinish;
        private PriorityQueue<Job> _scheduleJobs;//一个按时间优先度排序的队列,每次检测头部
    }

}