
using System.Collections.Generic;
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
        private List<DownResInfo> m_downList;

        public long CreateTime { get; protected set; }                      //创建时间
        public long FinishTime { get; protected set; }                      //完成时间
        public long CurSize { get {return m_downloadMgr != null ? m_downloadMgr.TotalDownSize : 0; } }                                      //当前下载大小
        public long TotalSize { get { return m_downloadMgr != null ? m_downloadMgr.TotalNeedDownSize : 0; } }                               //总的大小
        public int CurCount { get; protected set; }
        public int TotalCount { get; protected set; }

        public AssetDownloadTask()
        {
            CreateTime = XTimeTools.NowTimeStampMs();
        }

        public void Start()
        {
            var mgr = GetDownloadMgr();
            var downList = InitDownFile(urlPaths);
            mgr.StartDown(downList, downThreadNumb, limitDownSize, savePath);
        }

        public void Pause()
        {
            if (m_downloadMgr == null)
                return;

            m_downloadMgr.StopDown(false, true);
        }

        public void Stop()
        {
            if (m_downloadMgr == null)
                return;

            m_downloadMgr.StopDown(true, false);
        }

        public void Clear()
        {
            Stop();
            m_downList?.Clear();
            CreateTime = XTimeTools.NowTimeStampMs();
            FinishTime = -1;
        }

        ////
        private List<DownResInfo> InitDownFile(string[] downUrl)
        {
            List<DownResInfo> downList = GetDownloadList();
            downList.Clear();
            if (downUrl != null && downUrl.Length > 0)
            {
                foreach (var url in downUrl)
                {
                    PushDownFile(url);
                }
            }

            return downList;
        }

        private void PushDownFile(string url)
        {
            var downList = GetDownloadList();
            DownResInfo node = new DownResInfo();
            node.url = url;
            CHttpDown.GetDownFileSize(url, out node.nFileSize);
            downList.Add(node);
        }

        private CDownloader GetDownloadMgr()
        {
            m_downloadMgr = m_downloadMgr ?? new CDownloader();
            return m_downloadMgr;
        }

        private List<DownResInfo> GetDownloadList()
        {
            m_downList = m_downList ?? new List<DownResInfo>();
            return m_downList;
        }

        private void OnFinish()
        {
            FinishTime = XTimeTools.NowTimeStampMs();

            onCompleted?.Invoke(this);
        }

        private void OnProgress(long cur, long total)
        {
            onProgress?.Invoke(cur, total);
        }

    }
}
