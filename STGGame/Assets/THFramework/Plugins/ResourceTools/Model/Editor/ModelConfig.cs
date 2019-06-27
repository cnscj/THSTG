using System;
using System.Collections.Generic;
using UnityEngine;
namespace THEditor
{
    public static class ModelConfig
    {
        private class GlobalConfigInfos
        {
            public Dictionary<string, bool> stateLoopMap;
            public Dictionary<string, bool> defaultStateMap;
        }

        private class ConfigInfos
        {
            public Dictionary<string, bool> stateLoopMap;
            public bool isGenColliderBox;
            public string defaultState;
        }
        private static GlobalConfigInfos s_global = new GlobalConfigInfos
        {
            stateLoopMap = new Dictionary<string, bool>
            {
                { "stand" , true},
                { "run", true },

                { "ridestand" , true},
                { "riderun", true },
            },
            defaultStateMap = new Dictionary<string, bool>
            {
                //因类型不同而不同,这里默认stand,不能多个,否则会冲突
                { "stand" , true},
                { "ridestand" , true},
            },
        };
        private static Dictionary<string, ConfigInfos> s_unique = new Dictionary<string, ConfigInfos>
        {
            {
                "Role",
                new ConfigInfos
                {
                    stateLoopMap = new Dictionary<string, bool>
                    {

                    },
                    isGenColliderBox = true,
                    defaultState = "stand",
                }
            },
            {
                "Pet",
                new ConfigInfos
                {
                    stateLoopMap = new Dictionary<string, bool>
                    {

                    },
                    isGenColliderBox = true,
                    defaultState = "stand",
                }
            },
            {
                "Ride",
                new ConfigInfos
                {
                    stateLoopMap = new Dictionary<string, bool>
                    {

                    },
                    isGenColliderBox = true,
                    defaultState = "ridestand",             //坐骑就ridestan默认
                }
            },
            {
                "Npc",
                new ConfigInfos
                {
                    stateLoopMap = new Dictionary<string, bool>
                    {

                    },
                    isGenColliderBox = true,
                    defaultState = "stand",
                }
            },
            {
                "Mob",
                new ConfigInfos
                {
                    stateLoopMap = new Dictionary<string, bool>
                    {

                    },
                    isGenColliderBox = true,
                    defaultState = "stand",
                }
            },
            {
                "Boss",
                new ConfigInfos
                {
                    stateLoopMap = new Dictionary<string, bool>
                    {

                    },
                    isGenColliderBox = true,
                    defaultState = "stand",
                }
            },
        };

        /////

        public static bool IsNeedLoop(string folder, string stateName)
        {
            if (s_unique.ContainsKey(folder))
            {
                var map = s_unique[folder];
                if (map.stateLoopMap.ContainsKey(stateName))
                {
                    return true;
                }
            }
            if(s_global.stateLoopMap.ContainsKey(stateName))
            {
                return true;
            }
            return false;
        }

        public static bool isNeedCollider(string folder)
        {
            if (s_unique.ContainsKey(folder))
            {
                var info = s_unique[folder];
                return info.isGenColliderBox;
            }
            return false;
        }

        public static bool isDefaultState(string folder, string stateName)
        {
            if (s_unique.ContainsKey(folder))
            {
                var info = s_unique[folder];
                return (stateName.Equals(info.defaultState));
            }
            return s_global.defaultStateMap.ContainsKey(stateName);
        }

        public static string GetDefaultState(string folder)
        {
            if (s_unique.ContainsKey(folder))
            {
                var info = s_unique[folder];
                return info.defaultState == null ? info.defaultState : "";
            }
            return "";
        }
    }
}