using System;
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
        public int maxCount = 3;                                                //最大同时下载任务个数
        public int limidSpeed = -1;                                             //全局限速
        public string saveFolder;                                               //保存路径

        private Dictionary<string, AssetDownloadTask> m_tasksMap;               //所有任务列表
        private SortedSet<AssetDownloadTask> m_queueMap;                        //排队队列(含优先级
        private HashSet<AssetDownloadTask> m_progressMap;                       //下载队列
        private HashSet<AssetDownloadTask> m_pauseMap;                          //停止队列

        private LinkedList<AssetDownloadTask> m_successQueue;                   //成功队列
        private LinkedList<AssetDownloadTask> m_failedQueue;                    //失败队列
        private LinkedList<AssetDownloadTask> m_releaseQueue;                   //释放队列

        private int m_taskId = 0;

        public int TaskCount
        {
            get { return WaitingCount + DownloadingCount + PauseCount; }
        }

        public int WaitingCount
        {
            get { return m_queueMap != null ? m_queueMap.Count : 0; }
        }

        public int PauseCount
        {
            get { return m_pauseMap != null ? m_pauseMap.Count : 0; }
        }

        public int DownloadingCount
        {
            get { return m_progressMap != null ? m_progressMap.Count : 0; }
        }

        public long DownloadedSize
        {
            get
            {
                long total = 0;
                if (m_tasksMap != null)
                {
                    foreach (var task in m_tasksMap.Values)
                    {
                        total += task.CurSize;
                    }
                }

                return total;
            }
        }

        public long TotalSize
        {
            get
            {
                long total = 0;
                if (m_tasksMap != null)
                {
                    foreach (var task in m_tasksMap.Values)
                    {
                        total += task.TotalSize;
                    }
                }

                return total;
            }
        }

        /////////////////////////////////////

        //TODO:从磁盘加载任务
        public AssetDownloadTask LoadTask()
        {
            return null;
        }

        public void SaveTask()
        {
            //生成临时文件
        }

        //临时文件也一并移除
        public void DeleteTask(AssetDownloadTask task)
        {

        }

        public AssetDownloadTask NewTask(string[] urlPaths)
        {
            if (urlPaths != null && urlPaths.Length > 0)
            {
                var taskKey = GetTaskKey(urlPaths);
                if (m_tasksMap != null && m_tasksMap.TryGetValue(taskKey, out var taskInMap))
                {
                    return taskInMap;
                }

                var task = GetOrCreateTask();
                task.urlPaths = urlPaths;
                task.savePaths = GetFileSavePathsByUrls(urlPaths);
                GetTaskMap().Add(taskKey, task);

                //默认全部送到暂停队列,方便以后开启空闲下载
                GetPauseMap().Add(task);
                task.status = AssetDownloadStatus.DOWNLOAD_PAUSE;
                return task;
            }
            return null;
        }

        public void StartTask(AssetDownloadTask task)
        {
            if (m_queueMap != null && m_queueMap.Contains(task))
            {
                //如果本来就在排队,提高优先级
                m_queueMap.Remove(task);
                task.priority++;
                m_queueMap.Add(task);

            }
            else if (m_pauseMap != null && m_pauseMap.Contains(task))
            {
                //从暂停队列移到排队队列
                GetPauseMap().Remove(task);
                GetQueueMap().Add(task);
                task.status = AssetDownloadStatus.DOWNLOAD_QUEUE;
            }
        }

        public void PauseTask(AssetDownloadTask task)
        {
            if ((m_progressMap != null && m_progressMap.Contains(task)) || (m_queueMap != null && m_queueMap.Contains(task)))
            {
                DisactiveTask(task);
                GetProcressMap().Remove(task);
                GetQueueMap().Remove(task);
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
                var taskKey = GetTaskKey(task.urlPaths);
                if (GetTaskMap().ContainsKey(taskKey))
                {
                    GetReleaseList().AddLast(task);
                }
            }
        }

        public bool HadTask(string []urlPaths)
        {
            if (m_tasksMap != null)
            {
                var taskKey = GetTaskKey(urlPaths);
                return m_tasksMap.ContainsKey(taskKey);
            }
            return false;
        }

        public bool HadTask(AssetDownloadTask task)
        {
            return HadTask(task.urlPaths);
        }

        public void StartAll()
        {
            if (m_pauseMap == null || m_pauseMap.Count <= 0)
                return;

            List<AssetDownloadTask> taskList = new List<AssetDownloadTask>();
            taskList.AddRange(m_pauseMap);

            foreach (var task in taskList)
            {
                StartTask(task);
            }
        }

        public void PauseAll()
        {
            List<AssetDownloadTask> taskList = new List<AssetDownloadTask>();
            if (m_queueMap != null) taskList.AddRange(m_queueMap);
            if (m_progressMap != null) taskList.AddRange(m_progressMap);

            foreach (var task in taskList)
            {
                PauseTask(task);
            }

        }

        public void RemoveAll()
        {
            if (m_tasksMap == null || m_tasksMap.Count <= 0)
                return;

            List<AssetDownloadTask> taskList = new List<AssetDownloadTask>();
            taskList.AddRange(m_tasksMap.Values);

            foreach (var task in taskList)
            {
                RemoveTask(task);
            }
        }

        /////////////////////////////////////
        private AssetDownloadTask GetOrCreateTask()
        {
            var task = new AssetDownloadTask();
            task.id = m_taskId++;
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
            while (WaitingCount > 0 && (maxCount > 0 && DownloadingCount < maxCount))
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
            if (m_progressMap == null)
                return;

            foreach (var task in m_progressMap)
            {
                if (task.status == AssetDownloadStatus.DOWNLOAD_COMPLETED)
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

                DestroyTask(task);

                var taskKey = GetTaskKey(task.urlPaths);
                m_tasksMap.Remove(taskKey);
            }
        }

        private string GetTaskKey(string[] urlPaths)
        {
            if (urlPaths == null || urlPaths.Length <= 0)
                return string.Empty;

            int hashCode = 0;
            foreach(var url in urlPaths)
            {
                hashCode += url.GetHashCode();
            }
            return string.Format("{0}", hashCode);
        }

        private void Awake()
        {
            saveFolder = saveFolder ?? CTargetPlat.PersistentRootPath;
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
            if (task == null)
                return;

            task.Start();
        }

        //失活任务,停止下载
        private void DisactiveTask(AssetDownloadTask task)
        {
            if (task == null)
                return;

            task.Pause();
        }

        private void DestroyTask(AssetDownloadTask task)
        {
            if (task == null)
                return;

            task.Stop();
        }

        //Android目录下,下载中的存放路径必须在Application.dataPath,下载完成在拷贝到Application.persistentDataPath
        protected void OnDownloadSuccess(AssetDownloadTask task)
        {
            //将临时文件移动到持久目录
        }
        protected void OnDownloadFailed(AssetDownloadTask task)
        {
            //移除无效的临时文件
        }


        private string GetLocalPathNameByUrl(string url, string saveFolder)
        {
            int nIndex = url.LastIndexOf('/');
            if (nIndex != -1)
            {
                string szFileName = url.Substring(nIndex + 1);
                return string.Format("{0}/{1}", saveFolder, szFileName);
            }
            return string.Empty;
        }

        private string[] GetFileSavePathsByUrls(string[] urlPaths)
        {
            List<string> savePathList = new List<string>();

            foreach(var url in urlPaths)
            {
                savePathList.Add(GetLocalPathNameByUrl(url, saveFolder));
            }

            return savePathList.ToArray();
        }

    }

}
