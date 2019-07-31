using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace THEditor.UI
{
    public static class EasyUGUIMenu
    {
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
