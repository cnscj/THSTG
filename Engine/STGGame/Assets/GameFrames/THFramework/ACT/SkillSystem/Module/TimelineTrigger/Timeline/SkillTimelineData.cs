
using UnityEngine;
using XLibrary;

namespace THGame
{
    [System.Serializable]
    public class SkillTimelineData
    {
        public string version = "1.0";
        public SkillTimelineSequence[] sequences;

        public static SkillTimelineData Create(string content)
        {
            var data = JsonUtility.FromJson<SkillTimelineData>(content);
            return data;
        }

        public static SkillTimelineData LoadFromFile(string filePath)
        {
            var content = XFileTools.ReadAllText(filePath);
            var data = JsonUtility.FromJson<SkillTimelineData>(content);
            return data;
        }

        public static void SaveToFile(SkillTimelineData timelineData, string filePath)
        {
            string content = JsonUtility.ToJson(timelineData,true);
            XFileTools.WriteAllText(filePath, content);
        }
    }
}
