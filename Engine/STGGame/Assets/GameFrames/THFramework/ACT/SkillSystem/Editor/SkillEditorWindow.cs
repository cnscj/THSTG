
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
        private Vector2 _scrollPos3 = Vector2.zero;

        bool _actionFoldout = true;
        bool _effectFoldout = true;
        bool _eventFoldout = true;
        bool _audioFoldout = true;

        [MenuItem("THFramework/技能编辑")]
        public static void ShowGUI()
        {
            ShowWindow("SkillEditor");
        }

        protected override void OnShow()
        {
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            {
                //打开文件
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("新建", GUILayout.Width(100)))
                {

                }
                EditorGUILayout.EndHorizontal();
                //


                //
                EditorGUILayout.BeginHorizontal();
                _scrollPos1 = EditorGUILayout.BeginScrollView(_scrollPos1, (GUIStyle)"FIle List");
                _srcSearchTextField.OnGUI();
                EditorGUILayout.EndScrollView();

                _scrollPos2 = EditorGUILayout.BeginScrollView(_scrollPos2, (GUIStyle)"Skill List");
                EditorGUILayout.EndScrollView();

                _scrollPos3 = EditorGUILayout.BeginScrollView(_scrollPos3, (GUIStyle)"Config List");
                if (_actionFoldout = EditorGUILayout.Foldout(_actionFoldout, "Action"))
                {

                }

                if (_effectFoldout = EditorGUILayout.Foldout(_effectFoldout, "Effect"))
                {

                }


                if (_eventFoldout = EditorGUILayout.Foldout(_eventFoldout, "Event"))
                {

                }

                if (_audioFoldout = EditorGUILayout.Foldout(_audioFoldout, "Audio"))
                {

                }
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

        }
    }
}

