using Object = UnityEngine.Object;
namespace ASGame
{
    public delegate void DownloadCompleted(Object obj);  //加载完成回调
    public delegate void DownloadProgress(float val);    //处理回调
}
