
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLibrary.Package;

namespace XLibGame
{
    //用与子线程回调主线程
    public class CallbackManager : MonoSingleton<CallbackManager>
    {
        //单个执行单元（无延迟）
        struct NoDelayedQueueItem
        {
            public Action<object> action;
            public object param;
        }

        //单个执行单元（有延迟）
        struct DelayedQueueItem
        {
            public Action<object> action;
            public object param;
            public float time;
        }
        //全部执行列表（无延迟）
        List<NoDelayedQueueItem> _listNoDelayActions = new List<NoDelayedQueueItem>();
        //全部执行列表（有延迟）
        List<DelayedQueueItem> _listDelayedActions = new List<DelayedQueueItem>();

        //当前执行的无延时函数链
        List<NoDelayedQueueItem> _currentActions = new List<NoDelayedQueueItem>();
        //当前执行的有延时函数链
        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        //加入到主线程执行队列（无延迟）
        public void QueueOnMainThread(Action<object> taction, object param)
        {
            QueueOnMainThread(taction, param, 0f);
        }

        //加入到主线程执行队列（有延迟）
        public void QueueOnMainThread(Action<object> action, object param, float time)
        {
            if (time != 0)
            {
                lock (_listDelayedActions)
                {
                    _listDelayedActions.Add(new DelayedQueueItem { time = Time.fixedTime + time, action = action, param = param });
                }
            }
            else
            {
                lock (_listNoDelayActions)
                {
                    _listNoDelayActions.Add(new NoDelayedQueueItem { action = action, param = param });
                }
            }
        }

        void FixedUpdate()
        {
            if (_listNoDelayActions.Count > 0)
            {
                lock (_listNoDelayActions)
                {
                    _currentActions.Clear();
                    _currentActions.AddRange(_listNoDelayActions);
                    _listNoDelayActions.Clear();
                }
                for (int i = 0; i < _currentActions.Count; i++)
                {
                    _currentActions[i].action(_currentActions[i].param);
                }
            }

            if (_listDelayedActions.Count > 0)
            {
                lock (_listDelayedActions)
                {
                    _currentDelayed.Clear();
                    _currentDelayed.AddRange(_listDelayedActions.Where(d => Time.fixedTime >= d.time));
                    for (int i = 0; i < _currentDelayed.Count; i++)
                    {
                        _listDelayedActions.Remove(_currentDelayed[i]);
                    }
                }

                for (int i = 0; i < _currentDelayed.Count; i++)
                {
                    _currentDelayed[i].action(_currentDelayed[i].param);
                }
            }
        }
    }
}
