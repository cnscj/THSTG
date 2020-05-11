
using UnityEditor;
using XLibEditor;

namespace THEditor
{
    public class SkillEditorWindow : WindowGUI<SkillEditorWindow>
    {
        [MenuItem("THFramework/技能编辑")]
        public static void ShowGUI()
        {
            ShowWindow("SkillEditor");
        }
    }
}

