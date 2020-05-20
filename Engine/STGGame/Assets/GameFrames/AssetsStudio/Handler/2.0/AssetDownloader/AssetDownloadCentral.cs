using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary;

namespace ASGame
{
    //断点续传,下载记录(可用MD5等
    //下载优先级
    public class AssetDownloadCentral : MonoBehaviour
    {
        //全局限速
        //全局最大任务下载数
        //定时任务
        public int maxCount = -1;
        public float limitSpeed = -1f;

        private Dictionary<string, AssetDownloadTask> m_waitQueue = new Dictionary<string, AssetDownloadTask>();         //排队队列
        private Dictionary<string, AssetDownloadTask> m_progressMap = new Dictionary<string, AssetDownloadTask>();       //下载队列
        private Dictionary<string, AssetDownloadTask> m_stopMap = new Dictionary<string, AssetDownloadTask>();           //停止队列

        private LinkedList<AssetDownloadTask> m_successQueue = new LinkedList<AssetDownloadTask>();               //成功队列
        private LinkedList<AssetDownloadTask> m_failedQueue = new LinkedList<AssetDownloadTask>();                //失败队列
        private LinkedList<AssetDownloadTask> m_releaseQueue = new LinkedList<AssetDownloadTask>();               //释放队列

        public int TaskCount
        {
            get { return m_waitQueue.Count + m_progressMap.Count + m_stopMap.Count; }
        }

        public int DownloadingCount
        {
            get { return m_progressMap.Count; }
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

        public AssetDownloadTask NewTask(string urlPath, string storePath)
        {
            if (!string.IsNullOrEmpty(urlPath) && !string.IsNullOrEmpty(storePath))
            {
                //TODO:判断是否已经在队列中,是的话,下载优先级往前提


                var task = GetOrCreateTask();
                task.urlPath = urlPath;
                task.storePath = storePath;
                task.createTime = XTimeTools.NowTimeStampMs();


            }
            return null;
        }


        public void StartTask(AssetDownloadTask task)
        {

        }

        public void StopTask(AssetDownloadTask task)
        {

        }

        public void CancelTask(AssetDownloadTask task)
        {

        }

        public void StartAll()
        {

        }

        public void StopAll()
        {

        }

        public void CancelAll()
        {

        }
        /////////////////////////////////////

        private AssetDownloadTask GetOrCreateTask()
        {
            var task = AssetDownloadTaskManager.GetInstance().GetOrCreateTask();

            return task;
        }

        private void ReleaseTask(AssetDownloadTask task)
        {
            AssetDownloadTaskManager.GetInstance().RecycleTask(task);
        }

        /////////////////////////////////////

        private void UpdateWait()
        {
            
        }

        private void UpdateProgress()
        {
            if (m_progressMap != null)
            {
                foreach (var task in m_progressMap.Values)
                {

                }
            }
        }

        private void UpdateSuccess()
        {
            if (m_successQueue != null)
            {
                foreach (var task in m_successQueue)
                {

                }
            }
        }

        private void UpdateFailed()
        {
            if (m_failedQueue != null)
            {
                foreach (var task in m_failedQueue)
                {

                }
            }
        }

        private void UpdateRelease()
        {
            while(m_releaseQueue.Count > 0)
            {
                var task = m_releaseQueue.Last.Value;
                m_releaseQueue.RemoveLast();

                ReleaseTask(task);
            }
        }

        private void Update()
        {
            UpdateWait();
            UpdateProgress();

            UpdateSuccess();
            UpdateFailed();
            UpdateRelease();
        }

        ///

        //Android目录下,下载中的存放路径必须在Application.dataPath,下载完成在拷贝到Application.persistentDataPath
    }

}
