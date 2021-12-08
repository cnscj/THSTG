using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    //NOTE:一般不去使用他,
    public class ModelEffectLoader : MonoBehaviour
    {
        public string key;
        private List<GameObject> m_effectNodes;
        private bool m_isSetuped;
        private ModelEffect m_curModelEffect;

        public void Setup(GameObject modfxGo, bool isUseFullPath, bool isSetLayer = true)
        {
            if (modfxGo == null)
                return;

            var modfx = modfxGo.GetComponent<ModelEffect>();
            Setup(modfx, isUseFullPath, isSetLayer);
        }

        public bool HadSetuped()
        {
            return m_isSetuped;
        }

        public bool Setup(ModelEffect modfx ,bool isUseFullPath = true, bool isSetLayer = true)
        {
            if (modfx == null)
                return false;

            if (modfx.nodeInfos == null)
                return false;

            Unsetup();
            foreach (var nodeInfo in modfx.nodeInfos)
            {
                string nodeParentPath = Path.GetDirectoryName(nodeInfo.nodePath);
                nodeParentPath = isUseFullPath ? nodeParentPath : Path.GetFileName(nodeParentPath);

                Transform targetParentTrans = gameObject.transform.Find(nodeParentPath);
                if (targetParentTrans != null)
                {
                    var nodeEffect = Object.Instantiate(nodeInfo.nodeEffect, targetParentTrans, false);
                    nodeEffect.name = nodeEffect.name.Replace("(Clone)", "");

                    m_effectNodes = m_effectNodes ?? new List<GameObject>();
                    m_effectNodes.Add(nodeEffect);

                    if (isSetLayer)
                    {
                        XGameObjectTools.SetLayer(nodeEffect, gameObject);
                    }
                }
            }
            key = modfx.name;
            m_isSetuped = true;
            return true;

        }

        public void Unsetup()
        {
            if (m_effectNodes != null)
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
                key = null;
            }

            if (m_curModelEffect != null)
            {
                Object.Destroy(m_curModelEffect.gameObject);
                m_curModelEffect = null;
            }
        }

        //将特效节点打包回ModelEffect
        public ModelEffect Package()
        {
            ModelEffect modelEffect = null;
            if (m_curModelEffect == null)//这种方式,长度,级别基本都丢失了
            {
                if (m_effectNodes == null)
                    return null;

                string modfxName = string.Format("{0}_modfx", gameObject.name);
                GameObject fxInstance = new GameObject(modfxName);

                modelEffect = fxInstance.AddComponent<ModelEffect>();
                modelEffect.nodeInfos = new List<ModelEffect.NodeInfo>();

                foreach (var effectGo in m_effectNodes)
                {
                    if (effectGo != null)
                    {
                        var metadate = new ModelEffect.NodeInfo();

                        var nodePath = XGameObjectTools.GetPathByGameObject(effectGo, gameObject);
                        metadate.nodePath = nodePath;
                        metadate.nodeEffect = effectGo;

                        modelEffect.nodeInfos.Add(metadate);

                        effectGo.transform.SetParent(fxInstance.transform, false);
                    }
                }

                m_effectNodes.Clear();
            }
            else
            {
                modelEffect = m_curModelEffect;
                if (modelEffect.nodeInfos != null)
                {
                    foreach (var nodePair in modelEffect.nodeInfos)
                    {
                        var nodeEffect = nodePair.nodeEffect;
                        if (nodeEffect != null)
                        {
                            nodeEffect.transform.SetParent(modelEffect.gameObject.transform, false);
                        }
                    }
                }
                m_curModelEffect = null;
            }

            return modelEffect;
        }

        //解包
        public bool Unpackage(ModelEffect modelEffect, bool isUseFullPath = true, bool isSetLayer = false)
        {
            if (modelEffect == null)
                return false;

            Unsetup();
            foreach (var nodeInfo in modelEffect.nodeInfos)
            {
                var newEffectGO = nodeInfo.nodeEffect;
                if (newEffectGO != null)
                {
                    string nodeParentPath = Path.GetDirectoryName(nodeInfo.nodePath);
                    nodeParentPath = isUseFullPath ? nodeParentPath : Path.GetFileName(nodeParentPath);

                    var parentNode = gameObject.transform.Find(nodeParentPath);
                    if (parentNode != null)
                    {
                        newEffectGO.transform.SetParent(parentNode.transform, false);
                        newEffectGO.name = newEffectGO.name.Replace("(Clone)", "");
                        newEffectGO.SetActive(true);

                        m_effectNodes = m_effectNodes ?? new List<GameObject>();
                        m_effectNodes.Add(newEffectGO);

                        if (isSetLayer)
                        {
                            XGameObjectTools.SetLayer(newEffectGO, gameObject);
                        }
                    }
                    else
                    {
                        newEffectGO.SetActive(false);
                    }
                }
            }

            modelEffect.transform.SetParent(transform.parent, false);
            m_curModelEffect = modelEffect;

            return true;
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
