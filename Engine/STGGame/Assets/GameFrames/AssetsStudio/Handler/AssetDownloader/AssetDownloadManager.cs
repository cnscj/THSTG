using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    //需要提供分包工具,分包列表等
    //提供 静默下载 一个开关
    public class AssetDownloadManager : MonoSingleton<AssetDownloadManager>
    {
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

        //前台下载,主动下载
        public AssetDownloadCentral GetForegroundCentral()
        {
            m_foregroundCentral = m_foregroundCentral ?? new AssetDownloadCentral();
            return m_foregroundCentral;
        }

        //后台下载,用于边玩边下(空闲自动下载,随时启动,优先级比前台低
        public AssetDownloadCentral GetBackgroundCentral()
        {
            m_backgroundCentral = m_backgroundCentral ?? new AssetDownloadCentral();
            return m_backgroundCentral;
        }



    }

}
