using Object = System.Object;
namespace ASGame
{
    public class AssetLoadResult
    {
        public static readonly AssetLoadResult EMPTY_RESULT = new AssetLoadResult(null,false);

        public readonly Object asset;
        public readonly bool isDone;

        public AssetLoadResult(Object asset, bool isDone)
        {
            this.asset = asset;
            this.isDone = isDone;
        }
        public T GetAsset<T>() where T : class
        {
            return asset as T;
        }
    }
}
