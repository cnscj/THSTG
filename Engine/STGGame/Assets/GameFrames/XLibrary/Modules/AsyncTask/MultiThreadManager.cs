using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using XLibrary.Package;
namespace XLibGame
{
    /// <summary>
    /// 多线程管理器
    /// </summary>
    public class MultiThreadManager : MonoSingleton<MultiThreadManager>
    {
        // 需要在主线程执行的操作
        static List<Action> actions = new List<Action>();
        static List<Action> runningActions = new List<Action>();
        static object action_lock = new object();
        static object list_lock = new object();

        // 异步任务队列
        static List<AsyncTask> taskList = new List<AsyncTask>();

        /// <summary>
        /// 在主线程中执行
        /// </summary>
        /// <param name="action">Action.</param>
        public void RunOnMainThread(Action action)
        {
            lock (action_lock)
            {
                actions.Add(action);
            }
        }

        /// <summary>
        /// 添加异步任务
        /// </summary>
        /// <param name="runnable">Runnable.</param>
        public void AddAsyncTask(AsyncTask runnable)
        {
            Debug.Log("AddTask:" + runnable.ToString());
            taskList.Add(runnable);
            Thread thread = new Thread(runnable.Execute);
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 完成异步任务
        /// </summary>
        /// <param name="runnable">Runnable.</param>
        public void FinishTask(AsyncTask runnable)
        {
            //可能会有线程安全,建议加锁
            lock (list_lock)
            {
                taskList.Remove(runnable);
                Debug.Log("RemoveTask:" + runnable.ToString() + "," + taskList.Count);
            }
        }

        /// <summary>
        /// 主线程更新
        /// </summary>
        void Update()
        {
            lock (action_lock)
            {
                runningActions.Clear();
                runningActions.AddRange(actions);
                actions.Clear();
            }

            // 处理主线程事件
            if (runningActions.Count > 0)
            {
                foreach (Action action in runningActions)
                {
                    action();
                }
            }
            runningActions.Clear();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < taskList.Count; i++)
            {
                taskList[i].Close();
            }
        }
    }
}
