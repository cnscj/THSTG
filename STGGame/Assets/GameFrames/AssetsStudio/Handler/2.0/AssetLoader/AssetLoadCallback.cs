using Object = UnityEngine.Object;
namespace ASGame
{
    public delegate void LoadCompleted(Object obj);  //加载完成回调
    public delegate void LoadProgress(float val);    //处理回调
}
