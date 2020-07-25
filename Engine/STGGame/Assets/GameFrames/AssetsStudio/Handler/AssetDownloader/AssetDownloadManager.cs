using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    //TODO:静默下载:涉及到资源分包(S,U1~U9),直至把完整游戏下完
    //需要提供分包工具,分包列表等
    //提供 静默下载 一个开关
    public class AssetDownloadManager : MonoSingleton<AssetDownloadManager>
    {
        private AssetDownloadCentral m_foregroundCentral;           //前台下载
        private AssetDownloadCentral m_backgroundCentral;           //后台下载



        private AssetDownloadCentral GetForegroundCentral()
        {
            m_foregroundCentral = m_foregroundCentral ?? new AssetDownloadCentral();
            return m_foregroundCentral;
        }

        private AssetDownloadCentral GetBackgroundCentral()
        {
            m_backgroundCentral = m_backgroundCentral ?? new AssetDownloadCentral();
            return m_backgroundCentral;
        }
    }

}
