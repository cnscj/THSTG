using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class SkillTriggerManager : MonoSingleton<SkillTriggerManager>
    {
        private Dictionary<string, ISkillTriggerFactory> _factoryDict;
        public void RegisterFactory<T>(string funcName) where T : AbstractSkillTrigger,new()
        {
            var dict = GetFactoryDict();
            var factoryInstance = new SkillTriggerFactory<T>();
            dict[funcName] = factoryInstance;
            factoryInstance.Type = funcName;
        }

        public ISkillTriggerFactory GetFactory(string funcName)
        {
            if (_factoryDict == null || _factoryDict.Count <= 0)
                return default;

            _factoryDict.TryGetValue(funcName, out var factory);
            return factory;
        }

        public AbstractSkillTrigger GenerateTrigger(string command)
        {
            if (string.IsNullOrEmpty(command))
                return default;

            var newCommand = command.Trim();

            AbstractSkillTrigger skillTrigger = default;
            int leftBracketIndex = newCommand.IndexOf("(");
            if (leftBracketIndex >= 0)
            {
                int rightBracketIndex = newCommand.IndexOf(")");
                if (rightBracketIndex >= 0)
                {
                    string commandFuncType = newCommand.Substring(0, leftBracketIndex);
                    var factory = GetFactory(commandFuncType);
                    if (factory != null)
                    {
                        string argsString = newCommand.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1);
                        string[] argsArray = argsString.Split(',');

                        skillTrigger = factory.CreateTrigger();
                        skillTrigger.Parse(argsArray);
                    }
                }
            }
            return skillTrigger;
        }

        public string GenerateCommand(AbstractSkillTrigger trigger)
        {
            string command = "";

            if (trigger == null)
                return command;

            string argsStr = "";
            if (trigger.Args != null && trigger.Args.Length > 0)
            {
                foreach (var arg in trigger.Args)
                {
                    argsStr += arg;
                    argsStr += ",";
                }
                argsStr = argsStr.Substring(0, argsStr.Length - 1);
            }
            command = string.Format("{0}({1})", trigger.Type, argsStr);

            return command;
        }

        private Dictionary<string, ISkillTriggerFactory> GetFactoryDict()
        {
            _factoryDict = _factoryDict ?? new Dictionary<string, ISkillTriggerFactory>();
            return _factoryDict;
        }

    }

}

