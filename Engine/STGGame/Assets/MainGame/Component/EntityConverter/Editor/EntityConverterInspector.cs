using UnityEngine;
using UnityEditor;
using XLibEditor;
using STGGame;
using STGU3D;

namespace STGEditor
{
    [CustomEditor(typeof(EntityConverter))]
    public class EntityConverterInspector : MonoGUI<EntityConverter>
    {
        SerializedProperty serproEntityCode;
        protected override void OnProps()
        {
            serproEntityCode = AddProperty("entityCode");
            AddProperty("isLink","是否链接到实体","base");

            AddProperty("entityType", "实体类型", "entity");

            AddProperty("heroType", "Hero类型", "hero");
            AddProperty("bossType", "Boss类型", "boss");
            AddProperty("wingmanType", "僚机类型", "wingman");
            AddProperty("type", "子类型", "exHBW");

            AddProperty("comsList", "组件列表", "common");
        }

        protected override void OnShow()
        {
            if (string.IsNullOrEmpty(m_editor.entityCode))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serproEntityCode, new GUIContent("实体Code"));
                if (GUILayout.Button("刷新"))
                {
                    m_editor.RefreshCode();
                }
                EditorGUILayout.EndHorizontal();
                ShowPropertys("base");
                ShowPropertys("entity");
                switch (m_editor.entityType)
                {
                    case EEntityType.Hero:
                        ShowPropertys("hero");
                        break;
                    case EEntityType.Boss:
                        ShowPropertys("boss");
                        break;
                    case EEntityType.Wingman:
                        ShowPropertys("wingman");
                        break;
                    default:
                        ShowPropertys("exHBW");
                        break;
                }
            }else
            {
                EditorGUILayout.PropertyField(serproEntityCode, new GUIContent("实体Code"));
                ShowPropertys("base");
            }
            ShowPropertys("common");
        }
    }
}
