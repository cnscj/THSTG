using System.Collections.Generic;
using UnityEngine;
using XLibrary;

namespace ASGame
{

    public class EffectLevelController : MonoBehaviour
    {
        [System.Serializable]
        public class EffectLevelInfo
        {
            public GameObject node;
            public int level;
        }
        public string code;
        public int level;   //当前级别,受升级影响
        public List<EffectLevelInfo> nodeList;

        public void Change(int lv, GameObject prefab = null)
        {
            if (lv > level)
            {
                Upgrade(lv, prefab);
            }
            else if (lv < level)
            {
                Demote(lv);
            }
        }

        public void Demote(int lv)
        {
            //直接隐藏完事
            lv = FixLevel(lv);

            if (lv == level)
                return;

            if (nodeList != null)
            {
                foreach (var fxPair in nodeList)
                {
                    var fxNode = fxPair.node;
                    var fxNodeLv = fxPair.level;
                    if (fxNodeLv > lv)
                    {
                        fxNode.SetActive(false);
                    }
                }
            }


            level = lv;
        }

        public void Upgrade(int lv, GameObject prefab)
        {
            lv = FixLevel(lv);

            if (lv == level)
                return;

            if (prefab == null)
                return;

            var ctrl = prefab.GetComponent<EffectLevelController>();
            if (ctrl == null)
                return;

            //填充
            XGameObjectTools.UnionGameObject(gameObject, prefab, (fxGO) =>
            {
                //新或旧的节点
                fxGO.SetActive(true);
            });


            //更新节点信息
            if (ctrl.nodeList != null)
            {
                nodeList = (nodeList != null) ? nodeList : new List<EffectLevelInfo>();
                nodeList.Clear();
                foreach (var fxPair in ctrl.nodeList)
                {
                    var fxNodePath = XGameObjectTools.GetPathByGameObject(fxPair.node, prefab);
                    var newNode = XGameObjectTools.GetGameObjectByPath(gameObject, fxNodePath);
                    if (newNode != null)
                    {
                        var info = new EffectLevelInfo();
                        info.node = newNode;
                        info.level = fxPair.level;

                        nodeList.Add(info);
                    }
                }
                SortList();
            }

            level = lv;
        }

        public void SortList()
        {
            if (nodeList != null)
            {
                nodeList.Sort(delegate (EffectLevelInfo a, EffectLevelInfo b)
                {
                    if (a.node && b.node)
                    {
                        if (a.level == b.level)
                        {
                            string aPath = XGameObjectTools.GetPathByGameObject(a.node, gameObject);
                            string bPath = XGameObjectTools.GetPathByGameObject(b.node, gameObject);
                            return string.Compare(aPath, bPath);
                        }
                    }
                    return a.level - b.level;
                });
            }
        }

        private int FixLevel(int lv)
        {
            int okLv = lv;
            okLv = Mathf.Min(10, okLv);
            okLv = Mathf.Max(1, okLv);

            return okLv;
        }
    }
}