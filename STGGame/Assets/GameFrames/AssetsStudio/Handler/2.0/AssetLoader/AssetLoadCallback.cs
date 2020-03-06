using Object = UnityEngine.Object;
namespace ASGame
{
    public delegate void AssetLoadSuccess(Object obj);              //加载完成回调
    public delegate void AssetLoadFailed(int reason);               //加载失败回调
}
