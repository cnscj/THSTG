using System;
using XLua;
namespace SEGame
{
    public class LuaLateUpdateAssistant : LuaAssistantBase
    {
        void LateUpdate()
        {
            if (luaBehaviour != null && luaBehaviour.LuaInstance != null)
            {
                assistFunc?.Invoke(luaBehaviour.LuaInstance);
            }
        }
    }
}