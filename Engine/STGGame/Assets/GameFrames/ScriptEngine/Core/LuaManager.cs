using System;
using System.IO;
using UnityEngine;
using XLibrary.Package;

namespace SEGame
{
    public class LuaManager : MonoSingleton<LuaManager>
    {
        public string strupDoString = "require 'Main'";                     //执行代码
        public string scriptSPath = "../../Game/Script/Client";             //首包脚本路径
        public string scriptUPath = "";                                     //更新脚本路径
        public string trunkRoot = "";                                       //主干目录
        public string branchRoot = "";                                      //更新目录

        // 给lua用的刷新回调
        Action<float> m_luaUpdateCallback;
        Action<float> m_luaLateUpdateCallback;


        public void Startup()
        {
            LuaEngine.GetInstance().BaseLoader = OnLoaderInvoke;
            LuaEngine.GetInstance().LuaEnv.DoString(strupDoString);
        }

        public void Restart()
        {
            LuaEngine.GetInstance().Restart();
        }

        public void Clear()
        {
            UnregisterLuaUpdateListeners();
            LuaEngine.GetInstance().Clear();

            Debug.Log("LuaManager.Clear()");
        }


        /////////////////////////
        /// <summary>
        /// 添加Lua更新器
        /// </summary>
        public void RegisterLuaUpdateListeners(Action<float> updateCallback, Action<float> lateUpdateCallback)
        {
            m_luaUpdateCallback = updateCallback;
            m_luaLateUpdateCallback = lateUpdateCallback;
        }

        public void UnregisterLuaUpdateListeners()
        {
            m_luaUpdateCallback = null;
            m_luaLateUpdateCallback = null;
        }

        public void SetLuaTrunkAndBranch(string trunk, string brench)
        {
            trunkRoot = trunk;
            branchRoot = brench;
        }

        //注入调试方法:U9,如果在特定目录有这个脚本,优先用这个脚本,否则用原来的(先加载luaU,在加载先加载luaS
        //XXX:不能通过IO方式去判断,效率低
        //内置库不能通过路劲去搞,还得判断内置库
        public string GetScriptPath(string filePath)
        {
            string sPath = NormalizePath(scriptSPath, filePath);
            string uPath = NormalizePath(scriptUPath, filePath);
            if (File.Exists(uPath))
            {
                return uPath;
            }
            else
            {
                string tRoot = Path.Combine(scriptSPath, trunkRoot);
                string bRoot = Path.Combine(scriptSPath, branchRoot);
                string tPath = NormalizePath(tRoot, filePath);
                string bPath = NormalizePath(bRoot, filePath);

                if (File.Exists(bPath))
                {
                    return bPath;
                }
                else if (File.Exists(tPath))
                {
                    return tPath;
                }
                else if (File.Exists(sPath))
                {
                    return sPath;
                }
            }

            return NormalizePath(null, filePath);
        }

        /////////////////////////

        private string NormalizePath(string parentRoot, string filePath)
        {
            //将"\"转为"/"
            //将"."转为"/"
            var scrPath = filePath.Replace('.', '/').Replace('\\', '/');
            string fullPath;
            if (!string.IsNullOrEmpty(parentRoot))
            {
                fullPath = Path.Combine(parentRoot, string.Format("{0}.lua", scrPath));
            }
            else
            {
                fullPath = string.Format("{0}.lua", scrPath);
            }

            return fullPath;
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
            m_luaUpdateCallback?.Invoke(Time.deltaTime);
        }

        private void LateUpdate()
        {
            m_luaLateUpdateCallback?.Invoke(Time.deltaTime);
        }

        //////////////////////

    }

}

