
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
        public string[] urlPaths;                                           //源路径
        public string storePath;                                            //储存路径

        public AssetDownloadCompletedCallback onCompleted;                  //完成回调
        public AssetDownloadProgressCallback onProgress;                    //进度回调

        private CHttpDownMng m_downloadMgr;
        private List<DownResInfo> m_downList;

        public long CreateTime { get; protected set; }                      //创建时间
        public long FinishTime { get; protected set; }                      //完成时间
        public long CurSize { get; protected set; }                         //当前下载大小
        public long TotalSize { get; protected set; }                       //总的大小

        public AssetDownloadTask()
        {
            CreateTime = XTimeTools.NowTimeStampMs();
        }

        public void Start()
        {
            if(m_downloadMgr == null)
            {
                GetDownloadMgr();

            }
            InitDownFile(urlPaths);
            m_downloadMgr.StartDown(m_downList, downThreadNumb, limitDownSize, storePath);
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
            List<DownResInfo> downList = new List<DownResInfo>();
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
        private CHttpDownMng GetDownloadMgr()
        {
            m_downloadMgr = m_downloadMgr ?? new CHttpDownMng();
            return m_downloadMgr;
        }

        private List<DownResInfo> GetDownloadList()
        {
            m_downList = m_downList ?? new List<DownResInfo>();
            return m_downList;
        }

        private void OnCompleted()
        {
            FinishTime = XTimeTools.NowTimeStampMs();

            onCompleted?.Invoke(this);
        }
        private void OnProgress()
        {
            onProgress?.Invoke(this);
        }

    }
}
