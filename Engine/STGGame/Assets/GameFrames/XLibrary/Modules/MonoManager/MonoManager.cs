using XLibrary.Package;

namespace XLibGame
{
    public class MonoManager : MonoSingleton<MonoManager>
    {
        private MonoCallback _awakeCallback;
        private MonoCallback _startCallback;
        private MonoCallback _updateCallback;
        private MonoCallback _fixUpdateCallback;
        private MonoCallback _lateUpdateCallback;


        private new void Awake()
        {
            _awakeCallback?.Invoke();
        }

        private void Start()
        {
            _startCallback?.Invoke();
        }

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

        ///////
        public void AddAwakeListener(MonoCallback callback)
        {
            _awakeCallback += callback;
        }
        public void RemoveAwakeListener(MonoCallback callback)
        {
            _awakeCallback -= callback;
        }

        public void AddStartListener(MonoCallback callback)
        {
            _startCallback += callback;
        }
        public void RemoveStartListener(MonoCallback callback)
        {
            _startCallback -= callback;
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
