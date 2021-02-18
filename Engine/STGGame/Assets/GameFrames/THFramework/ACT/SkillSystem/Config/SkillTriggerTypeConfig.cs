using System;
using System.Collections.Generic;
using THGame;

namespace THEditor
{
    public static class SkillTriggerTypeConfig
    {
        public class ItemCfg
        {
            public string titleText;
            public List<string> argsText;
        }

        private static Dictionary<int, ItemCfg> _descToInspectDict = new Dictionary<int, ItemCfg>()
        {
            [(int)SkillTriggerType.PlayAnimation] = new ItemCfg()
            {
                titleText = "播放动画",
                argsText = new List<string>()
                {
                    "StateName:",
                },
            },
        };

        //////
       
        public static string GetTriggerName(SkillTriggerType triggerType)
        {
            var triggerStr = Enum.GetName(typeof(SkillTriggerType), triggerType);
            if (_descToInspectDict.TryGetValue((int)triggerType,out var itemCfg))
            {
                triggerStr = itemCfg.titleText;
            }

            return triggerStr;
        }
    }
}

