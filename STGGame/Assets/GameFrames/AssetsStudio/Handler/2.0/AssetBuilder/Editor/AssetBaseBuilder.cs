using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    public abstract class AssetBaseBuilder
    {
        protected string _builderName;
        protected Dictionary<string, string> m_buildMap = new Dictionary<string, string>();

        public AssetBaseBuilder(string name)
        {
            _builderName = name;
        }

        public void Do()
        {
            DoStart();

            DoAssets();

            DoEnd();
        }

        private void DoStart()
        {
            OnStart();
        }

        private void DoAssets()
        {
            string[] assetFiles = OnFiles();
            if (assetFiles == null || assetFiles.Length < 0)
                return;

            foreach (var assetPath in assetFiles)
            {
                string realPath = XFileTools.GetFileRelativePath(assetPath);
                string realPathLow = realPath.ToLower();
                string bundleName = OnName(assetPath);
                if (string.IsNullOrEmpty(bundleName))
                    continue;

                bundleName = bundleName.ToLower();
                m_buildMap[realPathLow] = bundleName;
            }
        }

        private void DoEnd()
        {
            OnEnd();
        }

        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }
        protected abstract string[] OnFiles();
        protected abstract string OnName(string assetPath);

    }
}
