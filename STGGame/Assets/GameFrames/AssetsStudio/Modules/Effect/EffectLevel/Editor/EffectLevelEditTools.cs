using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using XLibrary;
using ASGame;

namespace ASEditor
{
    public static class EffectLevelEditTools
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
                        int fatherLv = EffectLevelUtil.GetEffectLevel(father.gameObject);
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
                        int childLv = EffectLevelUtil.GetEffectLevel(child.gameObject);
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

                    EffectLevelUtil.SetEffectLevel(GO.gameObject, level);
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
                    EffectLevelUtil.ShowEffectLevel(GO.gameObject, level);
                }
            }
            else
            {
                Debug.LogWarning("请选择一个节点");
            }
        }

        //处理一个
        public static bool RidGameObjectByLv(GameObject newGO, int ridLv)
        {
            if (newGO)
            {
                if (ridLv > 0 && ridLv <= EffectLevelUtil.maxLevel)
                {
                    var levelCtrl = newGO.GetComponent<EffectLevelController>();
                    if (!levelCtrl) levelCtrl = newGO.AddComponent<EffectLevelController>();
                    levelCtrl.level = ridLv;
                    levelCtrl.nodeList = (levelCtrl.nodeList != null) ? levelCtrl.nodeList : new List<EffectLevelController.EffectLevelInfo>();
                    levelCtrl.nodeList.Clear();

                    foreach (var node in newGO.GetComponentsInChildren<Transform>(true))
                    {
                        int nodeLv = EffectLevelUtil.GetEffectLevel(node.gameObject);
                        if (nodeLv != -1)
                        {
                            if (nodeLv > ridLv)
                            {
                                Object.DestroyImmediate(node.gameObject);
                            }
                            else
                            {
                                var info = new EffectLevelController.EffectLevelInfo();
                                info.node = node.gameObject;
                                info.path = XGameObjectTools.GetPathByGameObject(node.gameObject, newGO);
                                info.level = nodeLv;

                                levelCtrl.nodeList.Add(info);
                            }
                        }
                    }
                    levelCtrl.SortList();

                    return true;
                }
            }
            return false;
        }

        //10级包含所有
        public static GameObject SavePrefabByLevel(GameObject srcGO, string saveName, int ridLv)
        {
            GameObject outGO = null;
            if (srcGO)
            {
               
                GameObject newGO = GameObject.Instantiate(srcGO);

                RidGameObjectByLv(newGO, ridLv);

                outGO = PrefabUtility.SaveAsPrefabAsset(newGO, saveName);
                Object.DestroyImmediate(newGO);
                
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
                for (int i = 1; i <= EffectLevelUtil.maxLevel; i++)
                {
                    string saveName = Path.Combine(finalPath, string.Format("{0}_{1:D2}.prefab", effectId, i));
                    SavePrefabByLevel(srcGO, saveName, i);
                }
            }

        }
    }
}