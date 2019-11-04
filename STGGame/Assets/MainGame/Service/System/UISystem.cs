using System.Collections.Generic;
using ASGame;

namespace STGService
{
    public static class UISystem
    {
        private static Dictionary<string, UIManager.PackageSettingInfo> m_packageSettingMap = new Dictionary<string, UIManager.PackageSettingInfo>();
        private static Dictionary<string, UIManager.ViewSettingInfo> m_viewSettingMap = new Dictionary<string, UIManager.ViewSettingInfo>();

        public static void InitAwake()
        {
            //设置Package的加载器
            PackageManager.GetInstance().residentTimeS = UIManager.GetInstance().residentTimeS;
            PackageManager.GetInstance().SetLoader((packageName) =>
            {
                var pair = AssetManager.GetInstance().LoadUI(packageName);
                PackageLoadMode mode = PackageLoadMode.PathString;
                if (pair.Key == (int)ResourceLoadMode.Editor) mode = PackageLoadMode.PathString;
                else if (pair.Key == (int)ResourceLoadMode.AssetBundler) mode = PackageLoadMode.AssetBundlePair;

                return new KeyValuePair<PackageLoadMode, System.Object>(mode, pair.Value);
            });

            PackageManager.GetInstance().OnAdded((packageInfo) =>
            {
                UIManager.PackageSettingInfo settingInfo = null;
                if (m_packageSettingMap.TryGetValue(packageInfo.package.name, out settingInfo))
                {
                    packageInfo.residentTimeS = settingInfo.residentTimeS;
                }
            });

            ViewManager.GetInstance().OnCreated((viewInfo) =>
            {
                UIManager.ViewSettingInfo settingInfo = null;
                if (m_viewSettingMap.TryGetValue(viewInfo.view.GetType().ToString(), out settingInfo))
                {
                    viewInfo.isPerpetual = settingInfo.isPerpetual;
                    viewInfo.isResident = settingInfo.isResident;
                }
            });

            foreach (var settingInfo in UIManager.GetInstance().packageList)
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

            foreach (var settingInfo in UIManager.GetInstance().viewList)
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
            foreach (var settingInfo in UIManager.GetInstance().viewList)
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

