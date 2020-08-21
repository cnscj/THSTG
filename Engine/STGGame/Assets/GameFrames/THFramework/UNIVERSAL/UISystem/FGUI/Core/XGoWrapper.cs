using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class XGoWrapper : GoWrapper
    {
        private Dictionary<string, GoBaseUpdater> _updaters;
        private GoUpdateContext _updateContext;

        public T AddUpdater<T>() where T : GoBaseUpdater, new()
        {
            var updaterKey = GetUpdaterKey<T>();
            if (_updaters != null)
            {
                if (_updaters.ContainsKey(updaterKey))
                {
                    return _updaters[updaterKey] as T;
                }
            }

            T iUpdater = new T();
            if (iUpdater == null)
                return default;

            GetUpdaters().Add(updaterKey, iUpdater);
            return iUpdater;
        }

        public T GetUpdater<T>() where T : GoBaseUpdater
        {
            if (_updaters == null)
                return default;

            var updaterKey = GetUpdaterKey<T>();
            if (_updaters.TryGetValue(updaterKey, out var iUpdater))
            {
                return iUpdater as T;
            }
            return default;
        }

        public void RemoveUpdater<T>() where T : GoBaseUpdater
        {
            if (_updaters == null)
                return;

            var updaterKey = GetUpdaterKey<T>();
            GetUpdaters().Remove(updaterKey);
        }

        public void RemoveAllUpdater()
        {
            if (_updaters == null)
                return;

            _updaters.Clear();
        }

        override public void Update(UpdateContext context)
        {
            UpdateContext(context);
            base.Update(context);
        }

        virtual public void Reset()
        {
            if (_updaters == null || _updaters.Count < 0)
                return;

            foreach (var updater in _updaters.Values)
            {
                updater.Reset();
            }
        }

        private void UpdateContext(UpdateContext context)
        {
            if (_updaters == null || _updaters.Count < 0)
                return;

            var updateContext = GetOrCreateContext();
            updateContext.wrapperTarget = wrapTarget;
            updateContext.wrapperContext = context;

            foreach (var updater in _updaters.Values)
            {
                updater.Update(_updateContext);
            }
        }

        private GoUpdateContext GetOrCreateContext()
        {
            _updateContext = _updateContext ?? new GoUpdateContext();
            return _updateContext;
        }

        private string GetUpdaterKey<T>()
        {
            var type = typeof(T);
            return type.FullName;
        }

        private Dictionary<string, GoBaseUpdater> GetUpdaters()
        {
            _updaters = _updaters ?? new Dictionary<string, GoBaseUpdater>();
            return _updaters;
        }
    }
}
