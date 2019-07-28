using UnityEngine;
using System.Collections;
using THGame.Package;
using System.Collections.Generic;
using UnityEditor;

namespace THEditor
{
    public class BundleBuilderManager : Singleton<BundleBuilderManager>
    {
        //不同平台下打各自的包
        //公共的抽出来打一个(就是依赖数>1的
        private List<BundleBuilder> m_builders = new List<BundleBuilder>();
        private List<BundleBuilder> m_defaultBuilders = new List<BundleBuilder>();

        public BundleBuilderManager(BundleBuilder [] builders)
        {
            foreach (var builer in builders)
            {
                m_builders.Add(builer);
            }
        }
        public void BuildAll()
        {
            m_defaultBuilders.Clear();
            foreach (var info in BundleBuilderConfig.GetInstance().buildInfoList)
            {
                string srcName = info.srcName.ToLower();
                if (srcName == "shader" || srcName == "shaders")
                {
                    m_defaultBuilders.Add(new BundleBuilderShader(info));
                }
                else
                {
                    m_defaultBuilders.Add(new BundleBuilderDefault(info));
                }
            }

            foreach (var builer in m_builders)
            {
                builer.Build();
            }

            foreach (var builer in m_defaultBuilders)
            {
                builer.Build();
            }
        }
    }
}

