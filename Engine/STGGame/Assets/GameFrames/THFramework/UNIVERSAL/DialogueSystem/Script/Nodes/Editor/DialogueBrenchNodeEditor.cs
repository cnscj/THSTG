using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;
using static XNodeEditor.NodeEditor;

namespace THEditor
{
    [CustomNodeEditor(typeof(DialogueBrenchNode))]
    public class DialogueBrenchNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            DialogueBrenchNode node = target as DialogueBrenchNode;
            NodeEditorGUILayout.PortField(target.GetInputPort("input"));
            EditorGUILayout.Space();
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("conditions"));
            NodeEditorGUILayout.PortField(target.GetOutputPort("pass"));
            NodeEditorGUILayout.PortField(target.GetOutputPort("fail"));

            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 336;
        }

    }
}