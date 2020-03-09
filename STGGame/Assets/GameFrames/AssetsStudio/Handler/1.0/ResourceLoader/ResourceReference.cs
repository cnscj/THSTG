using UnityEngine;

namespace ASGame
{
    public class ResourceReference
    {
        private ResourceLoadParams m_args;
        public ResourceReference(ResourceLoadParams args)
        {
            m_args = args;

        }

        public ResourceLoadHandler<T> LoadAsnyc<T>() where T : Object
        {
            return ResourceLoader.GetInstance().LoadFromFileAsync<T>(m_args);
        }

        public T Load<T>() where T: Object
        {

            return ResourceLoader.GetInstance().LoadFromFile<T>(m_args);
        }
    }

}
