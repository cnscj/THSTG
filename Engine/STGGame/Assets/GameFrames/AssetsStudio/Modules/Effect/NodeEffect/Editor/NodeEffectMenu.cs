using System.Collections;
using System.Collections.Generic;
using ASGame;
using UnityEditor;
using UnityEngine;

namespace ASEditor
{
    public static class NodeEffectMenu
    {
        [MenuItem("GameObject/ASEditor/资源扩展/特效/节点特效/创建节点特效节点")]
        public static void CreateEffectNode()
        {
            if (Selection.transforms.Length > 0)
            {
                Transform[] GOs = Selection.transforms;
                foreach (var GO in GOs)
                {
                    GameObject effectNode = new GameObject(NodeEffectConfig.EFFECT_NODE_NAME);
                    effectNode.transform.SetParent(GO.transform, false);
                }
            }
            else
            {
                Debug.LogWarning("请选择一个节点");
            }
        }
    }
}
