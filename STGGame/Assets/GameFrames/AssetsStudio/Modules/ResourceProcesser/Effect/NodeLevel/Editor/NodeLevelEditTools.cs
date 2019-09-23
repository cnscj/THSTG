using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using XLibrary;
using ASGame;

namespace ASEditor
{
    public static class NodeLevelEditTools
    {


        public static void ChangEffectLevel(int level)
        {
            if (Selection.transforms.Length > 0)
            {
                Transform[] GOs = Selection.transforms;
                foreach (var GO in GOs)
                {
                    //需要递归往上做检查
                    Transform targetNode;
                    int minLv = level;

                    Transform father = GO.transform.parent;
                    while (father != null)
                    {
                        int fatherLv = NodeLevelUtil.GetEffectLevel(father.gameObject);
                        fatherLv = fatherLv == -1 ? 1 : fatherLv;
                        targetNode = father;
                        if (fatherLv <= minLv)
                        {
                            minLv = fatherLv;
                        }
                        else
                        {
                            EditorGUIUtility.PingObject(targetNode); //跳转定位
                            EditorUtility.DisplayDialog("警告", string.Format("父节点({0})存在级别较小的,所以这里只能大于{1}级别", targetNode.name, fatherLv), "好的");
                            return;
                        }
                        father = father.parent;
                    }

                    //往下查找-必须让子节点最小的那个大于要设置的
                    targetNode = null;
                    minLv = level;
                    foreach (var child in GO.GetComponentsInChildren<Transform>(true))
                    {
                        if (child == GO)
                        {
                            continue;
                        }
                        int childLv = NodeLevelUtil.GetEffectLevel(child.gameObject);
                        childLv = childLv == -1 ? level : childLv;

                        if (childLv <= minLv)
                        {
                            minLv = childLv;
                            targetNode = child;
                        }

                    }
                    if (level > minLv)
                    {
                        EditorGUIUtility.PingObject(targetNode); //跳转定位
                        EditorUtility.DisplayDialog("警告", string.Format("子节点({0})存在级别较大的,所以这里只能小于或等于{1}级别", targetNode.name, minLv), "好的");
                        return;
                    }

                    NodeLevelUtil.SetEffectLevel(GO.gameObject, level);
                    EditorUtility.SetDirty(GO);
                    AssetDatabase.SaveAssets();
                }


            }
            else
            {
                Debug.LogWarning("请选择一个节点");
            }
        }

        //防止美术设置了又忘设置回全部显示,只能在运行时用
        public static void ShowEffectLevel(int level)
        {
            if (Selection.transforms.Length > 0)
            {
                Transform[] GOs = Selection.transforms;
                foreach (var GO in GOs)
                {
                    NodeLevelUtil.ShowEffectLevel(GO.gameObject, level);
                }
            }
            else
            {
                Debug.LogWarning("请选择一个节点");
            }
        }

        //10级包含所有
        public static GameObject SavePrefabByLevel(GameObject srcGO, string saveName, int ridLv)
        {
            GameObject outGO = null;
            if (srcGO)
            {
                if (ridLv > 0 && ridLv <= NodeLevelUtil.maxLevel)
                {
                    GameObject newGO = GameObject.Instantiate(srcGO);

                    foreach (var node in newGO.GetComponentsInChildren<Transform>())
                    {
                        int nodeLv = NodeLevelUtil.GetEffectLevel(node.gameObject);
                        if (nodeLv != -1)
                        {
                            if (nodeLv > ridLv)
                            {
                                Object.DestroyImmediate(node.gameObject);
                            }
                            else
                            {
                                NodeLevelUtil.ResetEffectLevel(node.gameObject);
                            }
                        }
                    }
                    outGO = PrefabUtility.SaveAsPrefabAsset(newGO, saveName);
                    Object.DestroyImmediate(newGO);
                }
            }
            return outGO;
        }
        //拆成10个
        public static void SaveAllLevelPrefabs(GameObject srcGO, string savePath)
        {
            if (srcGO)
            {
                string effectId = XStringTools.SplitPathId(srcGO.name);
                string finalPath = Path.Combine(savePath, effectId);
                if (!XFolderTools.Exists(finalPath))
                {
                    XFolderTools.CreateDirectory(finalPath);
                }
                for (int i = 1; i <= NodeLevelUtil.maxLevel; i++)
                {
                    string saveName = Path.Combine(finalPath, string.Format("{0}_{1:D2}.prefab", effectId, i));
                    SavePrefabByLevel(srcGO, saveName, i);
                }
            }

        }
        //补丁包的形式-配合NodeLevelController
        public static void SaveAllLevelPrefabs2(GameObject srcGO, string savePath)
        {
            if (srcGO)
            {
                string effectId = XStringTools.SplitPathId(srcGO.name);
                GameObject newGO = GameObject.Instantiate(srcGO);

                Dictionary<int, List<GameObject>> nodeMap = new Dictionary<int, List<GameObject>>();
                foreach (var effectGO in newGO.GetComponentsInChildren<Transform>())
                {
                    if (effectGO.gameObject == newGO) continue;     //不包括头结点
                    int effectLv = NodeLevelUtil.GetEffectLevel(effectGO.gameObject);
                    if (effectLv == -1)
                    {
                        effectLv = NodeLevelUtil.defaultLv;
                    }
                    if (effectLv >= NodeLevelUtil.defaultLv && effectLv <= NodeLevelUtil.maxLevel)
                    {
                        List<GameObject> list = null;
                        if (nodeMap.ContainsKey(effectLv))
                        {
                            list = nodeMap[effectLv];
                        }
                        else
                        {
                            list = new List<GameObject>();
                            nodeMap.Add(effectLv, list);
                        }
                        list.Add(effectGO.gameObject);
                    }

                }
                foreach (var pair in nodeMap)
                {
                    int level = pair.Key;
                    for (int i = 0; i < pair.Value.Count; i++)
                    {
                        pair.Value[i].name = string.Format("{0:D2}_{1}", level, i);
                    }

                }
                string finalPath = Path.Combine(savePath, effectId);
                if (!XFolderTools.Exists(finalPath))
                {
                    XFolderTools.CreateDirectory(finalPath);
                }
                var ctrl = newGO.AddComponent<NodeLevelController>();
                ctrl.metadataList = new List<NodeLevelMetadata>();

                for (int i = NodeLevelUtil.maxLevel; i >= NodeLevelUtil.defaultLv; i--)
                {
                    if (nodeMap.ContainsKey(i))
                    {
                        NodeLevelMetadata newData = new NodeLevelMetadata();
                        newData.effectList = new List<string>();
                        newData.level = i;
                        foreach (var effect in nodeMap[i])
                        {
                            newData.effectList.Add(XMiscTools.GetGameObjectPath(effect, newGO));
                        }
                        string packName = string.Format("{0}_{1:D2}", effectId, i);
                        GameObject levelPack = new GameObject(packName);
                        foreach (var effect in nodeMap[i])
                        {
                            effect.transform.SetParent(levelPack.transform);
                        }
                        PrefabUtility.SaveAsPrefabAsset(levelPack, Path.Combine(finalPath, packName + ".prefab"));
                        Object.DestroyImmediate(levelPack);

                        ctrl.metadataList.Add(newData);
                    }
                }
                ctrl.metadataList.Reverse();
                PrefabUtility.SaveAsPrefabAsset(newGO, Path.Combine(savePath, string.Format("{0}.prefab", effectId)));
                Object.DestroyImmediate(newGO);
            }

        }
    }
}