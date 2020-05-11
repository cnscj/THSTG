using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace THEditor
{
    public class CutsceneEditorWindow : WindowGUI<CutsceneEditorWindow>
    {
        [MenuItem("THFramework/剧情编辑/过场动画编辑")]
        public static void ShowGUI()
        {
            ShowWindow("CutsceneEditor");
        }

    }

}


