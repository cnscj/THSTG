using UnityEditor;
using UnityEngine;

namespace STGEditor
{
    public static class STGEditorMenu
    {
        [MenuItem("THSTG/处理并播放", false, 11)]
        public static void MenAllPlay()
        {
            AssetProcessorMain.processManager.ProcessAll();
            EditorApplication.isPlaying = true;
        }

        [MenuItem("THSTG/资源/后处理+打包", false, 12)]
        public static void MenuOneKeyAll()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            AssetProcessorMain.processManager.ProcessAll();
            AssetBuilderMain.builderManager.BuildAll();
            sw.Stop();
            Debug.Log(string.Format("处理完成,耗时:{0} s", sw.ElapsedMilliseconds / 1000f));
           
        }


    }
}