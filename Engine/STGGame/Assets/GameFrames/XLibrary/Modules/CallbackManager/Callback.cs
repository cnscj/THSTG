using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public delegate void Invoke();
    public delegate void Invoke<T>(T obj);
    public delegate void Invoke<T1, T2>(T1 obj1, T2 obj2);
    public delegate void Invoke<T1, T2, T3>(T1 obj1, T2 obj2, T3 obj3);

    public class CallbackInvoke<T>
    {
        private Invoke<T> m_funcs;
        public void Clear() { m_funcs = null; }
        public void Set(Invoke<T> f) { m_funcs = f; }
        public void Add(Invoke<T> f) { m_funcs += f; }
        public void Sub(Invoke<T> f) { m_funcs -= f; }
        public void Invoke(T obj) { m_funcs?.Invoke(obj); }
        public void InvokeDelay(T obj) {
            Timer.GetInstance().ScheduleNextFrame(() =>
            {
                Invoke(obj);
            });
        }


    }

    public class Callback<T1,T2> : BaseRef
    {
        public static Callback<T1, T2> GetOrNew()
        {
            var pool = ObjectPoolManager.GetInstance().GetOrCreatePool<Callback<T1, T2>>();
            var callback = pool.GetOrCreate();
            callback.Clear();
            return callback;
        }
        public CallbackInvoke<T1> onSuccess = new CallbackInvoke<T1>();
        public CallbackInvoke<T2> onFailed = new CallbackInvoke<T2>();

        public void Clear()
        {
            onSuccess?.Clear();
            onFailed?.Clear();
        }
        protected override void OnRelease()
        {
            base.OnRelease();
            Clear();
            var pool = ObjectPoolManager.GetInstance().GetOrCreatePool<Callback<T1, T2>>();
            pool.Release(this);
        }
    }


    /////////////////////////////////////
    public class CallbackInvoke<T1,T2>
    {
        private Invoke<T1, T2> m_funcs;
        public void Clear() { m_funcs = null; }
        public void Set(Invoke<T1, T2> f) { m_funcs = f; }
        public void Add(Invoke<T1, T2> f) { m_funcs += f; }
        public void Sub(Invoke<T1, T2> f) { m_funcs -= f; }
        public void Invoke(T1 obj1, T2 obj2) { m_funcs?.Invoke(obj1, obj2); }
        public void InvokeDelay(T1 obj1, T2 obj2)
        {
            Timer.GetInstance().ScheduleNextFrame(() =>
            {
                Invoke(obj1,obj2);
            });
        }

    }

    public class Callback<T1, T2, T3> : BaseRef
    {
        public static Callback<T1, T2, T3> GetOrNew()
        {
            var pool = ObjectPoolManager.GetInstance().GetOrCreatePool<Callback<T1, T2, T3>>();
            var callback = pool.GetOrCreate();
            callback.Clear();
            //callback.ReleaseLater();
            return callback;
        }
        public CallbackInvoke<T1, T2> onSuccess = new CallbackInvoke<T1, T2>();
        public CallbackInvoke<T3> onFailed = new CallbackInvoke<T3>();

        public void Clear()
        {
            onSuccess?.Clear();
            onFailed?.Clear();
        }
        protected override void OnRelease()
        {
            base.OnRelease();
            Clear();
            var pool = ObjectPoolManager.GetInstance().GetOrCreatePool<Callback<T1, T2, T3>>();
            pool.Release(this);
        }
    }
}
