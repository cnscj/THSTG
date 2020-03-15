using Object = UnityEngine.Object;
namespace ASGame
{
    public class AssetLoadResult
    {
        public Object asset;
        public bool isDone;

        public T GetAsset<T>() where T : Object
        {
            return asset as T;
        }
    }
}
