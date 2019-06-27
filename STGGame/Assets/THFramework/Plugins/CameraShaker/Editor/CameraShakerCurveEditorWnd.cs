using System;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class CameraShakerCurveEditorWnd : EditorWindow
    {
        private static float maxRange = 1.0f;
        private static float cycleTime = 0.12f;
        private static int cycleCount = 6;
        private static bool fixShake = false;

        private static Action<AnimationCurve> s_curCallback;
        private static AnimationCurve s_curCurve;

        public static void showWindow(AnimationCurve curve ,Action<AnimationCurve> callback)
        {
            EditorWindow.GetWindow(typeof(CameraShakerCurveEditorWnd));
            s_curCurve = curve;
            s_curCallback = callback;
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            EditorGUILayout.CurveField("曲线", s_curCurve);
            maxRange = EditorGUILayout.FloatField("最大震幅", maxRange);

            cycleTime = EditorGUILayout.FloatField("周期", cycleTime);
            cycleCount = EditorGUILayout.IntField("次数", cycleCount);
            fixShake = EditorGUILayout.Toggle("振幅不减", fixShake);

            if (GUILayout.Button("产生"))
            {
                Vector3 positionShake = Vector3.zero;
                Vector3 angleShake = Vector3.zero;
                var curve = CameraShakerUtil.CreateSimpleHarmonicCurve(maxRange, cycleTime, cycleCount, fixShake);
                s_curCurve = curve;
                s_curCallback(s_curCurve);
            }

            GUILayout.EndVertical();
        }
    }
}