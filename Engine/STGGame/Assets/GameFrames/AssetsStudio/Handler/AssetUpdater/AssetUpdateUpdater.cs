using System;
using System.Collections.Generic;
using System.IO;
using XLibrary;

namespace ASGame
{
    public class AssetUpdateUpdater
    {
        public int maxRetryTimes = 3;
        public Action<AssetDownloadTask> onDownloadCompleted;
        public Action<float> onDownloadProgress;
        public Action onFinish;

        private AssetUpdateUpdateList _updateList;
        private AssetDownloadTask _downloadTask;
        private string _srcAssetPaths;
        private int _downloadRetryTimes;
        public AssetUpdateUpdater(AssetUpdateUpdateList updateList)
        {
            if (updateList == null)
                return;

            _updateList = updateList;
        }

        // mac: /Users/test
        // win: D:\\test
        //这里没有考虑Android和Ios,需要提供SDk
        public long GetFreeDiskSpace(string storagePath)
        {
            string diskName = Path.GetPathRoot(storagePath);
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.Name == diskName)
                {
                    return drive.AvailableFreeSpace;  //不同设备获取的值需要验证
                }
            }
            return -1L;
        }

        public bool IsDiskspaceEnough(int packageIndex)
        {
            //取得保存路径下所在磁盘剩余空间大小
            var baseSaveFolder = _updateList.BaseSaveFolder;
            var residueSpace = GetFreeDiskSpace(baseSaveFolder);
            var needSpace = GetUpdateNeedSize(packageIndex);
            return needSpace <= residueSpace;
        }

        public long GetResidueDiskSpace()
        {
            var baseSaveFolder = _updateList.BaseSaveFolder;
            var residueSpace = GetFreeDiskSpace(baseSaveFolder);
            return residueSpace;
        }

        public long GetUpdateNeedSize(int packageIndex)
        {
            var package = _updateList.GetPackage(packageIndex);
            if (package != null)
            {
                return package.size;
            }
            return 0L;
        }

        public void Update(string srcAssetPaths, int packageIndex = 0)
        {
            if (!IsDiskspaceEnough(packageIndex))
                return;

            _srcAssetPaths = srcAssetPaths;

            var updateUrls = _updateList.GetUrlList(packageIndex);
            var updateSaves = _updateList.GetSaveList(packageIndex);

            _downloadRetryTimes = 0;
            DownloadFiles(packageIndex, updateUrls, updateSaves);
        }

        protected void OnDownloadCompleted(AssetUpdateUpdateList.Package package)
        {
            //校验文件完整性
            var packageIndex = package.index;
            string saveFolder = _updateList.GetSaveFolderPath(packageIndex);

            //完整性通过,开始拷贝文件
            List<string> urlPaths = new List<string>();
            List<string> savePaths = new List<string>();
            if(VerifyFiles(packageIndex, urlPaths, savePaths))  //所有文件验证成功才去拷贝
            {
                UpdateFiles(packageIndex, _srcAssetPaths);
            }
            else
            {
                _downloadRetryTimes++;
                DownloadFiles(packageIndex,urlPaths.ToArray(), savePaths.ToArray());
                return;
            }

            //没通过的文件重新下载

            onFinish?.Invoke();
        }

        private void DownloadFiles(int packageIndex, string[]urlPaths, string[] savePaths)
        {
            if (urlPaths == null)
                return;

            if (urlPaths.Length <= 0)
                return;

            if (_downloadRetryTimes >= maxRetryTimes)   //重试次数大于n次,判断为更新失败
                return;

            var downloader = AssetDownloadManager.GetInstance().GetForegroundCentral();

            _downloadTask = downloader.NewTask(urlPaths, savePaths);
            _downloadTask.onCompleted = (task) =>
            {
                onDownloadCompleted?.Invoke(task);
                var package = _updateList.GetPackage(packageIndex);
                OnDownloadCompleted(package);
            };
        }

        private bool VerifyFiles(int packageIndex, List<string> urlPaths, List<string> savePaths)
        {
            string folderPath = _updateList.GetSaveFolderPath(packageIndex);
            if (string.IsNullOrEmpty(folderPath))
                return false;

            if (!Directory.Exists(folderPath))
                return false;

            bool isVerify = true;
            var package = _updateList.GetPackage(packageIndex);

            var pathsList = package.GetSaveList();
            var urlsList = package.GetUrlList();
            int totalCount = Math.Max(pathsList.Count, urlsList.Count);
            for (int i = 0; i < totalCount; ++i)
            {
                var savePath = pathsList[i];
                var urlPath = urlsList[i];

                bool isFailed = true;
                var item = _updateList.GetItem(savePath);
                if (item != null)
                {
                    if (File.Exists(savePath))
                    {
                        string fileMd5 = XFileTools.GetMD5(savePath);
                        if (string.Compare(item.itemSrc.fileMd5, fileMd5) == 0)
                        {
                            isFailed = false;
                        }
                    }
                }

                if (isFailed)
                {
                    urlPaths?.Add(urlPath);
                    savePaths?.Add(savePath);

                    isVerify = false;
                    if (urlPaths == null && savePaths == null)
                        break;
                }
            }

            return isVerify;
        }

        //TODO:需要移除废弃文件
        private void UpdateFiles(int packageIndex, string srcAssetPaths, bool isMove = false)
        {
            var package = _updateList.GetPackage(packageIndex);

            var pathsList = package.GetSaveList();
            for (int i = 0; i < pathsList.Count; ++i)
            {
                var savePath = pathsList[i];
                var item = _updateList.GetItem(savePath);
                if (item != null)
                {
                    string destPath = Path.Combine(srcAssetPaths, item.itemSrc.filePath);
                    if (File.Exists(savePath))
                    {
                        if (File.Exists(destPath))
                            File.Delete(destPath);

                        if (isMove) File.Move(savePath, destPath);
                        else File.Copy(savePath, destPath);
                    }
                }
            }
        }
    }
}
