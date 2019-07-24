using UnityEngine;
using System.Collections;
using THGame.Package;
using System.Collections.Generic;

namespace THEditor
{
    public class BundleBuilderManager : Singleton<BundleBuilderManager>
    {
        //不同平台下打各自的包
        //公共的抽出来打一个(就是依赖数>1的
        private List<BundleBuilder> m_builders = new List<BundleBuilder>();

        public BundleBuilderManager(BundleBuilder [] builders)
        {
            foreach (var builer in builders)
            {
                m_builders.Add(builer);
            }
        }
        public void BuildAll()
        {
            foreach (var builer in m_builders)
            {
                builer.Build();
            }

            foreach (var info in BundleBuilderConfig.GetInstance().buildInfoList)
            {
                string srcName = info.srcName.ToLower();
                if (srcName == "shader" || srcName == "shaders")
                {
                    BuildShader(info);
                }
                else
                {
                    BuildOnce(info);
                }
            }

        }

        void BuildOnce(BundleBuilderConfig.BundleBuilderInfos info)
        {


        }

        void BuildShader(BundleBuilderConfig.BundleBuilderInfos info)
        {


        }

        void BuildCommon()
        {


        }

        void BuildShare()
        {


        }
    }
}

