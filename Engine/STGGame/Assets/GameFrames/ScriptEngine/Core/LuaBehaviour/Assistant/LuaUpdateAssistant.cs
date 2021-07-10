using System;
using XLua;
namespace SEGame
{
    public class LuaUpdateAssistant : LuaAssistantBase
    {
        void Update()
        {
            if (luaBehaviour != null && luaBehaviour.LuaInstance != null)
            {
                assistFunc?.Invoke(luaBehaviour.LuaInstance);
            }
        }
    }
}