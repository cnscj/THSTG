using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    public static class AssetDownloadStatus
    {
        public static readonly int DOWNLOAD_ASSET_ERROR = -104;
        public static readonly int DOWNLOAD_CENCELED = -103;
        public static readonly int DOWNLOAD_TIMEOUT = -102;
        public static readonly int DOWNLOAD_INVALID_URL = -101;
        public static readonly int DOWNLOAD_IDLE = 0;
        public static readonly int DOWNLOAD_WAIT = 101;
        public static readonly int DOWNLOAD_DOWNLOADING = 102;
        public static readonly int DOWNLOAD_FINISH = 200;
    }

}
