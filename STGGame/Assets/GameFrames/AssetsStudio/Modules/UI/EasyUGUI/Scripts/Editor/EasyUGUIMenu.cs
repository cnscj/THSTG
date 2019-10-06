using System;
using ASGame.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ASEditor.UI
{
    public static class EasyUGUIMenu
    {
        //[MenuItem("GameObject/THUI/View",false ,2)]
        public static void MenuCreatePanel()
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
