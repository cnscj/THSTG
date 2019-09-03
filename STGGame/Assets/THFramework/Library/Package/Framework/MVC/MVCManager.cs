using System;
using System.Collections.Generic;

namespace THGame.Package.MVC
{
    public class MVCManager : Singleton<MVCManager>
    {
        private Dictionary<string, Controller> m_controllerMaps = new Dictionary<string, Controller>();
        private Dictionary<string, Cache> m_cacheMaps = new Dictionary<string, Cache>();

        //Controller部分
        public T AddController<T>(string name = null) where T : Controller, new()
        {
            T controller = new T();
            bool ret = controller.Initialize();
            if (ret)
            {
                name = name == null ? controller.GetType().Name : name;
                m_controllerMaps.Add(name, controller);
                return controller;
            }

            return null;
        }

        public void RemoveController(string name)
        {
            Controller controller = null;
            if (m_controllerMaps.TryGetValue(name,out controller))
            {
                controller.Dispose();
                m_controllerMaps.Remove(name);
            }
            
        }

        public T GetController<T>(string name) where T : Controller
        {
            return (T)GetController(name);
        }

        public Controller GetController(string name)
        {
            return m_controllerMaps[name];
        }

        //Cache部分
        public T AddCache<T>(string name = null) where T : Cache, new()
        {
            T cache = new T();
            if (cache != null)
            {
                name = name == null ? cache.GetType().Name : name;
                m_cacheMaps.Add(name, cache);
                return cache;
            }

            return null;
        }

        public void RemoveCache(string name)
        {
            Cache controller = null;
            if (m_cacheMaps.TryGetValue(name, out controller))
            {
                controller.Clear();
                m_cacheMaps.Remove(name);
            }

        }
        public T GetCache<T>(string name) where T : Cache
        {
            return (T)GetCache(name);
        }
        public Cache GetCache(string name)
        {
            return m_cacheMaps[name];
        }

        public void ClearAllCache()
        {
            foreach(var pair in m_cacheMaps)
            {
                pair.Value.Clear();
            }
        }
    }
}
