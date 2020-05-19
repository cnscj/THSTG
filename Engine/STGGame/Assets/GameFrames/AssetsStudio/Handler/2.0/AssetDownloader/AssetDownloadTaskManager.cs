
using XLibGame;
using XLibrary.Package;

namespace ASGame
{
    public class AssetDownloadTaskManager : MonoSingleton<AssetDownloadTaskManager>
    {
        private ObjectPool<AssetDownloadTask> m_taskPool = ObjectPoolManager.GetInstance().GetOrCreatePool<AssetDownloadTask>();

        public AssetDownloadTask GetOrCreateTask()
        {
            return m_taskPool.GetOrCreate();
        }

        public void RecycleTask(AssetDownloadTask task)
        {
            m_taskPool.Release(task);
        }
    }

}
