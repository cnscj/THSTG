
using UnityEngine;

namespace XLibGame
{
    public delegate void Invoke<T>(T obj);
    public class CallbackInvoke<T>
    {
        private Invoke<T> m_funcs;
        private BaseRef m_baseRef;
        public CallbackInvoke(BaseRef baseRef) { m_baseRef = baseRef; }
        public void Clear() { m_funcs = null; }
        public CallbackInvoke<T> Set(Invoke<T> f) { m_funcs = f; return this; }
        public CallbackInvoke<T> Add(Invoke<T> f) { m_funcs += f; return this; }
        public CallbackInvoke<T> Sub(Invoke<T> f) { m_funcs -= f; return this; }
        public void Invoke(T obj) { Timer.GetInstance().ScheduleNextFrame(() => { m_funcs?.Invoke(obj); m_baseRef?.Release(); }); }

        public static CallbackInvoke<T> operator -(CallbackInvoke<T> left, Invoke<T> right){return left.Sub(right);}
        public static CallbackInvoke<T> operator +(CallbackInvoke<T> left, Invoke<T> right){return left.Add(right);}
    }

    public class Callback<T1, T2> : BaseRef
    {
        public static Callback<T1, T2> GetOrNew()
        {
            var pool = ObjectPoolManager.GetInstance().GetOrCreatePool<Callback<T1, T2>>();
            var callback = pool.GetOrCreate();
            callback.Clear();
            return callback;
        }
        public CallbackInvoke<T1> onSuccess;
        public CallbackInvoke<T2> onFailed;

        public Callback()
        {
            Clear();
        }
        public void Clear()
        {
            onSuccess = onSuccess ?? new CallbackInvoke<T1>(this);
            onFailed = onFailed ?? new CallbackInvoke<T2>(this);
            onSuccess.Clear();
            onFailed.Clear();
        }
        protected override void OnRelease()
        {
            Clear();
            ObjectPoolManager.GetInstance().GetOrCreatePool<Callback<T1, T2>>().Release(this);
        }
    }

}
