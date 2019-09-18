
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace THGame
{
    public class SchedulerManager : MonoSingleton<SchedulerManager>
    {
		private int m_id = 0;
		private SortedDictionary<int, Coroutine> m_coroutines = new SortedDictionary<int, Coroutine>();

        public void UnscheduleAll()
        {
            StopAllCoroutines();
            m_coroutines.Clear();
            m_id = 0;
        }

        public int Schedule(Action action, float interval, int times = 0)
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
            return Schedule(action, 0, 0);
        }

        public int ScheduleOnce(Action action, float interval)
        {
            return Schedule(action, interval, 1);
        }

        public void Unschedule(int id)
        {
            if (m_coroutines.ContainsKey(id))
            {
                StopCoroutine(m_coroutines[id]);
                m_coroutines.Remove(id);
            }
        }

        private IEnumerator CreateCoroutine(int id, Action action, float interval, int times)
        {
            if (interval < 0.001)
            {
                do
                {
                    yield return null;
                    action();
                }
                while (times == 0 || times-- > 1);
            }
            else
            {
                do
                {
                    yield return new WaitForSeconds(interval);
                    action();
                }
                while (times == 0 || times-- > 1);
            }

            m_coroutines.Remove(id);
            action = null;
        }
    }
    
}
