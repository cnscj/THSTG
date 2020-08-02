using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class GoUpdater
    {
        private Dictionary<string,GoBaseUpdater> _updaters;
        private GoWrapper _goWrapper;
        private GoUpdateContext _updateContext;

        public T AddUpdater<T>() where T : GoBaseUpdater , new()
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

            GetUpdaters().Add(updaterKey,iUpdater);
            return iUpdater;
        }

        public T GetUpdater<T>() where T : GoBaseUpdater
        {
            if (_updaters == null)
                return default;

            var updaterKey = GetUpdaterKey<T>();
            if (_updaters.TryGetValue(updaterKey,out var iUpdater))
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

        public void SetGoWrapper(GoWrapper wrapper)
        {
            if (_goWrapper != null)
            {
                _goWrapper.onUpdate -= OnUpdate;
            }

            _goWrapper = wrapper;
            _goWrapper.onUpdate += OnUpdate;
        }
        public GoWrapper GetGoWrapper()
        {
            return _goWrapper;
        }
        private void OnUpdate(UpdateContext context)
        {
            if (_goWrapper == null)
                return;

            if (_updaters == null || _updaters.Count < 0)
                return;

            var updateContext = GetOrCreateContext();
            updateContext.wrapperTarget = _goWrapper.wrapTarget;
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
            _updaters = _updaters ?? new Dictionary<string,GoBaseUpdater>();
            return _updaters;
        }
    }

}

