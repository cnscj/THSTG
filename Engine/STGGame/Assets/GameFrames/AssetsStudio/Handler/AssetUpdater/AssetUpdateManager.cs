using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    //辅助更新资源(获取更新列表,解压等
    //资源MD5化(严格大小写,
    //资源分包
    public class AssetUpdateManager : Singleton<AssetUpdateManager>
    {
        public static string updateCheckUrl = "";
        public static string updateDownloadUrl = "";
        public static string downloadTempFolder = "";

        public AssetUpdateCompletedCallback onCompleted;
        //初始化当前资源,版本号一些重要信息做资源更新的基础

        //校验文件完整性
        //从服务器获取版本号,检查是否要更新

        //从服务器下载最新的资源列表

        //(扫描资源目录,获得一份完整Md5对比列表)

        //检查磁盘空间

        //缓存有就不下载,没有就下载

        //移除废弃资源

        //校验文件完整性

        //更新完成

        public void Update()
        {
            CheckUpdate(OnCheckCallback);

        }

        public bool IsHaveUpdate()
        {
            return false;
        }

        //检查更新
        private void CheckUpdate(Action action)
        {

        }

        private void OnDownLoadcallback()
        {

        }

        private void DownloadAssets(AssetUpdateUpdateList updateList)
        {

        }

        //替换游戏资源
        //private void ReplaceAssets(AssetUpdateUpdateList.Package updatePackage, string tempFolder, string gameFolder)
        //{

        //}

        private void OnCheckCallback()
        {

        }

    }

}
