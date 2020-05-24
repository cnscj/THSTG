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
        private static readonly string DEFAULT_DOSTRING = "require 'Main'"; //执行代码
        public string scriptSPath = "../../Game/Script/Client";             //首包脚本路径
        public string scriptUPath = "";                                   //更新脚本路径
        public string trunkRoot = "";                                     //主干目录
        public string branchRoot = "";                                    //更新目录

        
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

            //TODO:还没编译json库
            //m_luaEnv.AddBuildin("cjson", XLua.LuaDLL.Lua.LoadCJson);
            //m_luaEnv.AddBuildin("lsocket", XLua.LuaDLL.Lua.LoadLsocket);


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

        public void SetLuaTrunkAndBrench(string trunk, string brench)
        {
            trunkRoot = trunk;
            branchRoot = brench;
        }

        /////////////////////////

        private CustomLoader OnCustomLoader()
        {
            m_customLoader = m_customLoader ?? OnLoaderInvoke;
            return m_customLoader;
        }

        private string OnMainString()
        {
            return DEFAULT_DOSTRING;
        }

        private string GetFullPath(string parentRoot, string filePath)
        {
            //将"\"转为"/"
            //将"."转为"/"
            var scrPath = filePath.Replace(".", "/").Replace("\\", "/");
            var fullPath = scrPath;
            if (!string.IsNullOrEmpty(parentRoot))
            {
                fullPath = Path.Combine(parentRoot, string.Format("{0}.lua", scrPath));
            }

            return fullPath;
        }

        //TODO:注入调试方法:U9,如果在特定目录有这个脚本,优先用这个脚本,否则用原来的(先加载luaU,在加载先加载luaS
        //不能通过IO方式去判断,效率低
        //内置库不能通过路劲去搞,还得判断内置库
        private string GetScriptPath(string filePath)
        {
            string sPath = GetFullPath(scriptSPath, filePath);
            string uPath = GetFullPath(scriptUPath, filePath);
            if (File.Exists(uPath))
            {
                return uPath;
            }
            else
            {
                string tRoot = Path.Combine(scriptSPath, trunkRoot);
                string bRoot = Path.Combine(scriptSPath, branchRoot);
                string tPath = GetFullPath(tRoot, filePath);
                string bPath = GetFullPath(bRoot, filePath);

                if (File.Exists(bPath))
                {
                    return bPath;
                }
                else if(File.Exists(tPath))
                {
                    return tPath;
                }
                else if (File.Exists(sPath))
                {
                    return sPath;
                }
            }

            return GetFullPath(null,filePath);
        }

        private byte[] OnLoaderInvoke(ref string fileRelaPath)
        {
            try
            {
                string fileFullPath = GetScriptPath(fileRelaPath);
                if (!string.IsNullOrEmpty(fileFullPath))
                {
                    return File.ReadAllBytes(fileFullPath);
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

        //////////////////////

    }

}

