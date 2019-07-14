using System;
using System.Collections.Generic;

namespace THGame
{
    namespace MVC
    {
        public class MVCManager
        {
            private Dictionary<string, Module> moduleMaps = new Dictionary<string, Module>();
            public Module AddModule<T>(string name) where T : Module, new()
            {
                T module = new T();
                bool ret = module.Init();
                if (ret)
                {
                    moduleMaps.Add(name, module);
                    return module;
                }
                
                return null;
            }

            public void RemoveModule(string name)
            {
                moduleMaps.Remove(name);
            }

            public Module getModule(string name)
            {
                return moduleMaps[name];
            }
        }
    }
}
