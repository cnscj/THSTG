using System;
using XLua;
using UnityEngine;
namespace SEGame
{
    public class LuaFixedUpdateAssistant : LuaAssistantBase
    {

        void FixedUpdate()
        {
            if (luaBehaviour != null && luaBehaviour.LuaInstance != null)
            {
                assistFunc?.Invoke(luaBehaviour.LuaInstance);
            }
        }


    }
}