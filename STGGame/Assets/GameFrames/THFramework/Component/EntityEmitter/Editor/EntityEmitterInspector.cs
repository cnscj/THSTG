using UnityEditor;
using THGame;
using UnityEngine;
using System.Collections.Generic;

namespace THEditor
{
    [CustomEditor(typeof(EntityEmitter))]
    public class EntityEmitterInspector : Editor
    {
        private EntityEmitter m_editor;
        private List<KeyValuePair<string, SerializedProperty>> normalProps = new List<KeyValuePair<string, SerializedProperty>>();


        private SerializedProperty m_launchOrderType;
        private List<KeyValuePair<string, SerializedProperty>> launchOrderProps = new List<KeyValuePair<string, SerializedProperty>>();

        private SerializedProperty m_launchType;
        private List<KeyValuePair<string, SerializedProperty>> lineProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> surroundProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> randomProps = new List<KeyValuePair<string, SerializedProperty>>();

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            serializedObject.Update();//属性序列化

            ShowNormalProps();
            ShowLaunchOrderTypeProps();
            ShowLaunchTypeProps();

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

        void ShowNormalProps()
        {
            ShowPropertys(normalProps);
        }

        void ShowLaunchOrderTypeProps()
        {
            EditorGUILayout.PropertyField(m_launchOrderType, new GUIContent("发射顺序"));
            if (m_editor.launchOrderType == EntityEmitter.ECreateOrderType.Fixed)
            {
                ShowPropertys(launchOrderProps);
            }
        }

        void ShowLaunchTypeProps()
        {
            EditorGUILayout.PropertyField(m_launchType, new GUIContent("发射类型"));
            if (m_editor.launchType == EntityEmitter.EDiffusionType.Line)
            {
                ShowPropertys(lineProps);
            }
            else if(m_editor.launchType == EntityEmitter.EDiffusionType.Surround)
            {
                ShowPropertys(surroundProps);
            }
            else if (m_editor.launchType == EntityEmitter.EDiffusionType.Random)
            {
                ShowPropertys(randomProps);
            }
        }

        void AddPropertys(List<KeyValuePair<string, SerializedProperty>> list, string name, string property)
        {
            SerializedProperty prop = serializedObject.FindProperty(property);
            KeyValuePair<string, SerializedProperty> pair = new KeyValuePair<string, SerializedProperty>(name, prop);
            if (prop != null)
            {
                list.Add(pair);
            }
        }
        void ShowPropertys(List<KeyValuePair<string, SerializedProperty>> list)
        {
            foreach (var pair in list)
            {
                EditorGUILayout.PropertyField(pair.Value, new GUIContent(pair.Key), true);
            }
        }


        private void OnEnable()
        {
            m_editor = (EntityEmitter)target;
            Clear();


            AddPropertys(normalProps, "发射实体队列", "launchEntities");
            AddPropertys(normalProps, "发射频度(s)", "launchFreq");
            AddPropertys(normalProps, "发射次数", "launchTimes");
            AddPropertys(normalProps, "发射数量", "launchNum");
            AddPropertys(normalProps, "发射初速度", "launchSpeed");
            AddPropertys(normalProps, "自动销毁", "launchAutoDestroy");
            AddPropertys(normalProps, "强制发射", "launchForceLaunch");

            m_launchOrderType = serializedObject.FindProperty("launchOrderType");
            AddPropertys(launchOrderProps, "实体序号", "launchOrderFixedIndex");

            m_launchType = serializedObject.FindProperty("launchType");
            AddPropertys(lineProps, "发射角度(°)", "launchLineAngle");
            AddPropertys(lineProps, "持续时间", "launchLineDuration");

            AddPropertys(surroundProps, "最小半径", "launchSurroundMinRadius");
            AddPropertys(surroundProps, "最大半径", "launchSurroundMaxRadius");

            AddPropertys(randomProps, "最小半径", "launchRandomMinRadius");
            AddPropertys(randomProps, "最大半径", "launchRandomMaxRadius");

        }
        void Clear()
        {
            normalProps.Clear();
            launchOrderProps.Clear();

            lineProps.Clear();
            surroundProps.Clear();
            randomProps.Clear();
        }

        void Awake()
        {
           

        }

    }
}
