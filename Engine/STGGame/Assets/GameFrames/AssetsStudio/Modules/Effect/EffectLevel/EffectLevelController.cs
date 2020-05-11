using System.Collections.Generic;
using System.IO;
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
            public string path;
            public int level;
        }
        public string code;
        public int level;   //当前级别,受升级影响
        public List<EffectLevelInfo> nodeList;

        public void Start()
        {
            EffectLevelManager.GetInstance().AddController(this);
        }

        public void OnDestroy()
        {
            EffectLevelManager.GetInstance().RemoveController(this);
        }

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

            if (ctrl.nodeList == null)
                return;

            nodeList = (nodeList != null) ? nodeList : new List<EffectLevelInfo>();
            foreach (var fxInfo in ctrl.nodeList)
            {
                var fxNode = fxInfo.node;
                if (fxNode == null)
                    continue;

                var fxNodePath = fxInfo.path;
                if (string.IsNullOrEmpty(fxNodePath))
                {
                    fxNodePath = XGameObjectTools.GetPathByGameObject(fxInfo.node, prefab);
                }

                var newNode = XGameObjectTools.GetGameObjectByPath(gameObject, fxNodePath);
                if (newNode != null)
                {
                    newNode.SetActive(true);
                }
                else
                {
                    string fxNodeParentPath = Path.GetDirectoryName(fxNodePath);
                    var newNodeParent = XGameObjectTools.GetGameObjectByPath(gameObject, fxNodePath);
                    if (newNodeParent != null)
                    {
                        var fxLevel = fxInfo.level;
                        newNode = Instantiate(fxNode, newNodeParent.transform, false);
                        var newInfo = new EffectLevelInfo();
                        newInfo.node = newNode;
                        newInfo.path = Path.Combine(fxNodeParentPath, newNode.name);
                        newInfo.level = fxLevel;

                        nodeList.Add(newInfo);
                    }
                }
            }

            level = lv;
        }

        public void SortList()
        {
            //保证顺序是从小到大,由浅到深
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