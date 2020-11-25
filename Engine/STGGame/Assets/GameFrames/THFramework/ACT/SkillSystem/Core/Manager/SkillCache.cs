using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace THGame
{
    public class SkillCache
    {
        public SkillCountdownCache countdownCache;
        public SkillcCommandCache commandCache;


        public void Update()
        {
            countdownCache.Update();
            commandCache.Update();
        }
    }

    public class SkillCountdownCache                //CD冷却Cache
    {
        public class Data
        {
            public string key;
            public float timeStamp;
            public float cd;
            public Action callback;
        }

        public float queryFrequentness;             //查询频度
        private Dictionary<string, Data> _cdDict;
        private float _lastQueryTimeStamp;

        public bool IsInCD(string key)
        {
            return false;
        }

        public void Update()
        {
            //移除过期cd并执行回调


            _lastQueryTimeStamp = Time.fixedTime;
        }


        protected Data GetOrCreateData()
        {
            return null;
        }

        protected void ReleaseData(Data data)
        {

        }
    }

    public class SkillcCommandCache     //指令Cache
    {
        public void Update()
        {

        }
    }
}