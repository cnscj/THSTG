
using System.Collections.Generic;
namespace ASGame
{
    public class AssetDownloadTask
    {
        public int id;              
        public int status;                                                  //状态
        public string[] urlPaths;                                           //源路径
        public string storePath;                                            //储存路径

        public AssetDownloadCompletedCallback onCompleted;                  //完成回调
        public AssetDownloadProgressCallback onProgress;                    //进度回调

        private AssetDownloadHandler m_handler;                             //处理者

        public long CreateTime { get; protected set; }                      //创建时间
        public long FinishTime { get; protected set; }                      //完成时间

        public long CurSize { get; protected set; }                         //当前下载量
        public long TotalSize { get; protected set; }                       //总量

        public void Start()
        {
            if (urlPaths == null || urlPaths.Length <= 0)
                return;

            if (string.IsNullOrEmpty(storePath))
                return;

        }

        public void Pause()
        {
            if (m_handler == null)
                return;
        }

        public void Stop()
        {
            if (m_handler == null)
                return;

        }

        public void Clear()
        {
            if (m_handler == null)
                return;
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
    }
}
