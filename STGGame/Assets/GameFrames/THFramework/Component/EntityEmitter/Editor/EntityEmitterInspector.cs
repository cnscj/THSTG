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
        private List<KeyValuePair<string, SerializedProperty>> launchOrderFixProps = new List<KeyValuePair<string, SerializedProperty>>();

        private SerializedProperty m_launchType;
        private List<KeyValuePair<string, SerializedProperty>> lineProps = new List<KeyValuePair<string, SerializedProperty>>();
        private List<KeyValuePair<string, SerializedProperty>> surroundProps = new List<KeyValuePair<string, SerializedProperty>>();
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
            ShowPropertys(launchOrderProps);
            if (m_editor.launchOrderType == EntityEmitter.CreateOrderType.Orderly)
            {
               
            }
            else if (m_editor.launchOrderType == EntityEmitter.CreateOrderType.Random)
            {
 
            }
            else if (m_editor.launchOrderType == EntityEmitter.CreateOrderType.Fixed)
            {
                ShowPropertys(launchOrderFixProps);
            }
        }

        void ShowLaunchTypeProps()
        {
            EditorGUILayout.PropertyField(m_launchType, new GUIContent("发射类型"));
            if (m_editor.launchType == EntityEmitter.LaunchType.Line)
            {
                ShowPropertys(lineProps);
            }
            else if(m_editor.launchType == EntityEmitter.LaunchType.Surround)
            {
                ShowPropertys(surroundProps);
            }
            else if (m_editor.launchType == EntityEmitter.LaunchType.Random)
            {
                ShowPropertys(randomProps);
            }
            else if (m_editor.launchType == EntityEmitter.LaunchType.FixedPoint)
            {
                ShowPropertys(fixedPointProps);
            }
            else if (m_editor.launchType == EntityEmitter.LaunchType.Custom)
            {
                ShowPropertys(customProps);
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
            AddPropertys(normalProps, "发射父节点", "launchParent");
            AddPropertys(normalProps, "发射相对节点", "launchRelative");
            AddPropertys(normalProps, "发射频度(s)", "launchFreq");
            AddPropertys(normalProps, "发射次数", "launchTimes");
            AddPropertys(normalProps, "发射数量", "launchNum");
            AddPropertys(normalProps, "发射初速度", "launchSpeed");
            //AddPropertys(normalProps, "自动销毁", "launchAutoDestroy");
            //AddPropertys(normalProps, "强制发射", "launchForceLaunch");

            m_launchOrderType = serializedObject.FindProperty("launchOrderType");

            AddPropertys(launchOrderProps, "按次数递增", "launchOrderOrderlyByTimes");
            AddPropertys(launchOrderFixProps, "实体序号", "launchOrderFixedIndex");
            

            m_launchType = serializedObject.FindProperty("launchType");
            AddPropertys(lineProps, "发射角度", "launchLineAngle");
            AddPropertys(lineProps, "发射角度增量", "launchLineRPT");

            AddPropertys(surroundProps, "轨道半径", "launchSurroundRadius");

            AddPropertys(randomProps, "最小半径", "launchRandomMinRadius");
            AddPropertys(randomProps, "最大半径", "launchRandomMaxRadius");

            AddPropertys(fixedPointProps, "固定点列表", "launchFixedPointPoints");
            
            AddPropertys(customProps, "自定义类型", "launchCustomType");
            AddPropertys(customProps, "自定义数据1", "launchCustomData1");
            AddPropertys(customProps, "自定义数据2", "launchCustomData2");
            AddPropertys(customProps, "自定义数据2", "launchCustomData3");
        }
        void Clear()
        {
            normalProps.Clear();
            launchOrderProps.Clear();
            launchOrderFixProps.Clear();

            lineProps.Clear();
            surroundProps.Clear();
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
