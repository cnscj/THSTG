
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class ModelTexturesChanger : MonoBehaviour
    {
        private static string DEFAULT_KEY = "default";
        [System.Serializable]
        public class TexInfo
        {
            public string name;
            public Texture mainTex;
            public Texture flowTex;
        }
        public List<TexInfo> texInfoList;

        private TexInfo m_curTexInfo;
        private Dictionary<string, TexInfo> m_texInfoMap;

        public bool Contains(string key)
        {
            if (m_texInfoMap == null)
                return false;

            return m_texInfoMap.ContainsKey(key);
        }

        public bool Add(TexInfo info, bool isReplace = false)
        {
            if (info == null)
                return false;

            if (string.IsNullOrEmpty(info.name))
                return false;

            var texMaps = GetOrCreateMaps();
            if (texMaps.ContainsKey(info.name))
            {
                if (isReplace)
                {
                    texMaps[info.name] = info;
                    return true;
                }
                return false;
            }

            texMaps.Add(info.name, info);

            return true;
        }

        public TexInfo GetTexInfo(string name)
        {
            if (m_texInfoMap != null)
            {
                if (m_texInfoMap.ContainsKey(name))
                {
                    return m_texInfoMap[name];
                }
            }
            return null;
        }

        public string Change(string name)
        {
            var texMaps = GetOrCreateMaps();
            if (texMaps.ContainsKey(name))
            {
                var oldTexInfo = m_curTexInfo;
                var newTexInfo = texMaps[name];

                var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                {
                    Material[] materials = smr.sharedMaterials;
                    if (materials != null)
                    {
                        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                        for (int i = 0 ; i < materials.Length; i++)
                        {
                            materialPropertyBlock.Clear();
                            smr.GetPropertyBlock(materialPropertyBlock, i);

                            materialPropertyBlock.SetTexture("_MainTex", newTexInfo.mainTex);
                            if (newTexInfo.flowTex) materialPropertyBlock.SetTexture("_FlowTex", newTexInfo.flowTex);

                            smr.SetPropertyBlock(materialPropertyBlock, i);
                        }
                        
                        m_curTexInfo = newTexInfo;
                        return oldTexInfo.name;
                    }
                }
            }

            return m_curTexInfo.name;
        }

        [ContextMenu("Reset")]
        public void Reset()
        {
            Change(DEFAULT_KEY);
        }

        private TexInfo CreateDefaultTexInfo()
        {
            TexInfo defaultTexInfo = new TexInfo();
            defaultTexInfo.name = DEFAULT_KEY;
            var smr = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                Material[] materials = smr.sharedMaterials;

                if (materials != null)
                {
                    MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materialPropertyBlock.Clear();
                        smr.GetPropertyBlock(materialPropertyBlock, i);

                        defaultTexInfo.mainTex = materialPropertyBlock.GetTexture("_MainTex");
                        defaultTexInfo.flowTex = materialPropertyBlock.GetTexture("_FlowTex");
                        break;
                    }
                }
            }

            return defaultTexInfo;
        }

        private Dictionary<string, TexInfo> GetOrCreateMaps()
        {
            if (m_texInfoMap == null)
            {
                m_texInfoMap = new Dictionary<string, TexInfo>();
                if (texInfoList != null)
                {
                    foreach (var texInfo in texInfoList)
                    {
                        if (!m_texInfoMap.ContainsKey(texInfo.name))
                        {
                            m_texInfoMap.Add(texInfo.name, texInfo);
                        }
                    }
                }

                m_curTexInfo = CreateDefaultTexInfo();
                m_texInfoMap.Add(m_curTexInfo.name, m_curTexInfo);

            }
            return m_texInfoMap;
        }
    }

}
