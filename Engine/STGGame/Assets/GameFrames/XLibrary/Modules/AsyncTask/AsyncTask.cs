using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace XLibGame
{
    /// <summary>
    /// 异步任务，交给子线程执行
    /// </summary>
    public abstract class AsyncTask
    {
        // multiThreadMgr
        protected MultiThreadManager MultiThreadMgr
        {
            get
            {
                return MultiThreadManager.GetInstance();
            }
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        public void Execute()
        {
            try
            {
                Run();
            }
            finally
            {
                MultiThreadMgr.FinishTask(this);
            }
        }

        /// <summary>
        /// 关闭执行
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 开始运行
        /// </summary>
        public abstract void Run();
    }


    public class AsyncActionTask : AsyncTask
    {
        Action onRun;
        Action onClose;
        public AsyncActionTask(Action runFunc, Action closeFunc)
        {
            onRun = runFunc;
            onClose = closeFunc;
        }

        public override void Close()
        {
            onClose?.Invoke();
        }

        public override void Run()
        {
            onRun?.Invoke();
        }
    }
}
