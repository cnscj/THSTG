using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public class ViewEffectLoader : MonoBehaviour
    {
        public List<NodeEffectData> datas;

        public List<string> GetPaths()
        {
            if (datas == null)
                return null;

            List<string> pathList = new List<string>();
            foreach (var info in datas)
            {
                pathList.Add(info.path);
            }
            return pathList;
        }

        public void Setup(GameObject fxPackage, bool isReplaceMode = true)
        {
            if (fxPackage == null)
                return;

            var fxCtrl = fxPackage.GetComponent<NodeEffectController>();
            if (fxCtrl == null)
                return;

            if (fxCtrl.datas == null)
                return;

            if (isReplaceMode)
            {
                Unsetup();
            }

            datas = datas ?? new List<NodeEffectData>();

            foreach (var data in fxCtrl.datas)
            {
                if (data.node != null && !string.IsNullOrEmpty(data.path))
                {
                    var parentNodePath = Path.GetDirectoryName(data.path);
                    var parentNode = XGameObjectTools.GetGameObjectByPath(gameObject, parentNodePath);

                    GameObject newFxNode = null;
                    if (isReplaceMode)
                    {
                        newFxNode = Object.Instantiate(data.node, parentNode.transform, false);
                    }
                    else
                    {
                        var curFxNode = XGameObjectTools.GetGameObjectByPath(gameObject, data.path);
                        if (curFxNode != null)
                        {
                            newFxNode = curFxNode;
                            newFxNode.SetActive(true);
                        }
                        else
                        {
                            newFxNode = Object.Instantiate(data.node, parentNode.transform, false);
                        }
                    } 
                        
                    newFxNode.name = newFxNode.name.Replace("(Clone)", "");

                    var newData = new NodeEffectData();
                    newData.node = newFxNode;
                    newData.path = Path.Combine(parentNodePath, newFxNode.name);

                    datas.Add(newData);
                }
            }
        }

        [ContextMenu("Unsetup")]
        public void Unsetup()
        {
            if (datas == null)
                return;

            foreach (var data in datas)
            {
                if (data.node != null)
                {
                    Object.Destroy(data.node);
                }
            }
            datas.Clear();
        }

        public void Show(bool val)
        {
            if (datas == null)
                return;

            foreach (var data in datas)
            {
                if (data.node != null)
                {
                    data.node.SetActive(val);
                }
            }
        }
    }

}
