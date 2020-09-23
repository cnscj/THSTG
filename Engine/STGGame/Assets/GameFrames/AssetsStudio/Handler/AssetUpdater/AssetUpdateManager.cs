using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    //辅助更新资源(获取更新列表,解压等
    //资源MD5化(严格大小写,
    //资源分包
    public class AssetUpdateManager : MonoSingleton<AssetUpdateManager>
    {
        public string assetListName = "";
        public string configListName = "";
        public string updateDownloadUrl = "";
        public string assetFolderPath = "";
        public string downloadFolderPath = "";
        public Action<int> onFinish;

        public void UpdateAsset()
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
            /////////////////////////////
            //请求最新的资源列表

            StartCoroutine(OnUpdateAsset());
        }
        ////////////////////////////

        protected IEnumerator OnUpdateAsset()
        {
            AssetUpdateAssetList newAssetList = new AssetUpdateAssetList(); //从服务器下载下来的资源列表
            AssetUpdateAssetList oldAssetList = new AssetUpdateAssetList();
            //从远端下载资源列表到本地
            bool isNetworkDownloadFinish = false;
            var networkAssetList = GetNetworkAssetPath();
            if (!File.Exists(networkAssetList))
            {
                var networkAssetListUrl = GetNetworkAssetUrl();
                var task = AssetDownloadManager.GetInstance().Download(new string[] { networkAssetListUrl }, new string[] { networkAssetList });
                task.onCompleted = (_) => { isNetworkDownloadFinish = true; };

                //等待下载结束
                while (!isNetworkDownloadFinish)
                    yield return null;
            }

            if (File.Exists(assetFolderPath))
            {
                oldAssetList.Import(assetFolderPath);
            }
            else
            {
                onFinish?.Invoke(-1);
                yield break;
            }

            //读取本地资源列表
            var localAssetList = GetLocalAssetListPath();
            if (!File.Exists(localAssetList))
            {
                oldAssetList.Scan(assetFolderPath);
                oldAssetList.Export(localAssetList);
            }
            else
            {
                oldAssetList.Import(assetFolderPath);
            }

            //配置列表读取
            var networkConfigList = GetNetworkConfigPath();
            AssetUpdateConfigList configList = null;
            if (File.Exists(networkConfigList))
            {
                configList = new AssetUpdateConfigList();
                configList.Import(networkConfigList);
            }


            //做资源对比
            AssetUpdateDifferenceList differenceList = new AssetUpdateDifferenceList();
            differenceList.Compare(newAssetList, oldAssetList);
            if (differenceList.IsVerify())  //资源一致,不需要更新
            {
                onFinish?.Invoke(0);
                yield break;
            }

            //差异表转成更新列表去更新
            AssetUpdateUpdateList assetUpdateUpdateList = new AssetUpdateUpdateList(updateDownloadUrl, downloadFolderPath);
            assetUpdateUpdateList.Convert(differenceList, configList);

            //启动更新
            bool isUpdateFinsin = false;
            AssetUpdateUpdater updater = new AssetUpdateUpdater(assetUpdateUpdateList);
            updater.onFinish = () => { isUpdateFinsin = true; };

            while (!isUpdateFinsin)
                yield return null;

            onFinish?.Invoke(1);
            yield break;
        }

        ////////////////////////////

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


        private string GetNetworkAssetUrl()
        {
            return Path.Combine(updateDownloadUrl, assetListName);
        }

        private string GetNetworkAssetPath()
        {
            return Path.Combine(downloadFolderPath, assetListName);
        }

        private string GetNetworkConfigPath()
        {
            return Path.Combine(downloadFolderPath, configListName);
        }

        private string GetLocalAssetListPath()
        {
            return Path.Combine(assetFolderPath, assetListName);
        }
    }

}
