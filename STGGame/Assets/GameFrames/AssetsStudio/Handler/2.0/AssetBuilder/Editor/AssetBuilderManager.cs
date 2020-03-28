using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetBuilderManager : Singleton<AssetBuilderManager>
    {
        //需要进行一次全局依赖Share打包
        private LinkedList<AssetBaseBuilder> m_builderList = new LinkedList<AssetBaseBuilder>();
        private Dictionary<string, int> m_refCount = new Dictionary<string, int>();

        public void CollectDependencies(string path)
        {

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
