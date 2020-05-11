using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class DownloadManager : MonoBehaviour
    {
        private static DownloadManager s_instance = null;
        public static DownloadManager instance
        {
            get
            {
                return s_instance;
            }
        }

        /// <summary>
        /// 最大请求(HttpWebRequest)数量
        /// 
        /// NOTE:
        /// http 1.0 和 http 1.1 标准规定的并发连接数为2，超过这个值，超时就很频繁了
        /// 
        /// 设置 System.Net.ServicePointManager.DefaultConnectionLimit = 512; 可以突破这个限制
        /// </summary>
        private int m_maxTaskCount = 4;
        public int maxTaskCount 
        {
            get { return m_maxTaskCount; }
            set 
            { 
                if (value > 0) 
                    m_maxTaskCount = value; 
            }
        }

        // 后台下载队列的最大数量，不能超过maxTaskCount
        private int m_maxBgTaskCount = 4;
        public int maxBgTaskCount 
        {
            get { return m_maxBgTaskCount; }
            set
            {
                if (value <= m_maxTaskCount && value >= 0)
                    m_maxBgTaskCount = value;
            }
        }

        /// <summary>
        /// 总下载限速，字节/秒
        /// 简单处理，所有正在进行的任务限速平均化
        /// </summary>
        public double totalDownloadSpeedLimit { get; set; } = 0d;


        // 工作队列和等待队列，用链表是因为有从中间位置移除结点的操作，还有插入到队列头和尾的需要。
        // FindDownloadTaskInQueue()查找的效率并不高，是O(n)，用其它链状的数据结构也是这么多。保守估计m_waitingTaskList队列的最大长度为3位数，都是需要排队静默下载的资源。
        // 如果效率实在太差，可以再引入1个字典，专门用来查找m_waitingTaskList。
        LinkedList<DownloadTask> m_runningTaskQueue = new LinkedList<DownloadTask>();
        LinkedList<DownloadTask> m_waitingTaskQueue = new LinkedList<DownloadTask>();
        List<DownloadTask> m_curRunningTasks = null;


        void Awake()
        {
            s_instance = this;

            UnlimitConnectionCount();
            UseMaxProcessorCount();

            m_curRunningTasks = new List<DownloadTask>(Mathf.Max(1, 2*maxTaskCount));
        }

        // 全速下载
        public void UseMaxProcessorCount()
        {
            m_maxTaskCount = System.Environment.ProcessorCount;
            if (m_maxTaskCount <= 0)
                m_maxTaskCount = 1;
            if (m_maxBgTaskCount > m_maxTaskCount)
                m_maxBgTaskCount = m_maxTaskCount;

            Debug.LogFormat("System.Environment.ProcessorCount = {0}", m_maxTaskCount);
        }

        // 留2个核给HttpClient，那边也有上传和下载需求，相对来说，那边的请求会更重要。
        public void UseSoftProcessorCount()
        {
            m_maxTaskCount = System.Environment.ProcessorCount - 2;
            if (m_maxTaskCount <= 0)
                m_maxTaskCount = 1;
            if (m_maxBgTaskCount > m_maxTaskCount)
                m_maxBgTaskCount = m_maxTaskCount;
        }


        int runningRequestCount = 0;
        int runningBgRequestCount = 0;

        void Update()
        {
            //请求队列，应该按顺序插入和删除，倒序不合要求
            runningRequestCount = 0;
            runningBgRequestCount = 0;

            if (m_runningTaskQueue.Count > 0)
            {
                if (m_curRunningTasks.Count > 0)
                    m_curRunningTasks.Clear();

                DownloadTask canRunRequest = null;

                var current = m_runningTaskQueue.First;
                while(current != null)
                {
                    var next = current.Next;

                    DownloadTask task = current.Value;
                    if (task.isDone)
                    {
                        task.Dispose();

                        m_runningTaskQueue.Remove(current);
                    }
                    else
                    {
                        // 只要在主队列中的，不管有没有开始，都计数。需要把优先级让给原本就在主队列中的请求。
                        if (task.priority == HttpTask.EPriority.Background)
                            runningBgRequestCount++;

                        if (task.state == DownloadTask.ETaskState.Running)
                        {
                            // task.Update();
                            m_curRunningTasks.Add(task);

                            runningRequestCount++;
                        }
                        else
                        {
                            if (canRunRequest == null)
                            {
                                canRunRequest = task;
                            }
                        }
                    }
                    
                    current = next;
                }

                if (canRunRequest != null && runningRequestCount < maxTaskCount && isNetworkEnable())
                {
                    StartCoroutine(canRunRequest.Start());
                    m_curRunningTasks.Add(canRunRequest);
                }

                // 限速，所有正在进行的任务平均一下
                if (m_curRunningTasks.Count > 0 && totalDownloadSpeedLimit > 0d)
                {
                    double averageDownloadSpeedLimit = totalDownloadSpeedLimit / m_curRunningTasks.Count;
                    foreach(var task in m_curRunningTasks)
                    {
                        task.downloadSpeedLimit = averageDownloadSpeedLimit;
                    }
                }
            }

            // 主队列空闲了才开始后台队列的请求
            if (runningRequestCount < maxTaskCount && m_runningTaskQueue.Count == 0)
            {
                // 主队列一帧只开始一个请求，这边也一帧开始一个请求好了。等待队列中的Task可能被Cancel了，这边可以忽略。
                if (m_waitingTaskQueue.Count > 0)
                {
                    var current = m_waitingTaskQueue.First;
                    while(current != null)
                    {
                        var next = current.Next;

                        DownloadTask task = current.Value;
                        m_waitingTaskQueue.Remove(current);

                        if (task.isDone)
                        {
                            task.Dispose();
                        }
                        else
                        {
                            break;
                        }
                        current = next;
                    }

                    // 等待队列中的任务有数量限制，超了就一直等吧
                    if (current != null && runningBgRequestCount < maxBgTaskCount)
                    {
                        m_runningTaskQueue.AddLast(current);
                    }
                }
            }
        }

        void OnDestroy()
        {
            ClearAllTask(true);
        }

        void UnlimitConnectionCount()
        {
            // 设置这个可以突破 http 1.0 和 http 1.1 标准规定的并发连接数2
            System.Net.ServicePointManager.DefaultConnectionLimit = 512;
        }

        // 网络是否可用
        bool isNetworkEnable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public void ClearAllTask(bool cancelRunningTasks)
        {
            foreach (var task in m_waitingTaskQueue)
            {
                task.Dispose();
            }
            m_waitingTaskQueue.Clear();

            // 如果想保留正在下载的Task，就传false
            var current = m_runningTaskQueue.First;
            while (current != null)
            {
                var next = current.Next;

                DownloadTask task = current.Value;
                if (!task.isDone && task.state == DownloadTask.ETaskState.Running && !cancelRunningTasks)
                {
                    // 只是清除，任务还照常跑，要存文件的就自己存
                    task.ClearCallback();
                }
                else
                {
                    task.Dispose();
                    m_runningTaskQueue.Remove(current);
                }

                current = next;
            }

            m_curRunningTasks.Clear();
        }

        //GET
        public DownloadTask DownloadFile(string host, string relativePath, string fileFullPath, int timeout, HttpCallback callback, bool topPriority = false, bool isAssetBundle = false, string matchMd5 = null)
        {
            return AddDownloadTask(false, host, relativePath, fileFullPath, timeout, callback, topPriority, isAssetBundle, matchMd5);
        }

        //GET
        public DownloadTask DownloadFileInBackground(string host, string relativePath, string fileFullPath, int timeout, HttpCallback callback, bool topPriority = false, bool isAssetBundle = false, string matchMd5 = null)
        {
            return AddDownloadTask(true, host, relativePath, fileFullPath, timeout, callback, topPriority, isAssetBundle, matchMd5);
        }

        private static LinkedListNode<T> FindNoneInQueue<T>(LinkedList<T> queue, System.Predicate<LinkedListNode<T>> match)
        {
            if (queue.Count == 0 || match == null)
                return null;

            // 迭代器遍历LinkedListNode拿到的是Node的值，而不是Node，所以换种方式
            LinkedListNode<T> current = queue.First;
            while( current != null )
            {
                if ( match(current) )
                {
                    break;
                }

                current = current.Next;
            }

            return current;
        }

        // 已经明确需求的情况下，还是不用Predicate的形式，节省大量委托的创建
        private static LinkedListNode<DownloadTask> FindDownloadTaskInQueue(LinkedList<DownloadTask> queue, string relativePath)
        {
            if (queue.Count == 0 || string.IsNullOrEmpty(relativePath))
                return null;

            // 迭代器遍历LinkedListNode拿到的是Node的值，而不是Node，所以换种方式
            LinkedListNode<DownloadTask> current = queue.First;
            while( current != null )
            {
                if ( current.Value.parameters.relativePath == relativePath )  //match
                {
                    break;
                }

                current = current.Next;
            }

            return current;
        }


        /// <summary>
        /// 调整现有的任务队列和顺序
        /// </summary>
        /// <param name="host"></param>
        /// <param name="relativePath"></param>
        /// <param name="callback"></param>
        /// <param name="inBackground"></param>
        /// <param name="topPriority"></param>
        private DownloadTask AdjustDownloadRequestTaskInCurrentQueues(string host, string relativePath, HttpCallback callback, bool inBackground, bool topPriority)
        {
            DownloadTask requestTask = null;

            // 已经在主队列中的，直接返回
            LinkedListNode<DownloadTask> node = FindDownloadTaskInQueue(m_runningTaskQueue, relativePath);
            if (node != null)
            {
                requestTask = node.Value;

                requestTask.parameters.callback -= callback;
                requestTask.parameters.callback += callback;

                if (!inBackground)
                {
                    // 移到最前面
                    if (topPriority)
                    {
                        if (node != m_runningTaskQueue.First)
                        {
                            m_runningTaskQueue.Remove(node);
                            m_runningTaskQueue.AddFirst(node);
                        }
                    }
                }
                else
                {
                    Debug.LogFormat("主队列中的请求想移动到后台队列？");
                }
                return requestTask;
            }
            else
            {
                // 在等待队列中
                node = FindDownloadTaskInQueue(m_waitingTaskQueue, relativePath);
                if (node != null)
                {
                    requestTask = node.Value;

                    requestTask.parameters.callback -= callback;
                    requestTask.parameters.callback += callback;

                    // 移到主队列中去
                    if (!inBackground)
                    {
                        // 优先级提升
                        requestTask.priority = HttpTask.EPriority.Main;

                        m_waitingTaskQueue.Remove(node);

                        if (topPriority)
                        {
                            m_runningTaskQueue.AddFirst(node);
                        }
                        else
                        {
                            m_runningTaskQueue.AddLast(node);
                        }
                    }
                    return requestTask;
                }
            }

            return requestTask;
        }

        private DownloadTask CreateDownloadTask(DownloadTaskParameters param, bool inBackground)
        {
            DownloadTask task = new DownloadTask();
            task.Init(param);  // 还有1个参数应该是全局设置

            if (inBackground)
                task.priority = HttpTask.EPriority.Background;
            else
                task.priority = HttpTask.EPriority.Main;

            return task;
        }

        private bool AddDownloadTaskToQueue(DownloadTask task, bool inBackground, bool topPriority)
        {
            if (task == null)
                return false;

            LinkedList<DownloadTask> queueToAdd = null;
            if (inBackground)
            {
                queueToAdd = m_waitingTaskQueue;
            }
            else
            {
                queueToAdd = m_runningTaskQueue;
            }

            if (topPriority)
            {
                queueToAdd.AddFirst(task);
            }
            else
            {
                queueToAdd.AddLast(task);
            }

            return true;
        }


        /// <summary>
        /// 避免参数过多而加的方法，可以给Lua调用
        /// </summary>
        /// <param name="param"></param>
        /// <param name="inBackground"></param>
        /// <param name="topPriority"></param>
        /// <returns></returns>
        public DownloadTask AddDownloadTask(DownloadTaskParameters param, bool inBackground, bool topPriority = false)
        {
            if (param == null)
            {
                return null;
            }

            DownloadTask requestTask = AdjustDownloadRequestTaskInCurrentQueues(param.cdnDomain, param.relativePath, param.callback, inBackground, topPriority);
            if (requestTask == null)
            {
                requestTask = CreateDownloadTask(param, inBackground);

                AddDownloadTaskToQueue(requestTask, inBackground, topPriority);
            }

            return requestTask;
        }

        // 把url拆分成host跟relativePath，主要是因为Task在等待时host可能被修改，这时如果用新的host再尝试下载，就会出现新的Task下载同一个文件，而如果其中一个Task下载失败，可能会把成功的那个Task下载的文件删除。
        // 所以，下载同一个文件，只能有一个Task。
        public DownloadTask AddDownloadTask(bool inBackground, string host, string relativePath, string fileFullPath, int timeout, HttpCallback callback, bool topPriority, bool isAssetBundle, string matchMd5)
        {
            DownloadTask requestTask = AdjustDownloadRequestTaskInCurrentQueues(host, relativePath, callback, inBackground, topPriority);
            if (requestTask == null)
            {
                DownloadTaskParameters param = new DownloadTaskParameters();
                param.cdnDomain = host;
                param.relativePath = relativePath;
                param.saveFileFullPath = fileFullPath;
                param.callback = callback;
                param.isAssetBundle = isAssetBundle;
                param.md5 = matchMd5;
                if (timeout > 0)
                    param.timeout = timeout;

                requestTask = CreateDownloadTask(param, inBackground);

                AddDownloadTaskToQueue(requestTask, inBackground, topPriority);
            }

            return requestTask;
        }

        /// <summary>
        /// 已经失败的请求，重置之后又重新请求.
        /// </summary>
        public bool ReAddDownloadTask(DownloadTask requestTask, bool topPriority = false)
        {
            Debug.LogFormat(">> 尝试将requestTask重新加入下载队列...");
            if (requestTask == null)
                return false;

            if ( FindDownloadTaskInQueue(m_runningTaskQueue, requestTask.parameters.relativePath) != null )
            {
                Debug.LogFormat(">> requestTask已经在工作队列当中了，不用再处理，保持当前的顺序.");
                return false;
            }

            if (topPriority)
            {
                m_runningTaskQueue.AddFirst(requestTask);
            }
            else
            {
                m_runningTaskQueue.AddLast(requestTask);
            }
            return true;
        }

        public void CancelDownloadTask(DownloadTask task)
        {
            if (task == null || task.isDone)
            {
                return;
            }
            task.Cancel();
        }

        public void DisposeDownloadTask(DownloadTask task)
        {
            if (task != null)
            {
                task.Dispose();
            }
        }
    }

}
