using Object = UnityEngine.Object;
namespace ASGame
{
    public delegate void AssetLoadCompleted(AssetLoadResult result); //加载回调

    public delegate void AssetLoadSuccess(Object obj);              //加载回调
    public delegate void AssetLoadSuccess1<T>(T obj);               //加载回调
    public delegate void AssetLoadSuccess2<T1,T2>(T1 obj1,T1 obj2); //加载回调
    public delegate void AssetLoadFailed(int reason);   //加载回调

    public class AssetLoadCallback<T> where T : Object
    {
        public AssetLoadSuccess1<T> onSuccess;
        public AssetLoadFailed onFailed;
    }
    public class AssetLoadCallback<T1,T2> where T1 : Object where T2 : Object
    {
        public AssetLoadSuccess2<T1,T2> onSuccess;
        public AssetLoadFailed onFailed;
    }
    public class AssetLoadCallback
    {
        public AssetLoadSuccess onSuccess;
        public AssetLoadFailed onFailed;
    }
}
