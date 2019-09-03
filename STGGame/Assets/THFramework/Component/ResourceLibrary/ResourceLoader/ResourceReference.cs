using UnityEngine;

namespace THGame
{
    public class ResourceReference
    {
        private ResourceLoadParams m_args;
        public ResourceReference(ResourceLoadParams args)
        {
            m_args = args;

        }

        public ResourceLoadHandle<T> LoadAsnyc<T>() where T : Object
        {
            return ResourceLoader.GetInstance().LoadFromFileAsync<T>(m_args);
        }

        public T Load<T>() where T: Object
        {

            return ResourceLoader.GetInstance().LoadFromFile<T>(m_args);
        }
    }

}
