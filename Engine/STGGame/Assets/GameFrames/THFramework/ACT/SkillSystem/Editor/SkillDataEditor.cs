using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class SkillDataEditor : EditorWindow
    {
        [MenuItem("THEditor/Skill Data Editor")]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SkillDataEditor));
        }
    }
}
