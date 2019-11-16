using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    public class MonoManager : MonoSingleton<MonoManager>
    {
        private MonoCallback _updateCallback;
        private MonoCallback _fixUpdateCallback;
        private MonoCallback _lateUpdateCallback;

        private void Update()
        {
            _updateCallback?.Invoke();
        }

        private void FixedUpdate()
        {
            _fixUpdateCallback?.Invoke();
        }

        private void LateUpdate()
        {
            _lateUpdateCallback?.Invoke();
        }

        public void AddUpdateListener(MonoCallback callback)
        {
            _updateCallback += callback;
        }
        public void RemoveUpdateListener(MonoCallback callback)
        {
            _updateCallback -= callback;
        }

        public void AddFixUpdateListener(MonoCallback callback)
        {
            _fixUpdateCallback += callback;
        }
        public void RemoveFixUpdateListener(MonoCallback callback)
        {
            _fixUpdateCallback -= callback;
        }

        public void AddLateUpdateListener(MonoCallback callback)
        {
            _lateUpdateCallback += callback;
        }
        public void RemoveLateUpdateListener(MonoCallback callback)
        {
            _lateUpdateCallback -= callback;
        }
    }

}
