using System;
using SysTimer = System.Threading.Timer;
using SysTimerCallback = System.Threading.TimerCallback;
namespace THGame
{
	public class Timer
	{
		private SysTimer m_timer;
		private int m_times;
		private Action m_action;

		private void OnHandle(Object obj)
		{
            if (m_times == 0)
			{
				Stop();
				return;
			}
            else
			{
				m_times--;
			}
			m_action();
		}

		public Timer(Action action, int interval, int times = -1)
		{
			m_timer = new SysTimer(new SysTimerCallback(OnHandle), null, interval, interval);
			m_times = times;
			m_action = action;
		}

		public void Stop()
		{
			if (m_timer != null)
			{
				m_timer.Change(-1, -1);
			}
		}

		~Timer()
		{
			Stop();
		}
	}
}

