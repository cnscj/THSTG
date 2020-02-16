
namespace ASGame
{
    public class AssetLoadHandler
    {
        public string path;
        public AssetLoadCompleted onCompleted;
        public AssetLoadProgress onProgress;

        /// <summary>
        /// 开始加载资源
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// 中断资源加载,只对异步有效
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Clear()
        {
            Stop();     //停止加载

            path = null;
            onCompleted = null;
            onProgress = null;
        }
    }
}
