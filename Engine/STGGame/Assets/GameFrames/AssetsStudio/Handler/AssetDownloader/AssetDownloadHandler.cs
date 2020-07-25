using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class AssetDownloadHandler
    {
        private CHttpDownMng m_downloadMgr;

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
