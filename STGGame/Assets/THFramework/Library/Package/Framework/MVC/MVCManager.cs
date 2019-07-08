using System;
using System.Collections.Generic;

namespace THGame
{
    namespace MVC
    {
        public class MVCManager
        {
            private Dictionary<string, Controller> controllerMaps = new Dictionary<string, Controller>();
            public Controller AddController<T>(string name) where T : Controller, new()
            {
                T controller = new T();
                controllerMaps.Add(name, controller);
                return null;
            }

            public void RemoveController(string name)
            {
                controllerMaps.Remove(name);
            }
        }
    }
}
