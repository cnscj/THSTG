
using UnityEditor;
using UnityEngine;
using XLibEditor;

namespace THEditor
{
    public class SkillEditorWindow : WindowGUI<SkillEditorWindow>
    {
        private SearchTextField _srcSearchTextField = new SearchTextField();

        private Vector2 _scrollPos1 = Vector2.zero;
        private Vector2 _scrollPos2 = Vector2.zero;

        [MenuItem("THFramework/技能编辑")]
        public static void ShowGUI()
        {
            ShowWindow("SkillEditor");
        }

        protected override void OnShow()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginVertical();

            //打开文件
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("新建", GUILayout.Width(100)))
            {
                
            }
            EditorGUILayout.EndHorizontal();


            //
            EditorGUILayout.BeginHorizontal();
            _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1, (GUIStyle)"Skill List");

            _srcSearchTextField.OnGUI();
            EditorGUILayout.EndHorizontal();

            _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, (GUIStyle)"Config List");


            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
    }
}

