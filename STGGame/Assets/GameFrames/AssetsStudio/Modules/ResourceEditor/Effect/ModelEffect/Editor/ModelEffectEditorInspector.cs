using UnityEngine;
using UnityEditor;
using ASGame.Editor;

namespace ASEditor
{
    [CustomEditor(typeof(ModelEffectEditor))]
    public class ModelEffectEditorInspector : Editor
    {
        private ModelEffectEditor m_modelEffectEditor;
       
        private SerializedProperty m_curModelPrefab;
        private SerializedProperty m_curModelEffectPrefab;
        private SerializedProperty m_curLevel;
        private SerializedProperty m_curMetadataList;

        private int m_selectedIndex = 1;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
        
            //属性序列化
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_curModelPrefab);
            EditorGUILayout.PropertyField(m_curModelEffectPrefab);
            EditorGUILayout.PropertyField(m_curLevel);
            EditorGUILayout.PropertyField(m_curMetadataList, true);

            serializedObject.ApplyModifiedProperties();

            //m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_modelEffectEditor.animStateList.ToArray());
            if (GUILayout.Button("播放"))
            {
                m_modelEffectEditor.PlayAnimation(m_selectedIndex);
            }

            if (GUILayout.Button("刷新"))
            {
                RefreshList();
                EditorUtility.SetDirty(m_modelEffectEditor.gameObject);
            }
            // if (m_modelEffectEditor.modelEffectPrefab != null)
            // {
            //     if (GUILayout.Button("保存"))
            //     {
            //         ExportPrefab();
            //     }
            // }
            // else
            // {
            //     if (GUILayout.Button("导出"))
            //     {
            //         ExportPrefab();
            //     }
            // }

            EditorGUILayout.EndVertical();
        }

        void OnEnable() 
        {
            m_modelEffectEditor = (ModelEffectEditor)target;
            m_curLevel = serializedObject.FindProperty("level");
            m_curMetadataList = serializedObject.FindProperty("metadataList");
            m_curModelPrefab = serializedObject.FindProperty("modelPrefab");
            m_curModelEffectPrefab = serializedObject.FindProperty("modelEffectPrefab");
        }

        private void RefreshList()
        {
            m_modelEffectEditor.Refresh();
        }
        
        private bool ExportPrefab()
        {
            return m_modelEffectEditor.Export();
        }

        void OnSceneGUI()
        {
            //得到test脚本的对象
            m_modelEffectEditor = (ModelEffectEditor)target;

            //开始绘制GUI
            Handles.BeginGUI();

            //规定GUI显示区域
            GUILayout.BeginArea(new Rect(0, 50, 100, 100));
            for(int i = 0; i < m_modelEffectEditor.animStateList.Count; i++)
            {
                var state = m_modelEffectEditor.animStateList[i];
                if (GUILayout.Button(state))
                {
                    m_modelEffectEditor.PlayAnimation(i);
                }
            }
            //GUI绘制一个按钮

          

            GUILayout.EndArea();

            Handles.EndGUI();
        }

    }
}
