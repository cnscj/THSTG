using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    [CustomEditor(typeof(MotionCurve))]
    public class MotionCurveInspector : Editor
    {
        void OnEnable()
        {

            SceneView.onSceneGUIDelegate += OnSceneViewGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
        }

        private void OnSceneViewGUI(SceneView sv)
        {
            MotionCurve mc = target as MotionCurve;
            //BezierExample be = target as BezierExample;

            //be.startPoint = Handles.PositionHandle(be.startPoint, Quaternion.identity);
            //be.endPoint = Handles.PositionHandle(be.endPoint, Quaternion.identity);
            //be.startTangent = Handles.PositionHandle(be.startTangent, Quaternion.identity);
            //be.endTangent = Handles.PositionHandle(be.endTangent, Quaternion.identity);

            //TODO:
            Handles.DrawBezier(mc.gameObject.transform.localPosition, new Vector3(-0.0f, 0.0f, 0.0f), Vector3.zero, Vector3.zero, Color.red, null, 2f);
        }

        public override void OnInspectorGUI()
        {
            MotionCurve mc = target as MotionCurve;
            EditorGUILayout.BeginVertical();

            EditorGUILayout.CurveField("CurveX", mc.curveX);
            EditorGUILayout.CurveField("CurveY", mc.curveY);
            EditorGUILayout.CurveField("CurveZ", mc.curveZ);

            EditorGUILayout.EndVertical();
        }

        
    }

}
