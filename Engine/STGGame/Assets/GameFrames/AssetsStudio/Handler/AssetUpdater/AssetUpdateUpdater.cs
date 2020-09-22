using System;
using System.Collections.Generic;

namespace ASGame
{
    public class AssetUpdateUpdater
    {
        public Action<AssetDownloadTask> onCompleted;

        private AssetUpdateUpdateList _updateList;
        private AssetDownloadTask _downloadTask;

        public void Update(AssetUpdateUpdateList updateList)
        {
            if (updateList == null)
                return;

            _updateList = updateList;
            var updateUrls = updateList.GetUrlList();
            var updateSaves = updateList.GetSaveList();

            var downloader = AssetDownloadManager.GetInstance().GetForegroundCentral();
            _downloadTask = downloader.NewTask(updateUrls, updateSaves);
            _downloadTask.onCompleted += OnCompleted;
        }

        private void OnCompleted(AssetDownloadTask task)
        {
            var dict = _updateList.GetOrCreateItemDict();

            //TODO:校验文件并更新
            //覆盖并更新
            foreach (var pair in dict)
            {
                
            }

            onCompleted?.Invoke(task);
        }
    }
}
