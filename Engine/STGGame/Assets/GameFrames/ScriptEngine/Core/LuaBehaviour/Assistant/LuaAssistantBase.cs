using System;
using UnityEngine;
using XLua;

namespace SEGame
{
    public abstract class LuaAssistantBase : MonoBehaviour
    {
        public LuaBehaviour luaBehaviour;
        public Action<LuaTable> assistFunc;
        void OnDestroy()
        {
            luaBehaviour = null;
            assistFunc = null;
        }
    }
}