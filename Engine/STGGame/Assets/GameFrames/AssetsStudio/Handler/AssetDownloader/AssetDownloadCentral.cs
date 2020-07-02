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

        private Dictionary<string, AssetDownloadTask> m_tasksMap;               //所有任务列表
        private SortedSet<AssetDownloadTask> m_queueMap;                        //排队队列(含优先级
        private HashSet<AssetDownloadTask> m_progressMap;                       //下载队列
        private HashSet<AssetDownloadTask> m_pauseMap;                          //停止队列

        private LinkedList<AssetDownloadTask> m_successQueue;                   //成功队列
        private LinkedList<AssetDownloadTask> m_failedQueue;                    //失败队列
        private LinkedList<AssetDownloadTask> m_releaseQueue;                   //释放队列

        public int TaskCount
        {
            get { return m_queueMap.Count + m_progressMap.Count + m_pauseMap.Count; }
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
                if (m_tasksMap != null && m_tasksMap.TryGetValue(urlPath, out var taskInMap))
                {
                    return taskInMap;
                }

                var task = GetOrCreateTask();
                task.urlPath = urlPath;
                task.storePath = storePath;
                task.createTime = XTimeTools.NowTimeStampMs();
                GetTaskMap().Add(urlPath, task);

                //默认全部送到暂停队列,方便以后开启空闲下载
                GetPauseMap().Add(task);
                task.status = AssetDownloadStatus.DOWNLOAD_PAUSE;
            }
            return null;
        }

        //TODO:
        public void StartTask(AssetDownloadTask task)
        {
            if (m_queueMap != null && m_queueMap.Contains(task))
            {
                //如果本来就在排队,提高优先级
                task.status = AssetDownloadStatus.DOWNLOAD_QUEUE;
            }
            else if (m_pauseMap != null && m_pauseMap.Contains(task))
            {
                //从暂停队列移到排队队列
                m_pauseMap.Remove(task);
                GetQueueMap().Add(task);
            }
        }

        //TODO:
        public void StopTask(AssetDownloadTask task)
        {
            if (m_progressMap.Contains(task) || m_queueMap.Contains(task))
            {
                DisactiveTask(task);
                m_progressMap.Remove(task);
                m_queueMap.Remove(task);
                GetPauseMap().Add(task);
            }
        }

        public void RemoveTask(AssetDownloadTask task)
        {
            if (m_queueMap != null) m_queueMap.Remove(task);
            if (m_pauseMap != null) m_pauseMap.Remove(task);
            if (m_progressMap != null) m_progressMap.Remove(task);

            if (m_tasksMap != null)
            {
                if (m_tasksMap.ContainsKey(task.urlPath))
                {
                    DisactiveTask(task);
                    m_tasksMap.Remove(task.urlPath);
                }
                
            }
        }

        public void StartAll()
        {
            //所有
            foreach(var task in m_pauseMap)
            {
                StartTask(task);
            }

        }

        public void StopAll()
        {
            List<AssetDownloadTask> taskList = new List<AssetDownloadTask>();
            foreach (var taskInQueue in m_queueMap)             //先停止在排队队列中的,不然会被插到下载队列
            {
                StopTask(taskInQueue);
            }
            foreach (var taskInProgress in m_progressMap)       //然后停止下载队列的
            {
                StopTask(taskInProgress);
            }
        }

        public void RemoveAll()
        {
            foreach(var task in m_tasksMap.Values)
            {
                RemoveTask(task);
            }
        }

        /////////////////////////////////////
        private AssetDownloadTask GetOrCreateTask()
        {
            var task = AssetDownloadTaskManager.GetInstance().GetOrCreateTask();
            return task;
        }

        private Dictionary<string, AssetDownloadTask> GetTaskMap()
        {
            m_tasksMap = m_tasksMap ?? new Dictionary<string, AssetDownloadTask>();
            return m_tasksMap;
        }

        private SortedSet<AssetDownloadTask> GetQueueMap()
        {
            m_queueMap = m_queueMap ?? new SortedSet<AssetDownloadTask>();
            return m_queueMap;
        }

        private HashSet<AssetDownloadTask> GetProcressMap()
        {
            m_progressMap = m_progressMap ?? new HashSet<AssetDownloadTask>();
            return m_progressMap;
        }

        private HashSet<AssetDownloadTask> GetPauseMap()
        {
            m_pauseMap = m_pauseMap ?? new HashSet<AssetDownloadTask>();
            return m_pauseMap;
        }

        private LinkedList<AssetDownloadTask> GetSuccessList()
        {
            m_successQueue = m_successQueue ?? new LinkedList<AssetDownloadTask>();
            return m_successQueue;
        }

        private LinkedList<AssetDownloadTask> GetFailedList()
        {
            m_failedQueue = m_failedQueue ?? new LinkedList<AssetDownloadTask>();
            return m_failedQueue;
        }

        private LinkedList<AssetDownloadTask> GetReleaseList()
        {
            m_releaseQueue = m_releaseQueue ?? new LinkedList<AssetDownloadTask>();
            return m_releaseQueue;
        }

        /////////////////////////////////////

        private void UpdateWait()
        {
            while (m_queueMap.Count > 0 && (maxCount > 0 && m_progressMap.Count < maxCount))
            {
                AssetDownloadTask task = null;
                foreach(var item in m_queueMap)
                {
                    task = item;
                    break;
                }

                m_queueMap.Remove(task);

                ActiveTask(task);
                GetProcressMap().Add(task);
                task.status = AssetDownloadStatus.DOWNLOAD_DOWNLOADING;
            }
        }

        private void UpdateProgress()
        {
            if (m_progressMap != null)
            {
                foreach (var task in m_progressMap)
                {
                    if (task.status == AssetDownloadStatus.DOWNLOAD_FINISH)
                    {
                        GetSuccessList().AddLast(task);
                    }
                    else if(task.status < AssetDownloadStatus.DOWNLOAD_NONE)
                    {
                        GetFailedList().AddLast(task);
                    }
                }

                if (m_successQueue != null) foreach (var taskInSuccess in m_successQueue) m_progressMap.Remove(taskInSuccess);
                if (m_failedQueue != null) foreach (var taskIFailed in m_failedQueue) m_progressMap.Remove(taskIFailed);
            }
        }

        private void UpdateSuccess()
        {
            while (m_successQueue != null && m_successQueue.Count > 0)
            {
                var task = m_successQueue.Last.Value;
                m_successQueue.RemoveLast();

                OnDownloadSuccess(task);
                GetReleaseList().AddLast(task);
            }
        }

        private void UpdateFailed()
        {
            while (m_failedQueue != null && m_failedQueue.Count > 0)
            {
                var task = m_failedQueue.Last.Value;
                m_failedQueue.RemoveLast();

                OnDownloadFailed(task);
                GetReleaseList().AddLast(task);
            }
        }

        private void UpdateRelease()
        {
            while (m_releaseQueue != null && m_releaseQueue.Count > 0)
            {
                var task = m_releaseQueue.Last.Value;
                m_releaseQueue.RemoveLast();

                m_tasksMap.Remove(task.urlPath);
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
        ///
        protected void OnDownloadSuccess(AssetDownloadTask task)
        {
            //将临时文件移动到持久目录
        }
        protected void OnDownloadFailed(AssetDownloadTask task)
        {
            //移除无效的临时文件
        }


    }

}
