using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetLoadStatus
    {
        public static readonly int LOAD_SUCCESS = -1;
        public static readonly int LOAD_IDLE = 0;
        public static readonly int LOAD_WAIT = 1;
        public static readonly int LOAD_PREPARE = 2;
        public static readonly int LOAD_LOADING = 3;
        public static readonly int LOAD_FAILED = 4;

    }

}
