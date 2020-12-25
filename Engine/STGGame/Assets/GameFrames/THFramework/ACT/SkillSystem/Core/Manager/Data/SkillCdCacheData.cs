
using System;

namespace THGame
{
    public class SkillCdCacheData
    {
        public IComparable key;
        public int maxTimes;
        public float maxCd;

        public float timeStamp;
        public int usedTimes;
    }
}