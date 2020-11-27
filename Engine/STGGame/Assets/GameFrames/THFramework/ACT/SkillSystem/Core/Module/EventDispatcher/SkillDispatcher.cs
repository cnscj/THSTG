using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame
{
    public class SkillDispatcher
    {
        private Dictionary<string, HashSet<SkillEventListener>> _listeners = new Dictionary<string, HashSet<SkillEventListener>>();

        public void AddEventListener(string type, SkillEventListener listener)
        {
            HashSet<SkillEventListener> listenerSet = null;
            listenerSet = GetOrCreateListenerSet(type);
            if (listenerSet != null)
            {
                if (!listenerSet.Contains(listener))
                {
                    listenerSet.Add(listener);
                }
            }
        }

        public void RemoveEventListener(string type, SkillEventListener listener)
        {
            HashSet<SkillEventListener> listenerSet = null;
            _listeners.TryGetValue(type, out listenerSet);
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

        public void DispatchEvent(object sender, string type, object args = null)
        {
            HashSet<SkillEventListener> listenerSet = null;
            _listeners.TryGetValue(type, out listenerSet);
            if (listenerSet != null)
            {
                var context = new SkillEventContext();
                context.sender = sender;
                context.type = type;
                context.args = args;

                foreach(var listener in listenerSet)
                {
                    listener.Invoke(context);
                }
            }
        }

        private HashSet<SkillEventListener> GetOrCreateListenerSet(string type)
        {
            if (string.IsNullOrEmpty(type))
                return default;

            HashSet<SkillEventListener> skillEventListeners = null;
            if (!_listeners.TryGetValue(type,out skillEventListeners))
            {
                skillEventListeners = new HashSet<SkillEventListener>();
                _listeners[type] = skillEventListeners;
            }
            return skillEventListeners;
        }

    }
}
