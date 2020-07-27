
namespace ASGame
{
    public static class AssetDownloadStatus
    {
        public static readonly int DOWNLOAD_UNKNOWN_ERROR = -106;
        public static readonly int DOWNLOAD_SPACE_IS_NOT_ENOUGH = -105;
        public static readonly int DOWNLOAD_WRITE_FAILED = -104;
        public static readonly int DOWNLOAD_CANCELED = -103;
        public static readonly int DOWNLOAD_TIMEOUT = -102;
        public static readonly int DOWNLOAD_INVALID_URL = -101;

        public static readonly int DOWNLOAD_NONE = 0;

        public static readonly int DOWNLOAD_PAUSE = 101;
        public static readonly int DOWNLOAD_QUEUE = 102;
        public static readonly int DOWNLOAD_DOWNLOADING = 103;

        public static readonly int DOWNLOAD_FINISH = 200;
    }

}
