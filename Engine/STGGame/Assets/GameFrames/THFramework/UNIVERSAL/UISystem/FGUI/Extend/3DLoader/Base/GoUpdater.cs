using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace THGame.UI
{
    public class GoUpdater
    {
        private List<IGoUpdate> _updaters;
        private GoWrapper _goWrapper;
        private GoUpdateContext _updateContext;

        public void AddUpdater(IGoUpdate iUpdater)
        {
            if (iUpdater == null)
                return;

            GetUpdaters().Add(iUpdater);
        }

        public void RemoveUpdater(IGoUpdate iUpdater)
        {
            if (_updaters == null)
                return;

            GetUpdaters().Remove(iUpdater);
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

            foreach (var updater in _updaters)
            {
                updater.Update(_updateContext);
            }
        }
        private GoUpdateContext GetOrCreateContext()
        {
            _updateContext = _updateContext ?? new GoUpdateContext();
            return _updateContext;
        }

        private List<IGoUpdate> GetUpdaters()
        {
            _updaters = _updaters ?? new List<IGoUpdate>();
            return _updaters;
        }
    }

}

