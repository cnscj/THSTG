using System;

namespace THGame
{
    public class SkillCdCacheData
    {
        public string key;
        public int maxTimes;
        public float maxCd;
        public Action callback;
        public bool autoRemove;

        public float timeStamp;
        public int usedTimes;
    }
}