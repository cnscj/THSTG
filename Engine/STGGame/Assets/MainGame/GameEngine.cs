using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace STGGame
{
    public class GameEngine : MonoSingleton<GameEngine>
    {
        public static readonly string ENGINE_NAME = "TohoEngine";
        public static readonly int ENGINE_VERSION_NUM = 100;
        public static readonly string ENGINE_APP_VERSION = "1.0";

        public string GetEngineName()
        {
            return ENGINE_NAME;
        }

        public int GetEngineVersion()
        {
            return ENGINE_VERSION_NUM;
        }

        public string GetVersion()
        {
            return ENGINE_APP_VERSION;
        }
    }

}
