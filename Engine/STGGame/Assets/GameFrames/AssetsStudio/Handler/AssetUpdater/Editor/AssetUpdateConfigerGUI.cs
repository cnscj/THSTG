using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XLibEditor;

namespace ASEditor
{
    public class AssetUpdateConfigerGUI1 : WindowGUI<AssetUpdateConfigerGUI1>
    {
        [MenuItem("AssetsStudio/资源更新配置", false, 6)]
        static void ShowWnd()
        {
            ShowWindow("资源更新配置");
        }

        protected override void OnInit()
        {


        }

        protected override void OnShow()
        {
            GUILayout.BeginVertical();
          
            GUILayout.EndVertical();
        }

    }
}
