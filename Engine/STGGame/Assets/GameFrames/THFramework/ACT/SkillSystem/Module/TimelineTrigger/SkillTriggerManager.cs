using System.Collections.Generic;
using XLibrary.Package;

namespace THGame
{
    public class SkillTriggerManager : Singleton<SkillTriggerManager>
    {
        private Dictionary<string, ISkillTriggerFactory> _factoryDict;

        public SkillTriggerManager()
        {
#if UNITY_EDITOR
            RegisterFactory<SkillPrintTrigger>("Print");
            RegisterFactory<SkillPlayEffectTrigger>("PlayEffect");
#endif
        }

        //NOTE:这里无法通过发射生成实例,估无法使用特性
        public void RegisterFactory<T>(string funcName) where T : AbstractSkillTrigger, new()
        {
            var factoryInstance = new SkillTriggerFactory<T>();
            RegisterFactory(funcName, factoryInstance);
        }

        public void RegisterFactory(string funcName, ISkillTriggerFactory factoryInstance)
        {
            if (string.IsNullOrEmpty(funcName))
                return;

            if (factoryInstance == null)
                return;

            var dict = GetFactoryDict();
            dict[funcName] = factoryInstance;
            factoryInstance.Type = funcName;
        }

        public ISkillTriggerFactory GetFactory(string funcName)
        {
            if (string.IsNullOrEmpty(funcName))
                return default;

            if (_factoryDict == null || _factoryDict.Count <= 0)
                return default;

            _factoryDict.TryGetValue(funcName, out var factory);
            return factory;
        }

       
        private Dictionary<string, ISkillTriggerFactory> GetFactoryDict()
        {
            _factoryDict = _factoryDict ?? new Dictionary<string, ISkillTriggerFactory>();
            return _factoryDict;
        }
    }
}

