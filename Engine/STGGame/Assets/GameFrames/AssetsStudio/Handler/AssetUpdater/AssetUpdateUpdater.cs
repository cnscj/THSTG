using System;
using System.IO;

namespace ASGame
{
    public class AssetUpdateUpdater
    {
        public Action<AssetDownloadTask> onDownloadCompleted;
        public Action<float> onDownloadProgress;

        public Action onFinish;

        private AssetUpdateUpdateList _updateList;
        private AssetDownloadTask _downloadTask;
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

        public bool IsDiskspaceEnough(int packageIndex = 0)
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

        public long GetUpdateNeedSize(int packageIndex = 0)
        {
            var package = _updateList.GetPackage(packageIndex);
            if (package != null)
            {
                return package.size;
            }
            return 0;
        }

        public void Update(int packageIndex = 0)
        {
            var updateUrls = _updateList.GetUrlList(packageIndex);
            var updateSaves = _updateList.GetSaveList(packageIndex);

            var downloader = AssetDownloadManager.GetInstance().GetForegroundCentral();

            _downloadTask = downloader.NewTask(updateUrls, updateSaves);
            _downloadTask.onCompleted += (task)=>
            {
                onDownloadCompleted?.Invoke(task);
                var package = _updateList.GetPackage(packageIndex);
                OnDownloadCompleted(package);
            };
        }

        protected void OnDownloadCompleted(AssetUpdateUpdateList.Package package)
        {
            //TODO:校验文件完整性
            string saveFolder = _updateList.GetSaveFolderPath(package.index);

            //完整性通过,开始拷贝文件

            //没通过的文件重新下载

            onFinish?.Invoke();
        }

        private void VerifyFiles()
        {

        }

    }
}
