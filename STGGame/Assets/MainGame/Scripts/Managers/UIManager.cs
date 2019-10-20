
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
            public int residentTimeS = -1;
            public bool isPlayLoad;
        }

        [Header("包设置")]
        public List<PackageSettingInfo> packageList = new List<PackageSettingInfo>();   //常驻包
        private Dictionary<string, PackageSettingInfo> m_settingMap = new Dictionary<string, PackageSettingInfo>();

        private void Awake()
        {
            //设置Package的加载器
            PackageManager.GetInstance().SetLoader((packageName) =>
            {
                var pair = AssetManager.GetInstance().LoadUI(packageName);
                PackageLoadMode mode = PackageLoadMode.PathString;
                if (pair.Key == (int)ResourceLoadMode.Editor) mode = PackageLoadMode.PathString;
                else if (pair.Key == (int)ResourceLoadMode.AssetBundler) mode = PackageLoadMode.AssetBundlePair;

                return new KeyValuePair<PackageLoadMode,System.Object>(mode, pair.Value);
            });
            

            foreach(var settingInfo in packageList)
            {
                if (!string.IsNullOrEmpty(settingInfo.packageName))
                {
                    if (!m_settingMap.ContainsKey(settingInfo.packageName))
                    {
                        m_settingMap.Add(settingInfo.packageName, settingInfo);
                        if (settingInfo.isPlayLoad)
                        {
                            var packageInfo = PackageManager.GetInstance().AddPackage(settingInfo.packageName);
                            if (packageInfo != null)
                            {
                                packageInfo.residentTimeS = settingInfo.residentTimeS;
                            }
                        }
                    }
                }
            }

        }

        private void Start()
        {
            ViewManager.GetInstance().Open<TestView>();

        }
    }
}
