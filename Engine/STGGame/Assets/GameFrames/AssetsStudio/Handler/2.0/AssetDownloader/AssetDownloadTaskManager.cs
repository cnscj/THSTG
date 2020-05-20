
using XLibGame;
using XLibrary.Package;

namespace ASGame
{
    public class AssetDownloadTaskManager : MonoSingleton<AssetDownloadTaskManager>
    {
        private int m_id;
        private ObjectPool<AssetDownloadTask> m_taskPool = ObjectPoolManager.GetInstance().GetOrCreatePool<AssetDownloadTask>();

        public AssetDownloadTask GetOrCreateTask()
        {
            var task = m_taskPool.GetOrCreate();
            task.Reset();
            task.id = ++m_id;

            return task;
        }

        public void RecycleTask(AssetDownloadTask task)
        {
            m_taskPool.Release(task);
        }
    }

}
