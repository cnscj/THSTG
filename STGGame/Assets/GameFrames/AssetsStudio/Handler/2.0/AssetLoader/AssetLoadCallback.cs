using Object = UnityEngine.Object;
namespace ASGame
{
    public delegate void AssetLoadCompleted(Object obj);  //加载完成回调
    public delegate void AssetLoadProgress(float val);    //处理回调
}
