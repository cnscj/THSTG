using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System;

namespace XLibGame
{
    public abstract class HttpTask : IDisposable
    {
        public enum EPriority
        {
            Main = 0,
            Background = 1,
        }

        public enum ETaskState : uint
        {
            Created = 0,
            Running,
            Completed,
            Canceled,
        }

        public EPriority priority { get; set; } = EPriority.Main;

        public ETaskState state {get; protected set;} = ETaskState.Created;
        public string error {get; protected set;}

        public bool hasError
        {
            get{ return !string.IsNullOrEmpty(error); }
        }

        public bool isDone
        {
            get { return state >= ETaskState.Completed || hasError; }
        }

        public float progress {get; protected set;}

        #region 生命周期
        /// <summary>
        /// 任务开始，用协程的方式
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator Start();

        /// <summary>
        /// 刷新任务
        /// </summary>
        // public virtual void Update(){}

        /// <summary>
        /// 取消任务
        /// </summary>
        public abstract void Cancel();

        /// <summary>
        /// 被cancel过，但是还来不急删除，这时又要请求了
        /// </summary>
        public virtual void Revive()
        {

        }
        #endregion

        /// <summary>
        /// 专门用来析构
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {

        }

        public virtual void ClearCallback()
        {

        }
    }
}