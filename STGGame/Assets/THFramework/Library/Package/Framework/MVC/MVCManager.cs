using System;
using System.Collections.Generic;

namespace THGame.Package.MVC
{
    public class MVCManager
    {
        private Dictionary<string, Controller> controllerMaps = new Dictionary<string, Controller>();

        public Controller AddController<T>(string name = null) where T : Controller, new()
        {
            T controller = new T();
            bool ret = controller.Initialize();
            if (ret)
            {
                name = name == null ? controller.GetType().Name : name;
                controllerMaps.Add(name, controller);
                return controller;
            }

            return null;
        }

        public void RemoveController(string name)
        {
            Controller controller = null;
            if (controllerMaps.TryGetValue(name,out controller))
            {
                controller.Clear();
                controllerMaps.Remove(name);
            }
            
        }

        public Controller GetController(string name)
        {
            return controllerMaps[name];
        }
    }
}
