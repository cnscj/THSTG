
using UnityEngine;
using XLibrary;

namespace THGame
{
    [System.Serializable]
    public class SkillTimelineData
    {
        public string version = "1.0";
        public SkillTimelineAsset[] sequence;

        public static SkillTimelineData LoadFromFile(string filePath)
        {
            var content = XFileTools.ReadAllText(filePath);
            var data = JsonUtility.FromJson<SkillTimelineData>(content);
            return data;
        }

        public static void SaveToFile(SkillTimelineData timelineData, string filePath)
        {
            string content = JsonUtility.ToJson(timelineData);
            XFileTools.WriteAllText(filePath, content);
        }
    }
}
