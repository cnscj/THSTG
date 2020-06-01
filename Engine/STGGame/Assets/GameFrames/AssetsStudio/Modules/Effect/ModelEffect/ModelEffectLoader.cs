using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public class ModelEffectLoader : MonoBehaviour
    {
        private List<GameObject> m_effectNodes;
        private bool m_isSetuped;
        public void Setup(GameObject modfxGo)
        {
            if (modfxGo == null)
                return;

            var modfx = modfxGo.GetComponent<ModelEffect>();
            Setup(modfx);
        }

        public bool HadSetuped()
        {
            return m_isSetuped;
        }

        public void Setup(ModelEffect modfx)
        {
            if (modfx == null)
                return;

            if (modfx.nodeInfos != null && modfx.nodeInfos.Count > 0)
            {
                Unsetup();
                foreach (var nodeInfo in modfx.nodeInfos)
                {
                    string nodeParentPath = Path.GetDirectoryName(nodeInfo.nodePath);
                    Transform targetParentTrans = gameObject.transform.Find(nodeParentPath);
                    if (targetParentTrans != null)
                    {
                        var nodeEffect = Object.Instantiate(nodeInfo.nodeEffect, targetParentTrans, false);
                        nodeEffect.name = nodeEffect.name.Replace("(Clone)", "");

                        m_effectNodes = m_effectNodes ?? new List<GameObject>();
                        m_effectNodes.Add(nodeEffect);
                    }
                }
                m_isSetuped = true;
            }
        }

        public void Unsetup()
        {
            if (m_effectNodes != null && m_effectNodes.Count > 0)
            {
                foreach (var nodeEffect in m_effectNodes)
                {
                    if (nodeEffect != null)
                    {
                        Object.Destroy(nodeEffect);
                    }
                }
                m_effectNodes.Clear();
                m_isSetuped = false;
            }
        }

        public void Show(bool val)
        {
            if (m_effectNodes != null && m_effectNodes.Count > 0)
            {
                foreach (var nodeEffect in m_effectNodes)
                {
                    if (nodeEffect != null)
                    {
                        nodeEffect.SetActive(val);
                    }
                }
            }
        }

        ///
        private void OnDestroy()
        {
            Unsetup();
        }
    }

}
