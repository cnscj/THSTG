using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public class NodeEffectController : MonoBehaviour
    {
        public List<NodeEffectData> datas;
        public GameObject attacher;
        public new string tag;

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

        public void Attach(GameObject go)
        {
            if (go == null)
                return;

            if (datas == null)
                return;

            Demount();

            gameObject.transform.SetParent(go.transform, false);
            foreach (var info in datas)
            {
                if (info.node != null && !string.IsNullOrEmpty(info.path))
                {
                    var nodeParentPath = Path.GetDirectoryName(info.path);
                    var parentNode = XGameObjectTools.GetGameObjectByPath(gameObject, nodeParentPath);

                    if (parentNode != null)
                    {
                        info.node.transform.SetParent(parentNode.transform, false);
                        info.node.SetActive(true);
                    }
                    else
                    {
                        info.node.SetActive(false);
                    }
                }
            }
            attacher = go;
        }

        public void Demount()
        {
            if (attacher == null)
                return;

            if (datas == null)
                return;

            foreach (var info in datas)
            {
                if (info.node != null)
                {
                    info.node.transform.SetParent(gameObject.transform, false);
                }
            }

            attacher = null;
        }

        public void Destroy()
        {
            Object.Destroy(this);
        }

        public void Show(bool val)
        {
            if (datas == null)
                return;

            foreach (var info in datas)
            {
                if (info.node != null)
                {
                    info.node.SetActive(val);
                }
            }
        }

        private void OnDestroy()
        {
            if (datas == null)
                return;

            foreach (var info in datas)
            {
                if (info.node != null)
                {
                    Object.Destroy(info.node);
                }
            }
        }
    }

}
