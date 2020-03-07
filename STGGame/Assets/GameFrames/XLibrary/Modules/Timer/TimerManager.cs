
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    public class TimerManager : MonoSingleton<TimerManager>
    {
        private int m_id = 0;
        private SortedDictionary<int, Coroutine> m_coroutines = new SortedDictionary<int, Coroutine>();

        public void UnscheduleAll()
        {
            foreach (var kvs in m_coroutines)
            {
                StopCoroutine(kvs.Value);
            }
            m_coroutines.Clear();
            m_id = 0;
        }

        public int Schedule(Action action, float interval, int times = -1)
        {
            int id = m_id++;
            IEnumerator co = CreateCoroutine(id, action, interval, times);
            m_coroutines.Add(id, StartCoroutine(co));
            return id;
        }

        public int ScheduleNextFrame(Action action)
        {
            return Schedule(action, 0f, 1);
        }

        public int ScheduleEachFrame(Action action)
        {
            return Schedule(action, 0f, -1);
        }

        public int ScheduleOnce(Action action, float interval)
        {
            return Schedule(action, interval, 1);
        }

        public int ScheduleDuration(float interval, int duration, Action<float> pollFunc, Action endFunc = null)
        {
            float usedTime = 0;
            int timerId = -1;
            timerId = Schedule(() =>
            {
                if (usedTime > duration)
                {
                    Unschedule(timerId);
                    endFunc?.Invoke();
                    return;
                }
                pollFunc?.Invoke(usedTime);
                usedTime += interval;
            }, interval);

            return timerId;
        }

        public void Unschedule(int id)
        {
            if (m_coroutines.ContainsKey(id))
            {
                StopCoroutine(m_coroutines[id]);
                m_coroutines.Remove(id);
            }
        }

        IEnumerator CreateCoroutine(int id, Action action, float interval, int times)
        {
            if (interval < 0.001f)
            {
                do
                {
                    yield return null;
                    action();
                }
                while (times <= 0 || times-- > 1);
            }
            else
            {
                WaitForSeconds wait = new WaitForSeconds(interval);
                do
                {
                    yield return wait;
                    action();
                }
                while (times <= 0 || times-- > 1);
            }

            m_coroutines.Remove(id);
            action = null;
        }
    }
    
}
