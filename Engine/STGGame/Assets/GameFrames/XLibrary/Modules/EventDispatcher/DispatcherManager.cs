using System.Collections.Generic;
using XLibrary.Package;
namespace XLibGame
{
    public class DispatcherManager : Singleton<DispatcherManager>
    {
        private Dictionary<string, Dispatcher> m_dispatchersMap;
        protected DispatcherManager()
        {
            m_dispatchersMap = new Dictionary<string, Dispatcher>();
        }

        public Dispatcher NewDispatcher(string name = null)
        {
            name = string.IsNullOrEmpty(name) ? string.Format("Dispatcher_{0}", m_dispatchersMap.Count) : name;
            Dispatcher dispatcher = new Dispatcher();
            m_dispatchersMap[name] = dispatcher;
            return dispatcher;
        }

        public Dispatcher GetDispatcher(string name)
        {
            Dispatcher dispatcher = null;
            if (!m_dispatchersMap.TryGetValue(name, out dispatcher))
            {
                return dispatcher;
            }
            return null;
        }

        public Dispatcher GetOrNewDispatcher(string name)
        {
            Dispatcher dispatcher = null;
            if (!m_dispatchersMap.TryGetValue(name,out dispatcher))
            {
                dispatcher = new Dispatcher();
                m_dispatchersMap.Add(name, dispatcher);
            }
            return dispatcher;
        }

    }

}
