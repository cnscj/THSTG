
using System.Collections.Generic;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    public class AssetDownloadTask
    {
        public int id;              
        public int status;                                                  //状态

        public int downThreadNumb = 2;                                      //下载线程数量
        public int limitDownSize = 100 * 1024;                              //下载速度
        public string[] urlPaths;                                           //下载路径
        public string savePath;                                             //储存路径

        public AssetDownloadCompletedCallback onCompleted;                  //完成回调
        public AssetDownloadProgressCallback onProgress;                    //进度回调

        private CDownloader m_downloadMgr;

        public long CreateTime { get; protected set; }                      //创建时间
        public long StartTime { get; protected set; }                       //开始时间
        public long CompletedTime { get; protected set; }                   //完成时间
        public long CurSize { get {return m_downloadMgr != null ? m_downloadMgr.TotalDownSize : 0; } }                                      //当前下载大小
        public long TotalSize { get { return m_downloadMgr != null ? m_downloadMgr.TotalNeedDownSize : 0; } }                               //总的大小
        public int CurCount { get { return m_downloadMgr != null ? m_downloadMgr.TotalDownCount : 0; } }
        public int TotalCount { get { return m_downloadMgr != null ? m_downloadMgr.TotalNeedDownCount : 0; } }
        public List<DownResFile> SuccessDownList { get { return m_downloadMgr != null ? m_downloadMgr.SuccessDownList : null; } }
        public List<DownResFile> FailedDownList { get { return m_downloadMgr != null ? m_downloadMgr.FailedDownList : null; } }

        public AssetDownloadTask()
        {
            CreateTime = XTimeTools.NowTimeStampMs();
            status = AssetDownloadStatus.DOWNLOAD_NONE;
        }

        public void Start()
        {
            var mgr = GetDownloadMgr();
            mgr.OnDownloadFinish = OnFinish;
            mgr.OnDownloadProgress = OnProgress;

            mgr.StartDown(urlPaths, downThreadNumb, limitDownSize, savePath);

            StartTime = XTimeTools.NowTimeStampMs();
            status = AssetDownloadStatus.DOWNLOAD_DOWNLOADING;
        }

        public void Pause()
        {
            if (m_downloadMgr == null)
                return;

            m_downloadMgr.StopDown(false, true);
            StartTime = -1;
            status = AssetDownloadStatus.DOWNLOAD_PAUSE;
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

        private void OnCompleted()
        {
            CompletedTime = XTimeTools.NowTimeStampMs();
            onCompleted?.Invoke(this);
            status = AssetDownloadStatus.DOWNLOAD_COMPLETED;
        }

        private void OnFinish(string url, bool isSuc)
        {
            if (m_downloadMgr == null)
                return;

            if (m_downloadMgr.TotalDownCount == m_downloadMgr.TotalNeedDownCount)
            {
                OnCompleted();
            }
        }

        private void OnProgress(long cur, long total)
        {
            if (m_downloadMgr == null)
                return;

            onProgress?.Invoke(cur, total);
        }
    }
}
