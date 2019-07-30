using System;
using System.Collections.Generic;

namespace THGame.Package.MVC
{
    public class MVCManager
    {
        private static MVCManager m_instance;

        private Dictionary<string, Controller> controllerMaps = new Dictionary<string, Controller>();


        public static MVCManager GetInstance()
        {
            if (m_instance == null)
            {
                m_instance = new MVCManager();
            }
            return m_instance;
        }

        public Controller AddController<T>(string name) where T : Controller, new()
        {
            T controller = new T();
            bool ret = controller.Initialize();
            if (ret)
            {
                controllerMaps.Add(name, controller);
                return controller;
            }

            return null;
        }

        public void RemoveController(string name)
        {
            controllerMaps.Remove(name);
        }

        public Controller GetController(string name)
        {
            return controllerMaps[name];
        }
    }
}
