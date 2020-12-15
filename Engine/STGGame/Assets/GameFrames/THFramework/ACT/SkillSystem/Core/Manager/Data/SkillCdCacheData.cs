using System;

namespace THGame
{
    public class SkillCdCacheData
    {
        public string key;
        public int maxTimes = 1;
        public float maxCd;
        public Action callback;

        public float timeStamp;
        public int usedTimes;
    }
}