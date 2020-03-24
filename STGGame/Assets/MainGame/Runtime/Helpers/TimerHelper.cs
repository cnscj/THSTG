using System;
using XLibGame;

namespace STGRuntime
{

    public static class TimerHelper
    {
        private static Timer GetTimer()
        {
            return Timer.GetInstance();
        }


        public static int Schedule(Action action, float interval, int times = 0)
        {
            return GetTimer().Schedule(action, interval, times);
        }

        public static int ScheduleNextFrame(Action action)
        {
            return GetTimer().ScheduleNextFrame(action);
        }

        public static int ScheduleEachFrame(Action action)
        {
            return GetTimer().ScheduleEachFrame(action);
        }

        public static int ScheduleOnce(Action action, float interval)
        {
            return GetTimer().ScheduleOnce(action, interval);
        }

        public static void Unschedule(int id)
        {
            GetTimer().Unschedule(id);
        }
    }
}
