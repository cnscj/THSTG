using System.Collections;
using System.Collections.Generic;
using THGame.Package;
using UnityEngine;
namespace THEditor
{
    public class PostProcesserManager
    {
        private List<PostProcesser> m_processers = new List<PostProcesser>();

        public PostProcesserManager()
        {

        }
        public PostProcesserManager(PostProcesser[] list)
        {
            foreach(var pcer in list)
            {
                m_processers.Add(pcer);
            }
        }

        public PostProcesser AddProcesser<T>() where T: PostProcesser,new()
        {
            T processer = new T();
            m_processers.Add(processer);
            return processer;
        }

        public void Process()
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

