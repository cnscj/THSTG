using System.Collections;
using XLibrary.Package;
using FairyGUI;
using System.Collections.Generic;
using ASGame;
using System;
using UnityEngine;

namespace STGGame
{
    public class PackageManager : MonoSingleton<PackageManager>
    {
        public int residentTimeS = 120;

        private Func<string,KeyValuePair<PackageLoadMode, System.Object>> m_loader;
        private Dictionary<string, PackageInfo> m_packageMap = new Dictionary<string, PackageInfo>();

        public PackageManager()
        {

        }

        public void SetLoader(Func<string, KeyValuePair<PackageLoadMode, System.Object>> loader)
        {
            m_loader = loader;
        }

        public PackageInfo AddPackage(string packageName)
        {
            if (m_loader != null)
            {
                UIPackage package = null;
                KeyValuePair<PackageLoadMode, System.Object> info = m_loader(packageName);
                if (info.Key == PackageLoadMode.PathString)
                {
                    string uiPath = info.Value as string;
                    UIPackage.AddPackage(uiPath);
                }
                else if(info.Key == PackageLoadMode.AssetBundlePair)
                {
                    KeyValuePair<AssetBundle, AssetBundle> pair = (KeyValuePair<AssetBundle, AssetBundle>)info.Value;
                    UIPackage.AddPackage(pair.Key, pair.Value);
                }

                if (package != null)
                {
                    PackageInfo packageInfo = new PackageInfo();
                    packageInfo.package = package;
                    packageInfo.residentTimeS = residentTimeS;
                    packageInfo.accessTimestamp = Time.realtimeSinceStartup;
                    m_packageMap.Add(package.name, packageInfo);

                    return packageInfo;
                }
            }
            return null;
        }

        public void RemovePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                return;

            if (!m_packageMap.ContainsKey(packageName))
                return;

            UIPackage.RemovePackage(packageName);

            m_packageMap.Remove(packageName);
        }

        public PackageInfo GetPackageInfo(string packageName)
        {
            if (m_packageMap.ContainsKey(packageName))
            {
                return m_packageMap[packageName];
            }
            return null;
        }

    }

}

