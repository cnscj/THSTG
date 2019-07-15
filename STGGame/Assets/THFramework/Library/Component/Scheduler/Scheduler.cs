
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace THGame
{
    public class Scheduler : Singleton<Scheduler>
    {
        private Dictionary<int, Timer> m_timerMap = new Dictionary<int, Timer>();

        public int Schedule(Action<float> action , float interval = 0.1f)
        {

            return 0;
        }

        public void Unschedule(int timerId)
        {

        }

        public int ScheduleOnce(Action<float> action, float interval)
        {
            return Schedule((delay)=>
            {
               action.Invoke(delay);
            }, interval);
        }

        public int ScheduleEachFrames(Action<float> action)
        {
            return Schedule((delay) =>
            {
                action.Invoke(delay);
            }, 1/60);
        }
    }
}
