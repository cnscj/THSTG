using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    public delegate void AssetDownloadCompletedCallback(AssetDownloadTask task);                //下载完成回调
    public delegate void AssetDownloadFinishCallback(string url, string path);                  //下载完成回调
    public delegate void AssetDownloadProgressCallback(long cur, long total);                   //下载进度回调
}
