using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Collection;
using XLibrary.Package;
using Object = UnityEngine.Object;

namespace THGame
{
    public class SkillLoaderContext
    {
        public string loadType;
        public string skillName;
        public string assetKey;
    }

    public class SkillEventContext
    {
        public Object sender;
        public string type;
        public string args;
    }

    public class SkillFactory
    {
        public virtual void PlayEffect()
        {

        }

        public virtual void PlayAudio()
        {

        }

        public virtual void PlayAction()
        {

        }
    }

    public class SkillLoader
    {
        public virtual void LoadEffect(SkillLoaderContext context,Action<GameObject[]> callback)
        {

        }

        public virtual void LoadAudio(SkillLoaderContext context, Action<AudioClip[]> callback)
        {

        }

        public virtual void LoadAction(SkillLoaderContext context, Action<AnimationClip[]> callback)
        {

        }

        public virtual void LoadConfig(SkillLoaderContext context, Action<SkillData> callback)
        {

        }
    }

    public class SkillCache
    {
        public SkillItem Get()
        {
            return default;
        }
    }

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

    //触发器
    public class SkillTrigger
    {



    }

    public class SkillManager : MonoSingleton<SkillManager>
    {
        public SkillFactory Factory = new SkillFactory();
        public SkillLoader Loader = new SkillLoader();
        public SkillTrigger Trigger = new SkillTrigger();
        public SkillCache Cache = new SkillCache();


    }
}
