using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using THGame;
namespace THEditor
{
    public class EffectTools : ScriptableObject
    {
        public const int maxLevel = 10;
        public const int defaultLv = 1;

        [MenuItem("GameObject/Guangyv/特效分级/Level_01")]
        public static void MenuChangeLevelOne(){ChangEffectLevel(1);}
        [MenuItem("GameObject/Guangyv/特效分级/Level_02")]
        public static void MenuChangeLevelTwo(){ChangEffectLevel(2);}
        [MenuItem("GameObject/Guangyv/特效分级/Level_03")]
        public static void MenuChangeLevelThree() { ChangEffectLevel(3); }
        [MenuItem("GameObject/Guangyv/特效分级/Level_04")]
        public static void MenuChangeLevelFour() { ChangEffectLevel(4); }
        [MenuItem("GameObject/Guangyv/特效分级/Level_05")]
        public static void MenuChangeLevelFive() { ChangEffectLevel(5); }
        [MenuItem("GameObject/Guangyv/特效分级/Level_06")]
        public static void MenuChangeLevelSix() { ChangEffectLevel(6); }
        [MenuItem("GameObject/Guangyv/特效分级/Level_07")]
        public static void MenuChangeLevelSeven() { ChangEffectLevel(7); }
        [MenuItem("GameObject/Guangyv/特效分级/Level_08")]
        public static void MenuChangeLevelEight() { ChangEffectLevel(8); }
        [MenuItem("GameObject/Guangyv/特效分级/Level_09")]
        public static void MenuChangeLevelNine() { ChangEffectLevel(9); }
        [MenuItem("GameObject/Guangyv/特效分级/Level_10")]
        public static void MenuChangeLevelTen() { ChangEffectLevel(10); }


        public static void ChangEffectLevel(int level)
        {
            if (Selection.transforms.Length > 0 )
            {
                Transform []GOs = Selection.transforms;
                foreach(var GO in GOs)
                {
                    //需要递归往上做检查
                    int minLv = level;
                    Transform father = GO.transform.parent;
                    while (father != null)
                    {
                        int fatherLv = EffectUtil.GetEffectLevel(father.gameObject);
                        if (fatherLv == -1)
                        {
                            fatherLv = 1;
                        }
                        if (fatherLv <= minLv)
                        {
                            minLv = fatherLv;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("警告", string.Format("父节点存在级别较小的,所以这里只能大于{0}级别", fatherLv), "好的");
                            return;
                        }
                        father = father.parent;
                    }
                    EffectUtil.SetEffectLevel(GO.gameObject, level);
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
                if (ridLv > 0 && ridLv <= maxLevel)
                {
                    GameObject newGO = GameObject.Instantiate(srcGO);

                    foreach (var node in newGO.GetComponentsInChildren<Transform>())
                    {
                        int nodeLv = EffectUtil.GetEffectLevel(node.gameObject);
                        if (nodeLv != -1)
                        {
                            if (nodeLv > ridLv)
                            {
                                Object.DestroyImmediate(node.gameObject);
                            }
                            else
                            {
                                //去掉后四位的等级("_L%02d")
                                node.gameObject.name = node.gameObject.name.Remove(node.gameObject.name.Length - 4);
                            }
                        }
                    }
                    outGO = PrefabUtility.SaveAsPrefabAsset(newGO, saveName);
                    Object.DestroyImmediate(newGO);
                }
            }
            return outGO;
        }

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
                for (int i = 1; i <= maxLevel; i++)
                {
                    string saveName = Path.Combine(finalPath, string.Format("{0}_{1:D2}.prefab", effectId, i));
                    SavePrefabByLevel(srcGO, saveName, i);
                }
            }

        }
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
                    int effectLv = EffectUtil.GetEffectLevel(effectGO.gameObject);
                    if (effectLv == -1)
                    {
                        effectLv = defaultLv;
                    }
                    if (effectLv >= defaultLv && effectLv <= maxLevel)
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
                foreach(var pair in nodeMap)
                {
                    int level = pair.Key;
                    for(int i = 0; i < pair.Value.Count; i++)
                    {
                        pair.Value[i].name = string.Format("{0:D2}_{1}", level, i);
                    }

                }
                string finalPath = Path.Combine(savePath, effectId);
                if (!XFolderTools.Exists(finalPath))
                {
                    XFolderTools.CreateDirectory(finalPath);
                }
                var ctrl = newGO.AddComponent<EffectController>();
                ctrl.metadataList = new List<EffectMetadata>();

                for(int i = maxLevel; i >= defaultLv ; i--)
                {
                    if (nodeMap.ContainsKey(i))
                    {
                        EffectMetadata newData = new EffectMetadata();
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