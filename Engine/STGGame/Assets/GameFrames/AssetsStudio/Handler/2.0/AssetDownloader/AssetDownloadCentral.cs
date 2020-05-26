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
        public float limidSpeed = -1f;

        private Dictionary<string, AssetDownloadTask> m_waitMap = new Dictionary<string, AssetDownloadTask>();            //排队队列(含优先级
        private Dictionary<string, AssetDownloadTask> m_progressMap = new Dictionary<string, AssetDownloadTask>();          //下载队列
        private Dictionary<string, AssetDownloadTask> m_stopMap = new Dictionary<string, AssetDownloadTask>();              //停止队列

        private LinkedList<AssetDownloadTask> m_successQueue = new LinkedList<AssetDownloadTask>();               //成功队列
        private LinkedList<AssetDownloadTask> m_failedQueue = new LinkedList<AssetDownloadTask>();                //失败队列
        private LinkedList<AssetDownloadTask> m_releaseQueue = new LinkedList<AssetDownloadTask>();               //释放队列

        public int TaskCount
        {
            get { return m_waitMap.Count + m_progressMap.Count + m_stopMap.Count; }
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
                if (m_waitMap != null && m_waitMap.TryGetValue(urlPath, out var waitTask))
                {
                    return waitTask;
                }

                if (m_progressMap != null && m_progressMap.TryGetValue(urlPath, out var progressTask))
                {
                    return progressTask;
                }

                if (m_stopMap != null && m_stopMap.TryGetValue(urlPath, out var stopTask))
                {
                    return stopTask;
                }

                var task = GetOrCreateTask();
                task.urlPath = urlPath;
                task.storePath = storePath;
                task.createTime = XTimeTools.NowTimeStampMs();


                //默认全部送到暂停队列,方便以后开启空闲下载
                GetStopMap().Add(urlPath, task);
                task.status = AssetDownloadStatus.DOWNLOAD_PAUSE;
            }
            return null;
        }


        public void StartTask(AssetDownloadTask task)
        {
            if (m_waitMap.ContainsKey(task.urlPath))
            {
                task.status = AssetDownloadStatus.DOWNLOAD_QUEUE;
            }
            else if (m_stopMap.ContainsKey(task.urlPath))
            {
                m_stopMap.Remove(task.urlPath);
                m_waitMap.Add(task.urlPath, task);
            }
        }

        public void StopTask(AssetDownloadTask task)
        {
            if (m_progressMap.ContainsValue(task))
            {

            }
            else if (m_waitMap.ContainsValue(task))
            {

            }
        }

        public void CancelTask(AssetDownloadTask task)
        {
            if (m_waitMap.ContainsValue(task))
            {

            }

            if (m_stopMap.ContainsValue(task))
            {

            }

            if (m_progressMap.ContainsValue(task))
            {

            }
        }

        public void StartAll()
        {
            //所有
        }

        public void StopAll()
        {
            //
        }

        public void CancelAll()
        {
            //
        }

        /////////////////////////////////////
        private AssetDownloadTask GetOrCreateTask()
        {
            var task = AssetDownloadTaskManager.GetInstance().GetOrCreateTask();

            return task;
        }

        private Dictionary<string, AssetDownloadTask> GetWaitMap()
        {
            return null;
        }

        private Dictionary<string, AssetDownloadTask> GetProcressMap()
        {
            return null;
        }

        private Dictionary<string, AssetDownloadTask> GetStopMap()
        {
            return null;
        }

        /////////////////////////////////////

        private void UpdateWait()
        {
            while (m_waitMap.Count > 0 && (maxCount > 0 && m_progressMap.Count < maxCount))
            {
                AssetDownloadTask task = null;
                foreach(var itemPair in m_waitMap)
                {
                    task = itemPair.Value;
                    break;
                }
                m_waitMap.Remove(task.urlPath);
                m_progressMap.Add(task.urlPath, task);
                task.status = AssetDownloadStatus.DOWNLOAD_DOWNLOADING;

                ActiveTask(task);
            }

        }

        private void UpdateProgress()
        {
            if (m_progressMap != null)
            {
                foreach (var task in m_progressMap.Values)
                {
                    if (task.status == AssetDownloadStatus.DOWNLOAD_FINISH)
                    {
                        m_successQueue.AddLast(task);
                    }
                    else if(task.status < AssetDownloadStatus.DOWNLOAD_NONE)
                    {
                        m_failedQueue.AddLast(task);
                    }
                }

                foreach (var taskInSuccess in m_successQueue) m_progressMap.Remove(taskInSuccess.urlPath);
                foreach (var taskIFailed in m_successQueue) m_progressMap.Remove(taskIFailed.urlPath);
            }
        }

        private void UpdateSuccess()
        {
            if (m_successQueue != null)
            {
                var task = m_successQueue.Last.Value;
                m_successQueue.RemoveLast();


            }
        }

        private void UpdateFailed()
        {
            if (m_failedQueue != null)
            {
                var task = m_failedQueue.Last.Value;
                m_failedQueue.RemoveLast();


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

        //激活任务,正式开始下载
        private void ActiveTask(AssetDownloadTask task)
        {

        }

        //失活任务,暂停下载
        private void DisactiveTask(AssetDownloadTask task)
        {

        }

        private void ReleaseTask(AssetDownloadTask task)
        {
            AssetDownloadTaskManager.GetInstance().RecycleTask(task);
        }

        //Android目录下,下载中的存放路径必须在Application.dataPath,下载完成在拷贝到Application.persistentDataPath
    }

}
