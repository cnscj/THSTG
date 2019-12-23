using UnityEditor;
using THGame;
using UnityEngine;
using System.Collections.Generic;

namespace THEditor
{
    [CustomEditor(typeof(ObjectEmitter))]
    public class ObjectEmitterInspector : Editor
    {
        private ObjectEmitter m_editor;
        private List<KeyValuePair<string, SerializedProperty>> normalProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> normalEndProps = new List<KeyValuePair<string, SerializedProperty>>();

        private SerializedProperty m_launchOrderType;
        private List<KeyValuePair<string, SerializedProperty>> launchOrderProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> launchOrderFixProps = new List<KeyValuePair<string, SerializedProperty>>();

        private SerializedProperty m_launchType;
        private List<KeyValuePair<string, SerializedProperty>> lineProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> sectorProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> randomProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> fixedPointProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> customProps = new List<KeyValuePair<string, SerializedProperty>>();



        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            serializedObject.Update();//属性序列化

            ShowNormalProps();
            ShowLaunchOrderTypeProps();
            ShowLaunchTypeProps();
            ShowNormalEndProps();

            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }

        void ShowNormalProps()
        {
            ShowPropertys(normalProps);
        }

        void ShowNormalEndProps()
        {
            EditorGUILayout.Space();
            ShowPropertys(normalEndProps);
        }

        void ShowLaunchOrderTypeProps()
        {
            EditorGUILayout.PropertyField(m_launchOrderType, new GUIContent("发射顺序"));
            ShowPropertys(launchOrderProps);
            switch (m_editor.launchOrderType)
            {
                case EObjectEmitCreateOrderType.Orderly:
                    break;
                case EObjectEmitCreateOrderType.Random:
                    break;
                case EObjectEmitCreateOrderType.Fixed:
                    ShowPropertys(launchOrderFixProps);
                    break;
            }
        }

        void ShowLaunchTypeProps()
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_launchType, new GUIContent("发射类型"));
            switch (m_editor.launchType)
            {
                case EObjectEmitLaunchType.Line:
                    ShowPropertys(lineProps);
                    break;
                case EObjectEmitLaunchType.Sector:
                    ShowPropertys(sectorProps);
                    break;
                case EObjectEmitLaunchType.Random:
                    ShowPropertys(randomProps);
                    break;
                case EObjectEmitLaunchType.FixedPoint:
                    ShowPropertys(fixedPointProps);
                    break;
                case EObjectEmitLaunchType.Custom:
                    ShowPropertys(customProps);
                    break;
            }
        }

        void AddPropertys(List<KeyValuePair<string, SerializedProperty>> list, string nickName, string property)
        {
            SerializedProperty prop = serializedObject.FindProperty(property);
            KeyValuePair<string, SerializedProperty> pair = new KeyValuePair<string, SerializedProperty>(nickName, prop);
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
            m_editor = (ObjectEmitter)target;
            Clear();

            AddPropertys(normalProps, "发射实体队列", "launchEntities");
            AddPropertys(normalProps, "发射父节点", "launchParent");
            AddPropertys(normalProps, "发射相对节点", "launchRelative");
            AddPropertys(normalProps, "发射次数", "launchTimes");
            AddPropertys(normalProps, "发射频度(s)", "launchFreq");
            AddPropertys(normalProps, "发射数量", "launchNum");
            AddPropertys(normalProps, "发射线速度", "launchMoveSpeed");
            AddPropertys(normalProps, "发射角速度", "launchAngleSpeed");
            AddPropertys(normalProps, "发射角固定", "launchFixAngle");

            m_launchOrderType = serializedObject.FindProperty("launchOrderType");

            AddPropertys(launchOrderProps, "按次数递增", "launchOrderOrderlyByTimes");
            AddPropertys(launchOrderFixProps, "实体序号", "launchOrderFixedIndex");
            

            m_launchType = serializedObject.FindProperty("launchType");
            AddPropertys(lineProps, "发射长度", "launchLineDistance");
            AddPropertys(lineProps, "发射角度", "launchLineAngle");
            AddPropertys(lineProps, "发射角度增量", "launchLineRPT");

            AddPropertys(sectorProps, "扇形半径", "launchSectorRadius");
            AddPropertys(sectorProps, "起始角", "launchSectorStartAngle");
            AddPropertys(sectorProps, "张角", "launchSectorSpreadAngle");

            AddPropertys(randomProps, "最小半径", "launchRandomMinRadius");
            AddPropertys(randomProps, "最大半径", "launchRandomMaxRadius");

            AddPropertys(fixedPointProps, "固定点列表", "launchFixedPointPoints");
            
            AddPropertys(customProps, "自定义函数", "launchCustomCallback");
        }
        void Clear()
        {
            normalProps.Clear();
            normalEndProps.Clear();
            launchOrderProps.Clear();
            launchOrderFixProps.Clear();

            lineProps.Clear();
            sectorProps.Clear();
            randomProps.Clear();
            customProps.Clear();
        }

        void Awake()
        {
           

        }

        void OnDisable()
        {
            if (m_editor)
            {
                m_editor.launchRelative = m_editor.launchRelative != null ? m_editor.launchRelative : m_editor.gameObject;
            }
        }

    }
}
