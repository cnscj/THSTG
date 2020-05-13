using System;
using System.IO;
using UnityEngine;
using XLibrary.Package;
using XLua;
using static XLua.LuaEnv;

namespace SEGame
{
    public class LuaEngine : MonoSingleton<LuaEngine>
    {
        public float intervalGC = 120f;                             //GC间隔

        public string scriptPath = "../../Game/Script/Client";      //lua脚本路径
        public string mainDostring = "require 'Main'";

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

            m_luaEnv.DoString(OnMainString());
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

        /////////////////////////

        /// <summary>
        /// 添加加载器
        /// </summary>
        public void AddLoadListener()
        {

        }

        /// <summary>
        /// 添加更新器
        /// </summary>
        public void AddUpdateListener()
        {

        }

        /// <summary>
        /// 添加更新器
        /// </summary>
        public void AddFixUpdateListener()
        {

        }

        /////////////////////////
        private string GetFullPath(string fileName)
        {
            return Path.Combine(scriptPath, string.Format("{0}.lua", fileName));
        }

        private string OnMainString()
        {
            return mainDostring;
        }

        private byte[] OnLoader(ref string fileName)
        {
            try
            {
                string filePath = GetFullPath(fileName);
                if (!string.IsNullOrEmpty(filePath))
                {
                    return File.ReadAllBytes(filePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return null;
        }

    }

}

