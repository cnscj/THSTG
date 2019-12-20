using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class STGTimelineEditorWindow : EditorWindow
    {
        [MenuItem("THFramework/STGTimeline/STGTimeline辅助")]
        public static void ShowWindow()
        {
            STGTimelineEditorWindow myWindow = (STGTimelineEditorWindow)EditorWindow.GetWindow(typeof(STGTimelineEditorWindow), false, "STGTimeline", false);//创建窗口
            myWindow.Show();//展示

        }
        public float embattleSurroundRadius;


        void OnGUI()
        {
            GUILayout.BeginVertical();

            embattleSurroundRadius = EditorGUILayout.FloatField("包围半径", embattleSurroundRadius);
            if (GUILayout.Button("排序"))
            {
                EmbattleSurround();
            }

            GUILayout.EndVertical();
        }

        private void OnEnable()
        {

         

        }

        private void EmbattleSurround()
        {
            var gos = GetSelections();
            var center = GetCenter(gos);
            float iPerAngle = 2 * Mathf.PI / gos.Count;
            for (int i = 0; i < gos.Count; i++)
            {
                var angle = iPerAngle * i;
                var posX = embattleSurroundRadius * Mathf.Cos(angle);
                var posY = embattleSurroundRadius * Mathf.Sin(angle);

                var rotZ = angle * Mathf.Rad2Deg - 90;

                gos[i].transform.position = new Vector3(center.x + posX, center.y + posY, gos[i].transform.position.z);
                gos[i].transform.eulerAngles = new Vector3(0, 0, rotZ);
            }

        }

        public static Vector3 GetCenter(List<GameObject> gos)
        {
            if (gos != null)
            {
                Bounds bounds = new Bounds();
                foreach (GameObject go in gos)
                {
                    bounds.Encapsulate(go.transform.position);
                }
                return bounds.center;
            }
            return Vector3.zero;

        }

        private void TraverseSelections(Action<int,GameObject[]> func)
        {
            if (func != null)
            {
                GameObject[] objs = Selection.objects as GameObject[];
                if (objs != null && objs.Length > 0)
                {
                    for (int i = 0; i < objs.Length; i++)
                    {
                        func(i, objs);
                    }
                }
            }

        }

        private List<GameObject> GetSelections()
        {
            UnityEngine.Object[]objs = Selection.objects;
            List<GameObject> list = new List<GameObject>(objs.Length);
            foreach(var go in objs)
            {
                list.Add(go as GameObject);
            }
            return list;
        }


    }

}
