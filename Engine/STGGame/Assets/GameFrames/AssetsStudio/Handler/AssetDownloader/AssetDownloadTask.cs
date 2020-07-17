
using System.Collections.Generic;
using XLibrary;

namespace ASGame
{
    public class AssetDownloadTask
    {
        public int id;              
        public int status;                          //状态

        public string[] urlPaths;                   //源路径
        public string storePath;                    //储存路径

        public AssetDownloadCompletedCallback onCompleted;    //完成回调
        public AssetDownloadProgressCallback onProgress;     //进度回调

        private CHttpDownMng m_downloadMgr;

        public long CreateTime { get; protected set; }                    //创建时间
        public long FinishTime { get; protected set; }                    //完成时间

        public long CurSize { get; protected set; }                       //当前下载量
        public long TotalSize { get; protected set; }                     //总量

        public void Start()
        {
            if (urlPaths == null || urlPaths.Length <= 0)
                return;

            if (string.IsNullOrEmpty(storePath))
                return;

            m_downloadMgr = new CHttpDownMng();
            var downList = InitDownFile(urlPaths);
            CreateTime = XTimeTools.NowTimeStampMs();

            m_downloadMgr.StartDown(downList, 2, 100 * 1024, storePath);
        }

        public void Pause()
        {
            if (m_downloadMgr == null)
                return;
        }

        public void Stop()
        {
            if (m_downloadMgr == null)
                return;

            m_downloadMgr.StopDown(true,true);
        }

        public void Clear()
        {
            if (m_downloadMgr != null)
            {

            }
        }

        public void Reset()
        {
            id = 0;
            status = 0;
            urlPaths = null;
            storePath = string.Empty;

            CreateTime = -1;
            FinishTime = -1;

            onCompleted = null;
            onProgress = null;

            CurSize = 0;
            TotalSize = 0;
        }

        private List<DownResInfo> InitDownFile(string[] downUrl)
        {
            List<DownResInfo> downList = new List<DownResInfo>();
            if (downUrl != null && downUrl.Length > 0)
            {
                foreach (var url in downUrl)
                {
                    PushDownFile(downList, url);
                }
            }

            return downList;
        }

        private void PushDownFile(List<DownResInfo> downList, string url)
        {
            DownResInfo node = new DownResInfo();
            node.url = url;
            CHttpDown.GetDownFileSize(url, out node.nFileSize);
            downList.Add(node);
        }
    }

}
