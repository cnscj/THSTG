using System.Collections.Generic;
using UnityEngine;

namespace ASEditor
{
    public class ResourceConfig : BaseResourceConfig<ResourceConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/ASRsourceConfig.asset"));

        [SerializeField] public string resModulePath = "";
        [SerializeField] public List<ResourcePathConfigInfos> resPathList = new List<ResourcePathConfigInfos>();
        private Dictionary<string, ResourcePathConfigInfos> m_resPathMaps;


        //取得配置路径
        public ResourcePathConfigInfos GetPathConfig(string key)
        {
            var maps = GetPathMap();
            return maps[key];
        }

        private Dictionary<string, ResourcePathConfigInfos> GetPathMap()
        {
            if (m_resPathMaps == null)
            {
                m_resPathMaps = new Dictionary<string, ResourcePathConfigInfos>();
                foreach(var infos in resPathList)
                {
                    if (infos.key != null && infos.key != "")
                    {
                        if (!m_resPathMaps.ContainsKey(infos.key))
                        {
                            m_resPathMaps.Add(infos.key, infos);
                        }
                    }

                }
            }
            return m_resPathMaps;
        }

        private void OnEnable()
        {
            var maps = GetPathMap();
            maps.Clear();
            foreach (var infos in resPathList)
            {
                if (infos.key != null && infos.key != "")
                {
                    if (!m_resPathMaps.ContainsKey(infos.key))
                    {
                        m_resPathMaps.Add(infos.key, infos);
                    }
                }

            }
        }
    }

}
