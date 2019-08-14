using System.Collections;
using System.Collections.Generic;
using THGame.Package;
using UnityEngine;
namespace THEditor
{
    public class ResourceProcesserManager
    {
        private List<ResourceProcesser> m_processers = new List<ResourceProcesser>();

        public ResourceProcesserManager()
        {

        }
        public ResourceProcesserManager(ResourceProcesser[] list)
        {
            foreach(var pcer in list)
            {
                m_processers.Add(pcer);
            }
        }

        public ResourceProcesser AddProcesser<T>() where T: ResourceProcesser,new()
        {
            T processer = new T();
            m_processers.Add(processer);
            return processer;
        }

        public void ProcessAll()
        {
            foreach(var process in m_processers)
            {
                process.Do();
            }
        }

        public void RemoveAllProcesser()
        {
            m_processers.Clear();
        }
    }

}

