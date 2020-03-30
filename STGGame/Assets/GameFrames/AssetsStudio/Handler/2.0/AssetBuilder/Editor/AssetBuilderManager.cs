using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetBuilderManager : Singleton<AssetBuilderManager>
    {
        //需要进行一次全局依赖Share打包
        private LinkedList<AssetBaseBuilder> m_builderList = new LinkedList<AssetBaseBuilder>();
        private Dictionary<string, int> m_refCounts = new Dictionary<string, int>();

        public void CollectDependencies(string assetPath)
        {
            string[] dps = AssetDatabase.GetDependencies(assetPath);
            foreach (var dp in dps)
            {
                if (m_refCounts.TryGetValue(dp, out var refCount))
                {
                    m_refCounts[dp] = refCount + 1;
                }
                else
                {
                    m_refCounts.Add(dp, 1);
                }
            }
        }

        public void AddIntoBuildMap(string assetPath, string assetBundleName)
        {

        }

        public List<AssetBundleBuild> GetBuildMap()
        {

            return null;
        }

        public void Do()
        {
            Clear();

            Setting();
            Build();
        }

        public void Clear()
        {

        }

        private void Setting()
        {
            Setting4Share();
        }

        private void Build()
        {

        }

        private void Setting4Share()
        {

        }
    }

}
