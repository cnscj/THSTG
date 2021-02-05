﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public static class SkillTriggerUtil
    {
        public static AbstractSkillTrigger GenerateTrigger(string command)
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
                var factory = SkillTriggerManager.GetInstance().GetFactory(funcName);
                if (factory != null)
                {
                    skillTrigger = factory.CreateTrigger();
                    skillTrigger.Initialize(clipInfo, clipArgs);
                }

            }

            return skillTrigger;
        }

        public static string GenerateCommand(AbstractSkillTrigger trigger)
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

    }

}
