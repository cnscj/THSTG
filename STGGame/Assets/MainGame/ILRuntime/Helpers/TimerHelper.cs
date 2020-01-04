using System;
using XLibGame;

namespace STGService
{

    public static class TimerHelper
    {
        private static TimerManager GetManager()
        {
            return TimerManager.GetInstance();
        }


        public static int Schedule(Action action, float interval, int times = 0)
        {
            return GetManager().Schedule(action, interval, times);
        }

        public static int ScheduleNextFrame(Action action)
        {
            return GetManager().ScheduleNextFrame(action);
        }

        public static int ScheduleEachFrame(Action action)
        {
            return GetManager().ScheduleEachFrame(action);
        }

        public static int ScheduleOnce(Action action, float interval)
        {
            return GetManager().ScheduleOnce(action, interval);
        }

        public static void Unschedule(int id)
        {
            GetManager().Unschedule(id);
        }
    }
}
