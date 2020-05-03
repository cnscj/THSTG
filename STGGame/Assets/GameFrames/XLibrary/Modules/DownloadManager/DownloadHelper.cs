using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace XLibGame
{
    /// <summary>
    /// 这里主要是放一些具体下载相关的逻辑
    /// </summary>
    public static class DownloadHelper
    {
        public enum ECdnType
        {
            Main = 0,
            Group,
            Test
        }
        public static Dictionary<ECdnType, List<string>> cdnDict = new Dictionary<ECdnType, List<string>>();

        public static void AddCdnDomain(ECdnType cdnType, string cdnDomain)
        {
            List<string> cdnDomains;
            if (!cdnDict.TryGetValue(cdnType, out cdnDomains))
            {
                cdnDomains = new List<string>();
                cdnDict.Add(cdnType, cdnDomains);
            }

            cdnDomains.Add(cdnDomain);
        }


        #region Download
        private static List<string> m_downloadHosts = new List<string>();

        public static void AddDownloadHost(string url)
        {
            m_downloadHosts.Add(url);
        }

        public static void ClearDownloadHosts()
        {
            m_downloadHosts.Clear();
        }


        public static string GetHost(int hostIndex)
        {
            if (m_downloadHosts.Count == 0)
            {
                Debug.LogError("no download host");
                return "";
            }

            if ( hostIndex >= m_downloadHosts.Count )
            {
                Debug.LogWarningFormat("hostIndex {0} is out of range", hostIndex);
                return "";
            }

            return m_downloadHosts[hostIndex];
        }

        // 开启网络下载
        public static DownloadTask DownloadFile(string storePath, string md5Path, string md5, bool isAssetBundle, HttpCallback callback)
        {
            int hostIndex = 0;

            string host = GetHost(hostIndex);
            string relativePath = md5Path;
            string saveFileFullPath = storePath + md5Path;
            if (string.IsNullOrEmpty(host))
            {
                Debug.LogErrorFormat("The download host is not initialized yet, but you are trying to download {0}", relativePath);
            }

            DownloadTask requestTask = null;
            requestTask = DownloadManager.instance.DownloadFile(host, relativePath, saveFileFullPath, 0, (dict)=>
            {
                bool isFailed = false;
                if ( (dict.ContainsKey("status") && (int)dict["status"] != 200) ||
                    !dict.ContainsKey("saveFilePath") )
                {
                    isFailed = true;
                    Debug.LogWarningFormat("{0}{1}: {2}", host, relativePath, (dict.ContainsKey("dict")? dict["data"] : "download error") );
                }

                if (isFailed && hostIndex < m_downloadHosts.Count - 1)
                {
                    host = GetHost(hostIndex+1);
                    requestTask.ChangeUrlAndRestart(relativePath, host);

                    DownloadManager.instance.ReAddDownloadTask(requestTask);
                    return;
                }

                callback(dict);
            },
            true, true, md5);

            return requestTask;
        }

        // 取消并清空回调，如果已经在进行的就取消不了了
        public static void DisposeDownloadTask(DownloadTask task)
        {
            DownloadManager.instance.DisposeDownloadTask(task);
        }

        // 取消
        //public static void CancelDownloadTask(DownloadTask task)
        //{
        //    DownloadManager.instance.CancelDownloadTask(task);
        //}

        #endregion


    }


}
