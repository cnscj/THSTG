using UnityEngine;
using System;
using XLibrary.Package;
using Object = UnityEngine.Object;
namespace ASGame
{

    public class AssetManage : MonoSingleton<AssetManage>
    {

        [Header("资源加载")]
        public string loadMode = "";

        [Header("资源下载")]
        public string downloadServerUrl = "";
        public float downloadOutTime = 10f;

        [Header("资源缓存")]
        public float cacheClearCDTimer = 30f;

        [Header("资源更新")]
        public string updateServerUrl = "";

        private AssetBaseDebugger m_debugger;

        public void LoadAsset(string path, Action<Object> onSuccess)
        {
            //先从Cache中查找
            //然后到本地查找
            //酒后在网络下载
        }

        public void LoadAsset<T>(string path, Action<T> onSuccess)
        {

        }


        public AssetBaseDebugger GetOrCreateDebugger()
        {
            if (m_debugger == null)
            {
#if UNITY_EDITOR
                m_debugger = AssetBaseDebugger.CreateDebugger<AssetInstanceDebugger>(transform);
#else
                m_debugger = AssetBaseDebugger.CreateDebugger<AssetNullDebugger>(transform);
#endif
            }

            return m_debugger;
        }
    }
}
