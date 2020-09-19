using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace ASGame
{
    //需要提供分包工具,分包列表等
    //提供 静默下载 一个开关
    public class AssetDownloadManager : MonoSingleton<AssetDownloadManager>
    {
        private GameObject m_centralsGameObject;
        private AssetDownloadCentral m_foregroundCentral;           //前台下载
        private AssetDownloadCentral m_backgroundCentral;           //后台下载

        // 启动时测试一下我本地的IPV6环境, 请将这个域名修改你项目的CDN地址
        public void StartTestIPV6(string szCDNUrl)
        {
            CTcp.StartTestIPV6(szCDNUrl); 
        }

        public int GetDownFileSize(string url)
        {
            CHttpDown.GetDownFileSize(url, out int fileSize);
            return fileSize;
        }

        private void Awake()
        {
            CallbackManager.GetInstance();  //唤醒下
        }

        private GameObject GetCentralGameObject()
        {
            if (m_centralsGameObject == null)
            {
                m_centralsGameObject = new GameObject("DownloadCentrals");
                m_centralsGameObject.transform.SetParent(transform);
            }
            return m_centralsGameObject;
        }

        //前台下载,主动下载
        public AssetDownloadCentral GetForegroundCentral()
        {
            if (m_foregroundCentral == null)
            {
                GameObject centralGobj = new GameObject("ForegroundCentral");
                centralGobj.transform.SetParent(GetCentralGameObject().transform, false);
                m_foregroundCentral = centralGobj.AddComponent<AssetDownloadCentral>();
            }
            return m_foregroundCentral;
        }

        //后台下载,用于边玩边下(空闲自动下载,随时启动,优先级比前台低
        public AssetDownloadCentral GetBackgroundCentral()
        {
            if (m_backgroundCentral == null)
            {
                GameObject centralGobj = new GameObject("BackgroundCentral");
                centralGobj.transform.SetParent(GetCentralGameObject().transform, false);
                m_backgroundCentral = centralGobj.AddComponent<AssetDownloadCentral>();
            }
            return m_backgroundCentral;
        }

        //下载文件
        public AssetDownloadTask Download(string []urls)
        {
            var task = GetForegroundCentral().NewTask(urls);
            GetForegroundCentral().StartTask(task);
            return task;
        }

    }

}
