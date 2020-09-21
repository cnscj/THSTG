using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    //辅助更新资源(获取更新列表,解压等
    //资源MD5化(严格大小写,
    //资源分包
    public class AssetUpdateManager : Singleton<AssetUpdateManager>
    {
        public static string updateDownloadUrl = "";
        public static string assetFolderPath = "";
        public static string downloadFolderPath = "";

       
        public void Update()
        {
            //初始化当前资源,版本号一些重要信息做资源更新的基础

            //校验文件完整性
            //从服务器获取版本号,检查是否要更新

            //检查资源列表是不是最新的

            //从服务器下载最新的资源列表

            //(扫描资源目录,获得一份完整Md5对比列表)

            //检查磁盘空间

            //缓存有就不下载,没有就下载

            //校验下载资源完整性
            //移除废弃资源

            //校验文件完整性

            //更新完成
            //回到开头在做一遍检查
        }

        public bool IsHaveUpdate()
        {
            return false;
        }

        //检查更新
        private void CheckUpdate(Action action)
        {

        }

        //比对资源
        private AssetUpdateDifferenceList CompareAsset(AssetUpdateAssetList serverList, AssetUpdateAssetList clientList)
        {
            if (serverList == null || clientList == null)
                return null;

            AssetUpdateDifferenceList assetUpdateContrastList = new AssetUpdateDifferenceList();
            assetUpdateContrastList.Compare(clientList, serverList);

            return assetUpdateContrastList;
        }

        private void UpdateAsset(AssetUpdateDifferenceList contrastList)
        {
            if (contrastList == null)
                return;

            if (string.IsNullOrEmpty(assetFolderPath) || string.IsNullOrEmpty(downloadFolderPath))
                return;


            if (!Directory.Exists(assetFolderPath) || !Directory.Exists(downloadFolderPath))
                return;

            //将更新目录的文件拷贝过来,不过这之前还要校验下更新目录资源完整性


        }
    }

}
