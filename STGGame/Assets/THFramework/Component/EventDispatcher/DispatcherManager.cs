using System.Collections.Generic;
using THGame.Package;

namespace THGame
{
    public class DispatcherManager : MonoSingleton<DispatcherManager>
    {
        private Dispatcher s_dispatcher = new Dispatcher();

        public void Dispatch(int eventId, Dictionary<string, object> args = null)
        {
            s_dispatcher.Dispatch(eventId, args);
        }


        public void AddListener(int eventId, Dispatcher.EventListener listener, int priority = 1)
        {
            s_dispatcher.AddListener(eventId, listener, priority);
        }

        public void RemoveListener(int eventId, Dispatcher.EventListener listenerToBeRemoved)
        {
            s_dispatcher.RemoveListener(eventId, listenerToBeRemoved);
        }
    }
}
