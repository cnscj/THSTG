using System.Collections.Generic;
using System.IO;
using ASGame;
using STGU3D;
using THGame;
using AssetManager = STGU3D.AssetManager;
namespace STGRuntime
{
    public static class UIMgr
    {
        [System.Serializable]
        public class PackageSettingInfo
        {
            public string packageName;
            public float residentTimeS = -1;
            public bool isPlayLoad;             //运行加载
        }

        [System.Serializable]
        public class ViewSettingInfo
        {
            public string viewName;

            public bool isResident;      //常驻View
            public bool isPerpetual;     //不可Close
            public bool isPlayLoad;      //运行加载
        }

        public static float residentTimeS = 15;
        public static List<PackageSettingInfo> packageList = new List<PackageSettingInfo>()
        {
            new PackageSettingInfo()
            {
                packageName = "UIBase",
                residentTimeS = -1,
                isPlayLoad = true,

            },
            new PackageSettingInfo()
            {
                packageName = "UIPublic",
                residentTimeS = -1,
                isPlayLoad = true,

            },

        };
        public static List<ViewSettingInfo> viewList = new List<ViewSettingInfo>()
        {
            new ViewSettingInfo()
            {
                viewName = "STGRuntime.UI.TestView",
                isResident = true,
                isPerpetual = true,
                isPlayLoad = true,
            }
        };

        //////////
        private static Dictionary<string, PackageSettingInfo> m_packageSettingMap = new Dictionary<string, PackageSettingInfo>();
        private static Dictionary<string, ViewSettingInfo> m_viewSettingMap = new Dictionary<string, ViewSettingInfo>();
        

        public static void InitAwake()
        {
            //设置Package的加载器
            PackageManager.GetInstance().residentTimeS = residentTimeS;
            PackageManager.GetInstance().SetLoader(new PackageCustomLoader((packageName) =>
            {
#if !UNITY_EDITOR
                return Path.Combine(AssetFileBook.FGUI_ROOT_SRC, string.Format("{0}", packageName));
#else
                return AssetManager.GetInstance().LoadUISync(packageName);//这里要改为异步
#endif
            }));

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

            foreach (var settingInfo in packageList)
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

        public static void InitStart()
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

