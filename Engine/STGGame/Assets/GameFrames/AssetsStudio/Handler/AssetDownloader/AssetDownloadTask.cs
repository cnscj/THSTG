using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    public class AssetDownloadTask
    {
        public int id;              
        public int status;                          //状态
        public string[] urlPaths;                   //源路径
        public string storePath;                    //储存路径

        public long createTime;                     //创建时间
        public long finishTime;                     //完成时间

        public long curSize;                       //当前下载量
        public long totalSize;                     //总量

        public AssetDownloadCompletedCallback onCompleted;    //完成回调
        public AssetDownloadProgressCallback onProgress;     //进度回调

        public void Reset()
        {
            id = 0;
            status = 0;
            urlPaths = null;
            storePath = string.Empty;

            createTime = -1;
            finishTime = -1;

            onCompleted = null;
            onProgress = null;

            curSize = 0;
            totalSize = 0;
        }
    }

}
