using System;
using System.Collections.Generic;
using System.Reflection;
using XLibrary.Package;

namespace THGame
{
    public class SkillTriggerManager : Singleton<SkillTriggerManager>
    {
        private Dictionary<string, ISkillTriggerFactory> _factoryDict;

        public SkillTriggerManager()
        {
            
        }

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

        public AbstractSkillTrigger GenerateTrigger(string command)
        {
            if (string.IsNullOrEmpty(command))
                return default;

            var newCommand = command.Trim();

            AbstractSkillTrigger skillTrigger = default;

            string funcName = null;
            string[] clipInfo = null;
            string[] clipArgs = null;

            int funcEndIndex = newCommand.IndexOf("[");
            if (funcEndIndex < 0) newCommand.IndexOf("(");

            if (funcEndIndex >= 0)
            {
                funcName = newCommand.Substring(0, funcEndIndex);
            }

            int leftSmallBracketIndex = newCommand.IndexOf("[");
            if (leftSmallBracketIndex >= 0)
            {
                int rightSmallBracketIndex = newCommand.IndexOf("]");
                if (rightSmallBracketIndex >= 0)
                {
                    string clipInfoStr = newCommand.Substring(leftSmallBracketIndex + 1, rightSmallBracketIndex - leftSmallBracketIndex - 1);
                    clipInfo = clipInfoStr.Split(',');
                }
            }

            int leftMiddleBracketIndex = newCommand.IndexOf("(");
            if (leftMiddleBracketIndex >= 0)
            {
                int rightMiddleBracketIndex = newCommand.IndexOf(")");
                if (rightMiddleBracketIndex >= 0)
                {
                    string clipArgsStr = newCommand.Substring(leftMiddleBracketIndex + 1, rightMiddleBracketIndex - leftMiddleBracketIndex - 1);
                    clipArgs = clipArgsStr.Split(',');
                }
            }

            if (!string.IsNullOrEmpty(funcName))
            {
                var factory = GetFactory(funcName);
                if (factory != null)
                {
                    skillTrigger = factory.CreateTrigger();
                    skillTrigger.Parse(clipInfo,clipArgs);
                }

            }

            return skillTrigger;
        }

        public string GenerateCommand(AbstractSkillTrigger trigger)
        {
            string command = "";

            if (trigger == null)
                return command;

            string infoStr = "";
            infoStr = string.Format("{0},{1}", trigger.StartTime, trigger.DurationTime);

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
            command = string.Format("{0}[{1}]({2})", trigger.Type, infoStr, argsStr);

            return command;
        }

        private Dictionary<string, ISkillTriggerFactory> GetFactoryDict()
        {
            _factoryDict = _factoryDict ?? new Dictionary<string, ISkillTriggerFactory>();
            return _factoryDict;
        }
    }
}

