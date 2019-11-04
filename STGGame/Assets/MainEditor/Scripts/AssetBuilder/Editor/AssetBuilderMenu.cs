using System.Collections;
using System.Collections.Generic;
using THEditor;
using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public static class AssetBuilderMenu
    {
       

        [MenuItem("THSTG/资源/资源打包全部", false, 13)]
        public static void MenuOneKeyBuildAll()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            AssetBuilderMain.builderManager.BuildAll();
            sw.Stop();
            Debug.Log(string.Format("打包完成,耗时:{0} s", sw.ElapsedMilliseconds / 1000f));
        }
    }

}
