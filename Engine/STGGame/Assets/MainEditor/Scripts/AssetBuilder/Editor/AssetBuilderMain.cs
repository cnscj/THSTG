
using ASEditor;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public static class AssetBuilderMain
    {
        [MenuItem("THSTG/资源/资源打包全部", false, 13)]
        public static void MenuOneKeyBuildAll()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            AssetBuilderMain.BuildAll();
            sw.Stop();
            Debug.Log(string.Format("打包完成,耗时:{0} s", sw.ElapsedMilliseconds / 1000f));
        }

        public static void BuildAll()
        {
            AssetBuilderManager.GetInstance().Build();
        }
    }
}
