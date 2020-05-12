using System;
using UnityEngine;
using XLibrary.Package;
using XLua;
using static XLua.LuaEnv;

namespace SEGame
{
    public class LuaEngine : MonoSingleton<LuaEngine>
    {
        public float intervalGC = 120f;     //GC间隔
        public string scriptPath = "";      //lua脚本路径

        private LuaEnv m_luaEnv;
        private IntPtr m_luaCache = IntPtr.Zero;

        internal LuaEnv LuaEnv
        {
            get { return m_luaEnv; }
        }

        public LuaEngine()
        {

        }

        ~LuaEngine()
        {

        }

        public void Start()
        {
            Launch();
        }

        public void Launch()
        {
            Clear();

            m_luaEnv = new LuaEnv();

            m_luaEnv.AddLoader(OnLoader);

            m_luaEnv.DoString(@"
                require 'Agent'
                require 'Main'
            ");
        }

        public void Restart()
        {
            GC();

            if (m_luaEnv != null)
            {
                m_luaEnv.Tick();

                m_luaEnv.Dispose();
                m_luaEnv = null;
            }

            Launch();
        }

        public void Clear()
        {
            if (m_luaCache != IntPtr.Zero)
            {
                m_luaCache = IntPtr.Zero;
            }

            Debug.Log("LuaEngine.Clear()");
        }

        public void GC()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            if (m_luaEnv != null)
                m_luaEnv.GC();

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
        }


        private byte[] OnLoader(ref string fileName)
        {
            Debug.Log(fileName);
            return null;
        }

    }

}

