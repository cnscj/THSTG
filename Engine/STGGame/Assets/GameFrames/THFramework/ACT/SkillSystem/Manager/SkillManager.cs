using System;
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
        private SkillController _curentSkillController;     //控制器

        public SkillInputReceiver GetInputReceiver() { return _skillInputReceiver = _skillInputReceiver ?? CreateManager<SkillInputReceiver>("InputReceiver"); }
        public SkillCdCache GetCdCache(){ return _skillCdCache = _skillCdCache ?? CreateManager<SkillCdCache>("CountdownCache"); }
        public SkillCastTrigger GetCastTrigger() { return _skillCastTrigger = _skillCastTrigger ?? CreateManager<SkillCastTrigger>("CastTrigger"); }
        public SkillEventDispatcher GetEventDispatcher() { return _skillDispatcher = _skillDispatcher ?? new SkillEventDispatcher(); }
        public SkillController CurentSkillController
        {
            set
            {
                _curentSkillController = value;
            }
            get
            {
                return _curentSkillController;
            }
        }

        private void Start()
        {
            InitInputSetting();//初始化按键信息

           
        }

        private void InitInputSetting()
        {
            GetInputReceiver().OnKeyDown += (stateInfo) => { OnInput(stateInfo, SkillInputType.KeyDown); };
            GetInputReceiver().OnKeyUp += (stateInfo) => { OnInput(stateInfo, SkillInputType.KeyUp); };
            GetInputReceiver().OnShotPress += (stateInfo) => { OnInput(stateInfo, SkillInputType.ShotPress); };
            GetInputReceiver().OnLongPress += (stateInfo) => { OnInput(stateInfo, SkillInputType.LongPress); };
        }

        private void OnInput(SkillInputStateInfo stateInfo, SkillInputType inputType)
        {
            //可能会定义一些技能之外的响应,但是这里只处理技能的
            if (CurentSkillController == null)
                return;


            CurentSkillController.InputKey(stateInfo, inputType);
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
