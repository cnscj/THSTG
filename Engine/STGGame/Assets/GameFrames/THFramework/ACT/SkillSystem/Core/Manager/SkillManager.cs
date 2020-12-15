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
        private SkillFSMMachine _skillStateMachine;         //状态机
        private SkillEventDispatcher _skillDispatcher;      //派发器

        private SkillData _curSkillData;                    //当前使用的技能数据

        public SkillInputReceiver GetInputReceiver() { return _skillInputReceiver = _skillInputReceiver ?? CreateManager<SkillInputReceiver>("InputReceiver"); }
        public SkillCdCache GetCdCache(){ return _skillCdCache = _skillCdCache ?? CreateManager<SkillCdCache>("CountdownCache"); }
        public SkillCastTrigger GetCastTrigger() { return _skillCastTrigger = _skillCastTrigger ?? CreateManager<SkillCastTrigger>("CastTrigger"); }
        public SkillFSMMachine GetStateMachine() { return _skillStateMachine = _skillStateMachine ?? new SkillFSMMachine(); }
        public SkillEventDispatcher GetEventDispatcher() { return _skillDispatcher = _skillDispatcher ?? new SkillEventDispatcher(); }


        private T CreateManager<T>(string name) where T : MonoBehaviour
        {
            GameObject managerGObj = new GameObject(name);
            managerGObj.transform.SetParent(transform);
            T manager = managerGObj.AddComponent<T>();

            return manager;
        }


        
    }
}
