using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace ASGame
{
    //断点续传,下载记录(可用MD5等
    //下载优先级
    public class AssetDownloadCentral : MonoBehaviour
    {
        //全局限速
        //全局最大任务下载数
        //定时任务
        private LinkedList<AssetDownloadTask> m_waitQueue = new LinkedList<AssetDownloadTask>();                            //等待队列
        private Dictionary<string, AssetDownloadTask> m_downloadingMap = new Dictionary<string, AssetDownloadTask>();       //加载队列

        private LinkedList<AssetDownloadTask> m_successQueue = new LinkedList<AssetDownloadTask>();               //成功队列
        private LinkedList<AssetDownloadTask> m_failedQueue = new LinkedList<AssetDownloadTask>();                //失败队列
        private LinkedList<AssetDownloadTask> m_releaseQueue = new LinkedList<AssetDownloadTask>();               //释放队列

        private ObjectPool<AssetDownloadTask> m_taskPool = ObjectPoolManager.GetInstance().GetOrCreatePool<AssetDownloadTask>();

        public int TaskCount
        {
            get { return m_waitQueue.Count + m_downloadingMap.Count; }
        }

        public int DownloadingCount
        {
            get { return m_downloadingMap.Count; }
        }

        public long TaskVolume
        {
            get { return 0; }
        }

        public long DownloadedVolume
        {
            get { return 0; }
        }

        public long TotalVolume
        {
            get { return 0; }
        }

        /////////////////////////////////////

        public AssetDownloadTask NewTask(string urlPath)
        {

            return null;
        }


        public void StartTask(AssetDownloadTask task)
        {

        }

        public void StopTask(AssetDownloadTask task)
        {

        }

        public void RemoveTask(AssetDownloadTask task)
        {

        }

        public void ClearAll()
        {

        }
        /////////////////////////////////////

        private AssetDownloadTask GetOrCreateTask()
        {
            return m_taskPool.GetOrCreate();
        }


        /////////////////////////////////////
    }

}
