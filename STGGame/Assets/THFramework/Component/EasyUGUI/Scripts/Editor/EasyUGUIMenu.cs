using System;
using System.Collections;
using System.Collections.Generic;
using THGame.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace THEditor.UI
{
    public static class EasyUGUIMenu
    {
        //[MenuItem("GameObject/THUI/Camera", false, 1)]
        //public static void MenuCreateUI()
        //{
        //    DoOnSelections((go) =>
        //    {
                
        //    });
        //}

        [MenuItem("GameObject/THUI/Component",false ,1)]
        public static void MenuCreateComponent()
        {
            DoOnSelections((go) =>
            {
                GameObject panelGo = new GameObject("Component");
                panelGo.transform.SetParent(go.transform);

                panelGo.AddComponent<RectTransform>();
                panelGo.AddComponent<CanvasRenderer>();

                panelGo.AddComponent<EComponent>();
            });
        }

        [MenuItem("GameObject/THUI/View",false ,2)]
        public static void MenuCreatePanel()
        {
            DoOnSelections((go) =>
            {
                GameObject panelGo = new GameObject("View");
                panelGo.transform.SetParent(go.transform);

                panelGo.AddComponent<RectTransform>();
                panelGo.AddComponent<CanvasRenderer>();
                var image = panelGo.AddComponent<Image>();
               

            });
        }

        [MenuItem("GameObject/THUI/List")]
        public static void MenuCreateList()
        {
            DoOnSelections((go) =>
            {

            });
        }


        public static void DoOnSelections(Action<GameObject> action)
        {
            if (Selection.transforms.Length > 0)
            {
                Transform[] GOs = Selection.transforms;
                foreach (var GO in GOs)
                {
                    action?.Invoke(GO.gameObject);
                }
            }
            else
            {
                Debug.LogWarning("请选择一个节点");
            }
        }
    }

}
