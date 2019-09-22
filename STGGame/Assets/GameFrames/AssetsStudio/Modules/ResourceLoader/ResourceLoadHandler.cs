using System;
using UnityEngine;

namespace ASGame
{
    public class ResourceLoadHandler<T>
    {
        public delegate void CompeletedCall<T0>(T0 obj);
        public delegate void ProgressCall(float percent);
        public bool isDone
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
            if (Compeleted != null)
            {
                Compeleted(obj);
            }
            m_isCompleted = true;
        }

        public void CallProgress(float percent)
        {
            if (Progress != null)
            {
                Progress(percent);
            }
        }

    }

}
