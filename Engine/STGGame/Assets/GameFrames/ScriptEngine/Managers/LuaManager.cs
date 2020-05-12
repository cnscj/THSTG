using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace SEGame
{
    public class LuaManager : Singleton<LuaManager>
    {
        public float intervalGC = 120f;     //GC间隔

        private LuaEngine m_engine;         //Lua引擎

    }
}

