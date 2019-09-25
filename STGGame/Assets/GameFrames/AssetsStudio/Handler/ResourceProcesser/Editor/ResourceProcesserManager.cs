using System.Collections.Generic;
using UnityEditor.SceneManagement;
using XLibrary;

namespace ASEditor
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
            string curScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

            foreach (var process in m_processers)
            {
                process.Do();
            }

            if (XFileTools.Exists(curScenePath))
            {
                EditorSceneManager.OpenScene(curScenePath);
            }
        }

        public void RemoveAllProcesser()
        {
            m_processers.Clear();
        }
    }

}

