using System;
using System.Collections.Generic;

namespace THEditor
{
    public static class SpriteConfig
    {
        private class GlobalConfigInfos
        {
            public Dictionary<string, bool> stateLoopMap;
            public Dictionary<string, bool> defaultStateMap;
        }

        private static GlobalConfigInfos s_global = new GlobalConfigInfos
        {
            stateLoopMap = new Dictionary<string, bool>
            {
                { "idle" , true},
                { "stand" , true},
                { "run", true },

                { "ridestand" , true},
                { "riderun", true },
            },
            defaultStateMap = new Dictionary<string, bool>
            {
                //因类型不同而不同,这里默认stand,不能多个,否则会冲突
                { "stand" , true},
                { "idle" , true},
            },
        };


        public static bool isDefaultState(string stateName)
        {
            return s_global.defaultStateMap.ContainsKey(stateName);
        }

        public static bool IsNeedLoop(string stateName)
        {
            if (s_global.stateLoopMap.ContainsKey(stateName))
            {
                return true;
            }
            return false;
        }
    }
}