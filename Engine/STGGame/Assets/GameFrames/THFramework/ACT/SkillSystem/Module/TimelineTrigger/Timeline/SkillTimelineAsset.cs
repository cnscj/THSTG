﻿
namespace THGame
{
    [System.Serializable]
    public class SkillTimelineAsset
    {
        public string name;
        public string type;
        public string[] args;

        public double startTime = 0f;
        public double durationTime = -1f;

    }
}
