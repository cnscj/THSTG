﻿using System;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace THGame
{
    public class SkillManager : MonoSingleton<SkillManager>
    {
        private SkillInputReceiver _skillInputReceiver;     //接收器
        private SkillCdCache _skillCdCache;                 //cd缓存池
        private SkillCastTrigger _skillCastTrigger;         //触发器
        private SkillEventDispatcher _skillDispatcher;      //派发器


        private SkillData _curSkillData;                    //当前使用的技能数据


        public SkillInputReceiver GetInputReceiver() { return _skillInputReceiver = _skillInputReceiver ?? CreateManager<SkillInputReceiver>("InputReceiver"); }
        public SkillCdCache GetCdCache(){ return _skillCdCache = _skillCdCache ?? CreateManager<SkillCdCache>("CountdownCache"); }
        public SkillCastTrigger GetCastTrigger() { return _skillCastTrigger = _skillCastTrigger ?? CreateManager<SkillCastTrigger>("CastTrigger"); }
        public SkillEventDispatcher GetEventDispatcher() { return _skillDispatcher = _skillDispatcher ?? new SkillEventDispatcher(); }


        public void SetSkillData(SkillData data)
        {
            _curSkillData = data;
            //TODO:设置短按长按的响应时间

        }

        public SkillData GetSkillData()
        {
            return _curSkillData;
        }

        private void Start()
        {
            InitInputSetting();//初始化按键信息


        }

        private void InitInputSetting()
        {
            GetInputReceiver().OnKeyDown += (stateInfo) => { OnSkillTouch((SkillSkillType)stateInfo.keyCode, SkillInputType.KeyDown); };
            GetInputReceiver().OnKeyUp += (stateInfo) => { OnSkillTouch((SkillSkillType)stateInfo.keyCode, SkillInputType.KeyUp); };
            GetInputReceiver().OnShotPress += (stateInfo) => { OnSkillTouch((SkillSkillType)stateInfo.keyCode, SkillInputType.ShotPress); };
            GetInputReceiver().OnLongPress += (stateInfo) => { OnSkillTouch((SkillSkillType)stateInfo.keyCode, SkillInputType.LongPress); };
        }

        private void OnSkillTouch(SkillSkillType skillType,SkillInputType inputType)
        {
            //可能会定义一些技能之外的响应,但是这里只处理技能的
            var skillData = GetSkillData();
            if (skillData == null)
                return;

            //调用触发器,如果在某个时间段内进行操作才能进入相应状态,否则超过则进入默认退出状态(应该是等动画自然状态回归)
            //从对应太进入,到自然释放过程会有一个空窗期(冷却期(_空窗期)_触发期s(_空窗期)_回归期,回归期计时可能从冷却期结束开始计时
            var trigger = GetCastTrigger();


            //TODO:同一个技能会有多个阶段,并且可能会按照不同的技能时长决定长短按
            //获取触发的技能类型 
            //获取对应skillId
            //获取skillInfo,
            //检查CD
        }

        private T CreateManager<T>(string name) where T : MonoBehaviour
        {
            GameObject managerGObj = new GameObject(name);
            managerGObj.transform.SetParent(transform);
            T manager = managerGObj.AddComponent<T>();

            return manager;
        }  
    }
}
