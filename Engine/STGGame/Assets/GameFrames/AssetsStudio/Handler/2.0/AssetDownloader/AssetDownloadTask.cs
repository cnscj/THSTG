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
        public string urlPath;                      //源路径
        public string storePath;                    //储存路径

        public long createTime;                     //创建时间
        public long finishTime;                     //完成时间

        public AssetDownloadCallback onCallback;    //回调

        public long curSize;                       //当前下载量
        public long totalSize;                     //总量


        public void Reset()
        {
            id = 0;
            status = 0;
            urlPath = string.Empty;
            storePath = string.Empty;

            createTime = -1;
            finishTime = -1;

            onCallback = null;

            curSize= 0;
            totalSize = 0;
        }
    }

}
