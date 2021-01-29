using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class SkillTriggerManager : MonoSingleton<SkillTriggerManager>
    {
        private Dictionary<string, ISkillTriggerFactory> _factoryDict;
        public void RegisterFactory(string funcName, ISkillTriggerFactory factoryInstance)
        {
            var dict = GetFactoryDict();
            dict[funcName] = factoryInstance;
        }

        public ISkillTriggerFactory GetFactory(string funcName)
        {
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

