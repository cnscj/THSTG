using System;
using System.Collections.Generic;
using System.IO;
using XLibrary.Package;
using XLua;
using static XLua.LuaEnv;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;

/*
    Lua回调如果在C#中注册,最好只只留一个入口
    因为回调注销必须确保两边都注销掉,不如只有C#单方面持有回调会报错的(先注销C#
*/
namespace SEGame
{
    public class LuaEngine : Singleton<LuaEngine>
    {
        private static readonly Dictionary<string, LuaCSFunction> DEFAULT_INITER = new Dictionary<string, LuaCSFunction>
        {
            //TODO:还没编译json库
            { "cjson", XLua.LuaDLL.Lua.LoadCJson },
            { "lsocket", XLua.LuaDLL.Lua.LoadLsocket },
        };

        private string m_scriptEditorPath = "UnityLua/";
        private string m_scriptPath = "Assets/GameFrames/ScriptEngine/Core/LuaBehaviour/Resources/UnityLua/";
        private LuaEnv m_luaEnv;
        private IntPtr m_luaCache = IntPtr.Zero;
        private CustomLoader m_customLoader = null;

        public LuaEnv LuaEnv
        {
            get { return m_luaEnv; }
        }

        public string LuaPath
        {
            get { return m_scriptPath; }
            set { m_scriptPath = value; }
        }

        public CustomLoader BaseLoader
        {
            get { return OnLoaderInvoke; }
            set { m_customLoader = value; }
        }

        public LuaEngine()
        {
            Startup();
        }

        public void Startup()
        {
            Clear();

            m_luaEnv = new LuaEnv();

            foreach(var pairs in DEFAULT_INITER)
            {
                m_luaEnv.AddBuildin(pairs.Key, pairs.Value);
            }

            m_luaEnv.AddLoader(OnLoaderInvoke);
        }

        public void Restart()
        {
            GC();
            Startup();
        }

        public void Clear()
        {
            if (m_luaEnv != null)
            {
                m_luaEnv.Tick();

                m_luaEnv.Dispose();
                m_luaEnv = null;
            }

            if (m_luaCache != IntPtr.Zero)
            {
                m_luaCache = IntPtr.Zero;
            }
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


        public string GetFullLuaPath(string relaFilePath)
        {
            return relaFilePath;
        }

        public string GetRelaLuaPath(string fullFilePath)
        {
            return fullFilePath;
        }

        public string NormalizeLuaPath(string filePath)
        {
            string retPath = filePath;
            if (!retPath.EndsWith(".lua", StringComparison.Ordinal))
            {
                retPath = retPath.Replace('.', '/');
                retPath += ".lua";
            }
            else
            {
                retPath = retPath.Replace('.', '/').Replace("/lua", ".lua");
            }

            //抽象路径具现化,区分来自不同的资源目录
            if (retPath.IndexOf("{Resource}") >= 0)
            {
                //TextAsset不支持以lua结尾文件(只能以bytes结尾),且加载时不需要后缀
                retPath = retPath.Replace("{Resource}", m_scriptEditorPath);
                retPath = string.Format("{0}/{1}", Path.GetDirectoryName(retPath), Path.GetFileNameWithoutExtension(retPath));
            }
            else
            {
                retPath = retPath.Replace("{LuaPath}", LuaPath);
            }

            return retPath;
        }

        //默认加载委托
        private byte[] OnLoaderInvoke(ref string relaFilePath)
        {
            if (m_customLoader != null)
            {
                return m_customLoader.Invoke(ref relaFilePath);
            }
            else
            {
                try
                {
                    string finalFilePath = NormalizeLuaPath(relaFilePath);

                    if (!string.IsNullOrEmpty(finalFilePath))
                    {
                        if (relaFilePath.IndexOf("{Resource}") >= 0)
                        {
                            var fileText = UnityEngine.Resources.Load<UnityEngine.TextAsset>(finalFilePath);
                            return fileText?.bytes;
                        }
                        else
                        {
                            return File.ReadAllBytes(finalFilePath);
                        }
                    }
                }
                catch (Exception) { }
            }
            return null;
            
        }

        //////////////////////

    }

}

