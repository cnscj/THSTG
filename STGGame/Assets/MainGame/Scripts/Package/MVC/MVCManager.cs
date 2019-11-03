using System.Collections.Generic;
using XLibrary.Package;

namespace STGGame.MVC
{
    public class MVCManager : Singleton<MVCManager>
    {
        private Dictionary<string, Controller> m_controllerMaps = new Dictionary<string, Controller>();
        private Dictionary<string, Cache> m_cacheMaps = new Dictionary<string, Cache>();

        //Controller部分
        public T AddController<T>() where T : Controller, new()
        {
            T controller = new T();
            bool ret = controller.Initialize();
            if (ret)
            {
                string name = typeof(T).Name;
                m_controllerMaps[name] = controller;
            }
            return controller;
        }

        public void RemoveController<T>()
        {
            string name = typeof(T).Name;
            Controller controller = null;
            if (m_controllerMaps.TryGetValue(name,out controller))
            {
                controller.Dispose();
                m_controllerMaps.Remove(name);
            }
            
        }

        public T GetController<T>() where T : Controller, new()
        {
            string name = typeof(T).Name;
            Controller ctrl = null;
            if (!m_controllerMaps.TryGetValue(name,out ctrl))
            {
                ctrl = AddController<T>();
            }
            return ctrl as T;
        }

        //Cache部分
        public T AddCache<T>() where T : Cache, new()
        {
            string name = typeof(T).Name;
            T cache = new T();
            m_cacheMaps[name] = cache;
            
            return cache;
        }

        public void RemoveCache<T>()
        {
            Cache controller = null;
            string name = typeof(T).Name;
            if (m_cacheMaps.TryGetValue(name, out controller))
            {
                controller.Clear();
                m_cacheMaps.Remove(name);
            }
        }
        public T GetCache<T>() where T : Cache, new()
        {
            string name = typeof(T).Name;
            Cache cache = null;
            if (!m_cacheMaps.TryGetValue(name, out cache))
            {
                cache = AddCache<T>();
            }
            return cache as T;
        }

        public void ClearAllCaches()
        {
            foreach(var pair in m_cacheMaps)
            {
                pair.Value.Clear();
            }
        }

        public void ResetAllCaches()
        {
            m_cacheMaps.Clear();
        }
       

    }
}
