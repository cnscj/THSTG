namespace ASGame
{
    public class AssetLoadStatus
    {
        public static readonly int LOAD_ERROR = -103;
        public static readonly int LOAD_ABORT = -102;
        public static readonly int LOAD_TIMEOUT = -101;
        public static readonly int LOAD_IDLE = 0;
        public static readonly int LOAD_WAIT = 101;
        public static readonly int LOAD_LOADING = 102;
        public static readonly int LOAD_FINISHED = 200;
    }

}
