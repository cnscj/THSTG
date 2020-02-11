using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace THEditor
{
    public class DialogEditorWindow : WindowGUI<DialogEditorWindow>
    {
        [MenuItem("THFramework/剧情编辑/对话编辑")]
        public static void ShowGUI()
        {
            ShowWindow("DialgoEditor");
        }

    }

}


