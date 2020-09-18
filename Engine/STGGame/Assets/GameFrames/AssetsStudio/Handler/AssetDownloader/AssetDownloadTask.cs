
using System;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;
using XLibrary;

namespace ASGame
{
    public class AssetDownloadTask : IComparable<AssetDownloadTask>
    {
        public int id;
        public int priority;                                                //优先级
        public int status;                                                  //状态

        public int downThreadNumb = 2;                                      //下载线程数量
        public int limitDownSize = -1;                                      //下载速度(Kb)
        public string[] urlPaths;                                           //下载路径
        public string[] savePaths;                                          //储存路径

        public AssetDownloadCompletedCallback onCompleted;                  //完成回调
        public AssetDownloadFinishCallback onFinish;                        //下载回调
        public AssetDownloadProgressCallback onProgress;                    //进度回调

        private CDownloader m_downloadMgr;

        public long CreateTime { get; protected set; }                      //创建时间
        public long StartTime { get; protected set; }                       //开始时间
        public long CompletedTime { get; protected set; }                   //完成时间
        public long CurSize { get {return m_downloadMgr != null ? m_downloadMgr.HadDownedSize : 0; } }                                      //当前下载大小
        public long TotalSize { get { return m_downloadMgr != null ? m_downloadMgr.TotalNeedDownSize : 0; } }                               //总的大小
        public int CurCount { get { return m_downloadMgr != null ? m_downloadMgr.HadDownedCount : 0; } }
        public int TotalCount { get { return m_downloadMgr != null ? m_downloadMgr.TotalNeedDownCount : 0; } }
        public List<DownResFile> SuccessDownList { get { return m_downloadMgr != null ? m_downloadMgr.SuccessDownList : null; } }
        public List<DownResFile> FailedDownList { get { return m_downloadMgr != null ? m_downloadMgr.FailedDownList : null; } }

        public AssetDownloadTask()
        {
            CreateTime = XTimeTools.NowTimeStampMs;
            status = AssetDownloadStatus.DOWNLOAD_NONE;
        }
      
        public int CompareTo(AssetDownloadTask other)
        {
            if (this.priority == other.priority)
                return this.id - other.id;

            return other.priority - this.priority;
        }

        public void Start()
        {
            var mgr = GetDownloadMgr();
            mgr.OnDownloadFinish = OnFinish;
            mgr.OnDownloadProgress = OnProgress;

            mgr.StartDown(urlPaths, savePaths, downThreadNumb, limitDownSize);

            StartTime = XTimeTools.NowTimeStampMs;
            status = AssetDownloadStatus.DOWNLOAD_DOWNLOADING;
        }

        public void Pause()
        {
            if (m_downloadMgr == null)
                return;

            m_downloadMgr.PauseDown();
            StartTime = -1;
            status = AssetDownloadStatus.DOWNLOAD_PAUSE;
        }

        public void Resume()
        {
            if (m_downloadMgr == null)
                return;

            m_downloadMgr.ResumeDown();
            StartTime = XTimeTools.NowTimeStampMs;
            status = AssetDownloadStatus.DOWNLOAD_DOWNLOADING;
        }

        public void Stop()
        {
            if (m_downloadMgr == null)
                return;

            m_downloadMgr.StopDown(true, false);
            StartTime = -1;
            status = AssetDownloadStatus.DOWNLOAD_CANCELED;
        }

        ////
        private CDownloader GetDownloadMgr()
        {
            m_downloadMgr = m_downloadMgr ?? new CDownloader();
            return m_downloadMgr;
        }

        // 下面的回调是子线程回调主线程
        protected void OnCompleted()
        {
            CompletedTime = XTimeTools.NowTimeStampMs;
            status = AssetDownloadStatus.DOWNLOAD_COMPLETED;

            CallbackManager.GetInstance()?.QueueOnMainThread((param) =>
            {
                var task = (AssetDownloadTask)param;
                onCompleted?.Invoke(task);
            },this);
            
        }

        protected void OnFinish(string url, string path)
        {
            if (m_downloadMgr == null)
                return;

            Tuple<string, string> bundle = new Tuple<string, string>(url, path);
            CallbackManager.GetInstance()?.QueueOnMainThread((param) =>
            {
                var tuple = (Tuple<string, string>)param;
                onFinish?.Invoke(tuple.Item1, tuple.Item2);
            }, bundle);

                
            if (m_downloadMgr.HadDownedCount == m_downloadMgr.TotalNeedDownCount)
            {
                OnCompleted();
            }
        }

        protected void OnProgress(long cur, long total)
        {
            if (m_downloadMgr == null)
                return;

            Tuple<long, long> bundle = new Tuple<long, long>(cur, total);
            CallbackManager.GetInstance()?.QueueOnMainThread((param) =>
            {
                var tuple = (Tuple<long, long>)param;
                onProgress?.Invoke(tuple.Item1, tuple.Item2);
            }, bundle);
          
        }

    }
}
