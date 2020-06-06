using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace STGRuntime
{
    public class PackageCustomLoader
    {
        public enum LoadMode
        {
            None,
            Path,
            Bundles,
        }
        private Func<string, string> m_loaderWithPath;
        private Func<string, AssetBundle[]> m_loaderWithBundle;
        private LoadMode m_mode;

        public PackageCustomLoader()
        {
            m_mode = LoadMode.None;
        }
        public PackageCustomLoader(Func<string, string> loader)
        {
            m_loaderWithPath = loader;
            m_mode = LoadMode.Path;
        }
        public PackageCustomLoader(Func<string, AssetBundle[]> loader)
        {
            m_loaderWithBundle = loader;
            m_mode = LoadMode.Bundles;
        }

        public LoadMode GetLoadMode()
        {
            return m_mode;
        }

        public UIPackage Load(string packageName)
        {
            if (GetLoadMode() == PackageCustomLoader.LoadMode.Path)
            {
                object result = m_loaderWithPath?.Invoke(packageName);
                string uiPath = result as string;
                return UIPackage.AddPackage(uiPath);

            }
            else if (GetLoadMode() == PackageCustomLoader.LoadMode.Bundles)
            {
                object result = m_loaderWithBundle?.Invoke(packageName);
                AssetBundle[] abs = (AssetBundle[])result;
                AssetBundle binaryAb = abs.Length > 0 ? abs[0] : null;
                AssetBundle altasAb = abs.Length > 1 ? abs[1] : null;
                return UIPackage.AddPackage(binaryAb, altasAb);
            }
            return null;
        }

        public void LoadAsync(string packageName, Action<UIPackage> action)
        {
            UIPackage package = Load(packageName);
            action?.Invoke(package);
        }
    }

}
