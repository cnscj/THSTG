using System;
using System.Collections.Generic;

namespace ASGame
{

    public class AssetUpdateUpdater
    {
        public void Update(AssetUpdateUpdateList.Package package)
        {
            var downloader = AssetDownloadManager.GetInstance().GetForegroundCentral();

        }

        private void OnCompleted()
        {

        }
    }
}
