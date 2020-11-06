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
        public int time;            //第几帧开始
        public int duration;        //耗时

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
        private SortedDictionary<int, Queue<SkillScheduleJob>> _scheduleJobs;

    }

}