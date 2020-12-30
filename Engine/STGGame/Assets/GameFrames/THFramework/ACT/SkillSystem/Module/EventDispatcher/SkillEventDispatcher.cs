using System;
using System.Collections.Generic;

namespace THGame
{
    public class SkillEventDispatcher
    {
        private Dictionary<IComparable, HashSet<SkillEventListener>> _listeners = new Dictionary<IComparable, HashSet<SkillEventListener>>();

        public void AddEventListener(IComparable type, SkillEventListener listener)
        {
            HashSet<SkillEventListener> listenerSet = GetOrCreateListenerSet(type);
            if (listenerSet != null)
            {
                if (!listenerSet.Contains(listener))
                {
                    listenerSet.Add(listener);
                }
            }
        }

        public void RemoveEventListener(IComparable type, SkillEventListener listener)
        {
            _listeners.TryGetValue(type, out HashSet<SkillEventListener> listenerSet);
            if (listenerSet != null)
            {
                if (!listenerSet.Contains(listener))
                {
                    listenerSet.Remove(listener);
                    if (listenerSet.Count <= 0)
                    {
                        _listeners.Remove(type);
                    }
                }
            }
        }

        public void DispatchEvent(IComparable type, params object[] args)
        {
            _listeners.TryGetValue(type, out HashSet<SkillEventListener> listenerSet);
            if (listenerSet != null)
            {
                var context = new SkillEventContext();
                context.type = type;
                context.args = args;

                foreach(var listener in listenerSet)
                {
                    listener.Invoke(context);
                }
            }
        }

        private HashSet<SkillEventListener> GetOrCreateListenerSet(IComparable type)
        {
            if (type == null) return default;

            if (!_listeners.TryGetValue(type, out HashSet<SkillEventListener> skillEventListeners))
            {
                skillEventListeners = new HashSet<SkillEventListener>();
                _listeners[type] = skillEventListeners;
            }
            return skillEventListeners;
        }

    }
}
