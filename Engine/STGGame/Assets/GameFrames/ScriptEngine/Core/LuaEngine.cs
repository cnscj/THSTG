using System;
using System.IO;
using UnityEngine;
using XLibrary.Package;
using XLua;
using static XLua.LuaEnv;

/*
    Lua回调如果在C#中注册,最好只只留一个入口
    因为回调注销必须确保两边都注销掉,不如只有C#单方面持有回调会报错的(先注销C#
*/
namespace SEGame
{
    
    public class LuaEngine : MonoSingleton<LuaEngine>
    {
        public string scriptPath = "../../Game/Script/Client";      //lua脚本路径
        public string mainDostring = "require 'Main'";

        private LuaEnv m_luaEnv;
        private IntPtr m_luaCache = IntPtr.Zero;
        private CustomLoader m_customLoader = null;

        // 给lua用的刷新回调
        Action<float> m_luaUpdateCallback;
        Action m_luaLateUpdateCallback;

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

        public void Startup()
        {
            Clear();

            m_luaEnv = new LuaEnv();

            m_luaEnv.AddLoader(OnCustomLoader());

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

            Startup();
        }

        public void Clear()
        {
            if (m_luaCache != IntPtr.Zero)
            {
                m_luaCache = IntPtr.Zero;
            }

            UnregisterLuaUpdateListeners();

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
        /// 添加Lua加载器
        /// </summary>
        public void SetCustomLoadListener(CustomLoader loader)
        {
            m_customLoader = loader;
        }

        /// <summary>
        /// 添加Lua更新器
        /// </summary>
        public void RegisterLuaUpdateListeners(Action<float> updateCallback, Action lateUpdateCallback)
        {
            m_luaUpdateCallback = updateCallback;
            m_luaLateUpdateCallback = lateUpdateCallback;
        }

        public void UnregisterLuaUpdateListeners()
        {
            m_luaUpdateCallback = null;
            m_luaLateUpdateCallback = null;
        }

        /////////////////////////

        private CustomLoader OnCustomLoader()
        {
            m_customLoader = m_customLoader ?? OnLoaderInvoke;
            return m_customLoader;
        }

        private string OnMainString()
        {
            return mainDostring;
        }

        private string GetFullPath(string fileName)
        {
            return Path.Combine(scriptPath, string.Format("{0}.lua", fileName));
        }

        private byte[] OnLoaderInvoke(ref string fileName)
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

        private void Update()
        {
            if (m_luaUpdateCallback != null)
            {
                m_luaUpdateCallback(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (m_luaLateUpdateCallback != null)
            {
                m_luaLateUpdateCallback();
            }
        }
    }

}

