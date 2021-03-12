
using System;
using System.Collections.Generic;

namespace THGame
{
    public static class SkillTriggerUtil
    {
        //下面函数负责各种序列化与反序列化

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

            string infoStr;
            infoStr = string.Format("{0},{1}", trigger.startTime, trigger.durationTime);

            string argsStr = "";
            if (trigger.args != null && trigger.args.Length > 0)
            {
                foreach (var arg in trigger.args)
                {
                    argsStr += arg;
                    argsStr += ",";
                }
                argsStr = argsStr.Substring(0, argsStr.Length - 1);
            }
            command = string.Format("{0}[{1}]({2})", trigger.type, infoStr, argsStr);

            return command;
        }

        public static SkillTimelineSequence[] LoadSequenceFromData(SkillTimelineData data)
        {
            if (data == null)
                return default;

            var playableList = new List<SkillTimelineSequence>();
            foreach(var playableData in data.sequences)
            {
                var rootTrigger = AbstractSkillTrigger.Create(playableData) ?? playableData;
                var breadthFirstQueue = new Queue<Tuple<SkillTimelineSequence, SkillTimelineSequence>>();
                breadthFirstQueue.Enqueue(new Tuple<SkillTimelineSequence, SkillTimelineSequence>(playableData, rootTrigger));

                while (breadthFirstQueue.Count > 0)
                {
                    var nodeTuple = breadthFirstQueue.Dequeue();

                    if (nodeTuple.Item1.sequences != null && nodeTuple.Item1.sequences.Length > 0)
                    {
                        foreach(var childSequenceData in nodeTuple.Item1.sequences)
                        {
                            var childTrigger = AbstractSkillTrigger.Create(childSequenceData) ?? childSequenceData;
                            nodeTuple.Item2.AddSequence(childTrigger);

                            breadthFirstQueue.Enqueue(new Tuple<SkillTimelineSequence, SkillTimelineSequence>(childSequenceData, childTrigger));
                        }
                    }
                }
                playableList.Add(rootTrigger);
            }

            return playableList.ToArray();
        }

        public static void SaveDataToFile(SkillTimelineData skillTimelineData, string savePath)
        {
            if (skillTimelineData == null)
                return;

            if (string.IsNullOrEmpty(savePath))
                return;

            SkillTimelineData.SaveToFile(skillTimelineData, savePath);
        }

        public static void SaveSequenceToFile(SkillTimelineSequence[] skillTimelineSequences, string savePath)
        {
            if (skillTimelineSequences == null)
                return;

            if (string.IsNullOrEmpty(savePath))
                return;

            var data = new SkillTimelineData();
            data.sequences = skillTimelineSequences;

            SaveDataToFile(data, savePath);
        }

        public static SkillTimelineSequence[] LoadSequenceFromFile(string loadPath)
        {
            if (string.IsNullOrEmpty(loadPath))
                return default;

            var data = SkillTimelineData.LoadFromFile(loadPath);
            return LoadSequenceFromData(data);
        }

        

    }

}
