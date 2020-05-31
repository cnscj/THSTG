using System.Collections;
using System.Collections.Generic;
using ASEditor;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public static class AssetProcessorMain
    {
        [MenuItem("THSTG/资源/资源后处理全部", false, 13)]
        public static void MenuOneKeyBuildAll()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            AssetProcessorMain.ProcessAll();
            sw.Stop();
            Debug.Log(string.Format("后处理完成,耗时:{0} s", sw.ElapsedMilliseconds / 1000f));
        }

        public static void ProcessAll()
        {
            AssetProcesserManager.GetInstance().Do();
        }
    }

}

