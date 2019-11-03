
using System.Collections.Generic;
using ASGame;
using STGGame.UI;
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
            public float residentTimeS = -1;
            public bool isPlayLoad;
        }

        [System.Serializable]
        public class ViewSettingInfo
        {
            public string viewName;

            public bool isResident;      //常驻View
            public bool isPerpetual;     //不可Close
            public bool isPlayLoad;


        }


        [Header("Package设置")]
        public float residentTimeS = 15;
        public List<PackageSettingInfo> packageList = new List<PackageSettingInfo>();   
        private Dictionary<string, PackageSettingInfo> m_packageSettingMap = new Dictionary<string, PackageSettingInfo>();

        [Header("View设置")]
        public List<ViewSettingInfo> viewList = new List<ViewSettingInfo>();
        private Dictionary<string, ViewSettingInfo> m_viewSettingMap = new Dictionary<string, ViewSettingInfo>();

        private void Awake()
        {
            //设置Package的加载器
            PackageManager.GetInstance().residentTimeS = residentTimeS;
            PackageManager.GetInstance().SetLoader((packageName) =>
            {
                var pair = AssetManager.GetInstance().LoadUI(packageName);
                PackageLoadMode mode = PackageLoadMode.PathString;
                if (pair.Key == (int)ResourceLoadMode.Editor) mode = PackageLoadMode.PathString;
                else if (pair.Key == (int)ResourceLoadMode.AssetBundler) mode = PackageLoadMode.AssetBundlePair;

                return new KeyValuePair<PackageLoadMode,System.Object>(mode, pair.Value);
            });

            PackageManager.GetInstance().OnAdded((packageInfo) =>
            {
                PackageSettingInfo settingInfo = null;
                if (m_packageSettingMap.TryGetValue(packageInfo.package.name, out settingInfo))
                {
                    packageInfo.residentTimeS = settingInfo.residentTimeS;
                }
            });

            ViewManager.GetInstance().OnCreated((viewInfo) =>
            {
                ViewSettingInfo settingInfo = null;
                if (m_viewSettingMap.TryGetValue(viewInfo.view.GetType().ToString(), out settingInfo))
                {
                    viewInfo.isPerpetual = settingInfo.isPerpetual;
                    viewInfo.isResident = settingInfo.isResident;
                }
            });

            foreach (var settingInfo in  packageList)
            {
                if (!string.IsNullOrEmpty(settingInfo.packageName))
                {
                    if (!m_packageSettingMap.ContainsKey(settingInfo.packageName))
                    {
                        m_packageSettingMap.Add(settingInfo.packageName, settingInfo);
                        if (settingInfo.isPlayLoad)
                        {
                            PackageManager.GetInstance().AddPackage(settingInfo.packageName);
                        }
                    }
                }
            }

            foreach (var settingInfo in viewList)
            {
                if (!string.IsNullOrEmpty(settingInfo.viewName))
                {
                    if (!m_viewSettingMap.ContainsKey(settingInfo.viewName))
                    {
                        m_viewSettingMap.Add(settingInfo.viewName, settingInfo);
                    }
                }
            }
        }

        private void Start()
        {
            foreach (var settingInfo in viewList)
            {
                if (!string.IsNullOrEmpty(settingInfo.viewName))
                {
                    if (settingInfo.isPlayLoad)
                    {
                        ViewManager.GetInstance().Open(settingInfo.viewName);
                    }
                }
            }
        }
      
    }
}
