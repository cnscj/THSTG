using System.Collections;
using System.Collections.Generic;
using ASGame;
using UnityEditor;
using UnityEngine;
using XLibrary;

namespace ASEditor
{
    public static class NodeEffectEditorTool
    {
        public static bool PackageNodeEffect(GameObject tmplGo, GameObject tager)
        {
            if (tmplGo == null)
                return false;

            tager = tager ?? tmplGo;

            List<GameObject> fxNodes = new List<GameObject>();
            foreach(var nodeTrans in tmplGo.GetComponentsInChildren<Transform>())
            {
                if (nodeTrans == tmplGo)
                    continue;

                if (nodeTrans.name.ToLower().Contains(NodeEffectConfig.EFFECT_NODE_NAME.ToLower()))
                {
                    fxNodes.Add(nodeTrans.gameObject);
                }
            }

            if (fxNodes.Count > 0)
            {
                var nodeFxCtrl = tager.GetComponent<NodeEffectController>();
                if (nodeFxCtrl != null) Object.Destroy(nodeFxCtrl);
                nodeFxCtrl = tager.AddComponent<NodeEffectController>();

                int index = 0;
                nodeFxCtrl.datas = nodeFxCtrl.datas ?? new List<NodeEffectData>();
                foreach(var fxNode in fxNodes)
                {
                    GameObject newFxNode = null;
                    if (tager == tmplGo)
                    {
                        newFxNode = fxNode;
                    }
                    else
                    {
                        newFxNode = Object.Instantiate(fxNode);
                        newFxNode.name = newFxNode.name.Replace("(Clone)", "");
                        newFxNode.name = string.Format("{0}({1})", newFxNode.name, index++);
                    }
                    newFxNode.transform.SetParent(tager.transform, false);
                    var fxNodePath = XGameObjectTools.GetPathByGameObject(fxNode, tmplGo);

                    var info = new NodeEffectData();
                    info.node = newFxNode;
                    info.path = fxNodePath;

                    nodeFxCtrl.datas.Add(info);
                }
                return true;
            }

            return false;
        }

        public static GameObject SaveNodeEffectPackagePrefab(GameObject tmplGo, string savePath)
        {
            GameObject outGO = null;
            if (tmplGo)
            {

                GameObject packageGo = new GameObject();

                PackageNodeEffect(tmplGo, packageGo);

                outGO = PrefabUtility.SaveAsPrefabAsset(packageGo, savePath);
                Object.DestroyImmediate(packageGo);
            }
            return outGO;
        }
    }
}

