using System;
using UnityEngine;

namespace THGame
{
    public class ResourceLoadListener<T>
    {
        public delegate void CompeletedCall<T0>(T0 obj);
        public delegate void ProgressCall(float percent);
        public bool completed
        {
            get
            {
                return m_isCompleted;
            }
        }
        public CompeletedCall<T> Compeleted;
        public ProgressCall Progress;
        protected bool m_isCompleted;

        public void CallCompeleted(T obj)
        {
            //TODO:当加载速度比设置速度快时,这里循序有问题
            if (Compeleted != null)
            {
                Compeleted.Invoke(obj);
            }
            m_isCompleted = true;
        }

        public void CallProgress(float percent)
        {
            if (Progress != null)
            {
                Progress.Invoke(percent);
            }
        }

    }

}
