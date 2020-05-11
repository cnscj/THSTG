using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace STGEditor
{
    public static class AssetProcessorMenu
    {
        [MenuItem("THSTG/资源/后处理全部", false, 12)]
        public static void MenuOneKeyAll()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            AssetProcessorMain.processManager.ProcessAll();
            sw.Stop();
            Debug.Log(string.Format("后处理完成,耗时:{0} s", sw.ElapsedMilliseconds/1000f));
        }
    }
}

