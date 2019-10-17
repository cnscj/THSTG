
using System.Collections.Generic;
using ASGame;
using FairyGUI;
using UnityEngine;
using XLibrary.Package;

namespace STGGame
{
    //场景管理器
    public class UIManager : MonoSingleton<UIManager>
    {
        [System.Serializable]
        public class PackageSettingInfo
        {
            public string packageName;
            public int residentTimeS = -1;
            public bool isPlayLoad;
        }

        ////////
        private class ComponentInfo
        {
            public string packageName;


        }
        private class PackageInfo
        {
            public string packageName;
            public int residentTimeS = -1;

            public float accessTimestamp;

            Dictionary<string, ComponentInfo> componentsInfo;

        }

        [Header("普通包常驻时间(s)")]
        public int residentTimeS = 120;
        [Header("包设置")]
        public List<PackageSettingInfo> packageList = new List<PackageSettingInfo>();   //常驻包
        private Dictionary<string, PackageSettingInfo> m_settingMap = new Dictionary<string, PackageSettingInfo>();


        private Dictionary<string, PackageInfo> m_packageMap = new Dictionary<string, PackageInfo>();
        private Dictionary<string, ComponentInfo> m_componentMap = new Dictionary<string, ComponentInfo>();

        private void Awake()
        {
            foreach(var info in packageList)
            {
                if (!string.IsNullOrEmpty(info.packageName))
                {
                    if (!m_settingMap.ContainsKey(info.packageName))
                    {
                        m_settingMap.Add(info.packageName, info);
                        if (info.isPlayLoad)
                        {
                            AddPackage(info.packageName);
                        }
                    }
                }
            }
        }

        private bool AddPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return false;

            if (m_packageMap.ContainsKey(packageName))
                return true;

            //加载进缓冲
            UIPackage package = null;
            var pair = AssetManager.GetInstance().LoadUI(packageName);
            if (pair.Key == (int)ResourceLoadMode.Editor)
            {
                string path = pair.Value as string;
                package = UIPackage.AddPackage(string.Format("{0}", path));
            }
            else if (pair.Key == (int)ResourceLoadMode.AssetBundler)
            {
                KeyValuePair<AssetBundle, AssetBundle> abPair = (KeyValuePair<AssetBundle, AssetBundle>)pair.Value;
                package = UIPackage.AddPackage(abPair.Key, abPair.Value);
            }

            if (package == null)
                return false;

            PackageSettingInfo settingInfo = m_settingMap[packageName];
            PackageInfo packageInfo = new PackageInfo();

            packageInfo.packageName = packageName;
            packageInfo.residentTimeS = (settingInfo != null) ? settingInfo.residentTimeS : residentTimeS;
            packageInfo.accessTimestamp = Time.realtimeSinceStartup;
            m_packageMap.Add(packageName, packageInfo);

            return true;
        }

        private void RemovePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return;

            if (!m_packageMap.ContainsKey(packageName))
                return;

            UIPackage.RemovePackage(packageName);

            m_packageMap.Remove(packageName);
        }

        public void OpenView(string packageName, string componentName)
        {
            //没有包就先加载包
            if (!m_packageMap.ContainsKey(packageName))
                AddPackage(packageName);

            if (m_packageMap.ContainsKey(packageName))
            {
                string key = string.Format("{0}_{1}", packageName, componentName);
                if (m_componentMap.ContainsKey(key))
                {


                }
                else
                {


                }
            }
            
        }

        public void CloseView(string packageName, string conponentName)
        {
            //送回缓冲
        }

        public bool IsOpenedView(string packageName, string conponentName)
        {
            return false;
        }
    }
}
