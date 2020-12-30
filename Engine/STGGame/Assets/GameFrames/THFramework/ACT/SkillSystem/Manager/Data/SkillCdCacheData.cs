
using System;

namespace THGame
{
    //这里的最大CD可能要根据实际情况发生变动
    public class SkillCdCacheData
    {
        public IComparable key;
        public int maxTimes;
        public float maxCd;     

        public float timeStamp;
        public int usedTimes;
    }
}