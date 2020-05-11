using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Net;
using System.IO;
using System;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using XLibrary.Package;
using XLibrary;

namespace XLibGame
{
    public delegate void HttpCallback(Dictionary<string, object> dict);

    public class HttpClient : MonoSingleton<HttpClient>
    {
        private static readonly string SEPARATER = "|{&&}|";
        private static readonly int SEPARATER_LEN = SEPARATER.Length;

        // 所有的下载会放到2个队列里去：
        // m_mainRequestList存放当前进行的请求，m_backgroundRequestList存放等待中的请求，当前进行的请求没有之后再从等待队列中去取。
        List<IOTask> m_ioTaskList = new List<IOTask>();
        List<HttpRequestTask> m_mainRequestList = new List<HttpRequestTask>();
        List<HttpRequestTask> m_backgroundRequestList = new List<HttpRequestTask>();

        // int m_currentRunningRequestCount = 0;


        //最大io线程数量
        public int maxIOTask { get; set; } = 4;
        //最大请求线程数量，大量下载都放到DownloadManager了，这边只有少量下载，以及不算多的其它请求。
        public int maxRequestTask { get; set; } = 4;


        public class IOTask
        {
            public const int RESULT_SUCCESS = 0;
            public const int RESULT_FAIL = -1;

            public Task task;
            public Action<int> callback;
        }


        void Awake()
        {

            // 最大请求数=硬件核数
            maxRequestTask = System.Environment.ProcessorCount;
            maxIOTask = maxRequestTask;
        }

        void Update()
        {
            //IO队列
            if (m_ioTaskList.Count > 0)
            {
                int runningIOCount = 0;
                Task canRunTask = null;

                //倒序删除
                for (int i = m_ioTaskList.Count - 1; i >= 0; i--)
                {
                    IOTask ioTask = m_ioTaskList[i];
                    if (ioTask.task.IsCompleted)
                    {
                        m_ioTaskList.RemoveAt(i);
                        if (ioTask.task.Status == TaskStatus.RanToCompletion)
                        {
                            ioTask.callback(IOTask.RESULT_SUCCESS);
                        }
                        else
                        {
                            ioTask.callback(IOTask.RESULT_FAIL);
                        }
                        continue;
                    }
                    else
                    {
                        if (ioTask.task.Status == TaskStatus.Created)
                        {
                            canRunTask = ioTask.task;
                        }
                        else
                        {
                            runningIOCount++;
                        }
                    }
                }

                if (canRunTask != null && runningIOCount < maxIOTask)
                {
                    canRunTask.Start();
                }
            }

            //请求队列，应该按顺序插入和删除，倒序不合要求
            int runningRequestCount = 0;

            if (m_mainRequestList.Count > 0)
            {
                HttpRequestTask canRunRequest = null;

                for (int i = m_mainRequestList.Count - 1; i >= 0; i--)
                {
                    var requestTask = m_mainRequestList[i];
                    if (requestTask.isDone)
                    {
                        requestTask.Dispose();

                        m_mainRequestList.RemoveAt(i);
                        continue;
                    }

                    if (requestTask.state == HttpTask.ETaskState.Running)
                    {
                        if (requestTask.ioTask != null && requestTask.ioTask.IsCompleted)
                        {
                            requestTask.OnComplete();
                        }
                        else
                        {
                            runningRequestCount++;
                        }
                    }
                    else
                    {
                        if (canRunRequest == null)
                        {
                            canRunRequest = requestTask;
                        }
                    }
                }

                if (canRunRequest != null && runningRequestCount < maxRequestTask)
                {
                    StartCoroutine( canRunRequest.Start() );
                }
            }


            // 主队列空闲了才开始后台队列的请求
            if (runningRequestCount < maxRequestTask && m_mainRequestList.Count == 0)
            {
                // 主队列一帧只开始一个请求，这边也一帧开始一个请求好了
                if (m_backgroundRequestList.Count > 0)
                {
                    while(m_backgroundRequestList.Count > 0)
                    {
                        var requestTask = m_backgroundRequestList[0];
                        m_backgroundRequestList.RemoveAt(0);

                        if (requestTask.isDone)
                        {
                            requestTask.Dispose();
                        }
                        else
                        {
                            m_mainRequestList.Add(requestTask);
                            break;
                        }
                    }
                }
            }

        }


        private void SaveFile(string fullpath, byte[] data, int offset = 0)
        {
            if (data == null || string.IsNullOrEmpty(fullpath))
            {
                Debug.LogWarningFormat("skip to save {0}, coz the bytes to save is null or the save path is empty", fullpath);
                return;
            }

            string dir = Path.GetDirectoryName(fullpath);

            FileStream fs = null;
            try
            {
                if (!XFolderTools.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                fs = File.OpenWrite(fullpath);
                fs.Write(data, offset, data.Length-offset);
            }
            catch(System.IO.IOException e){
                Debug.LogException(e);
            }
            finally{
                if (fs != null)
                    fs.Close();
            }
        }

        private string GetHttpData(byte[] bytes, bool needUnZip)
        {
            string data = null;
            if (needUnZip)
            {
                int ret = HttpDataUnzip(bytes, out data);
                if (ret != 0)
                {
                    data = "{\"code\":0,\"data\":\"data unzip fail\"}";
                }
            }
            else
            {
                data = Encoding.UTF8.GetString(bytes);
            }
            return data;
        }

        private int HttpDataUnzip(byte[] srcData, out string outstring)
        {
            try
            {
                outstring = Encoding.UTF8.GetString(ZipUtil.Decompress_Deflate(srcData));
                return 0;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                outstring = "data uncompress failed";
                return -1;
            }
        }


        public Task OnRequestTaskComplete(HttpRequestTask requestTask)
        {
            UnityWebRequest request = requestTask.request;
            float startTime = requestTask.startTime;
            bool needUnZip = requestTask.parameters.needUnZip;
            HttpCallback callback = requestTask.parameters.callback;

            bool isDownload = requestTask.parameters.isDownload;  //如果是DownloadFile，会有个标志；其它的就算是Get请求也没有。
            string saveFileFullPath = requestTask.parameters.saveFileFullPath;
            string matchMd5 = requestTask.parameters.md5;


            int respCode = (int)request.responseCode;
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "status", respCode }
            };

            byte[] bytes = request.downloadHandler.data;

            if (!request.isHttpError && !request.isNetworkError)
            {
                ulong size = request.downloadedBytes;
                float speed = size / (Time.realtimeSinceStartup - startTime);
                dict.Add("size", size);
                dict.Add("speed", speed);

                string data = "";

                if (isDownload)  // DownloadFile()的回调
                {
                    // 不保存，
                    if (string.IsNullOrEmpty(saveFileFullPath))
                    {
                        data = GetHttpData(bytes, needUnZip);
                        dict.Add("data", data);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(matchMd5) && XStringTools.ToMD5(bytes) != matchMd5)
                        {
                            data = string.Format("{{\"code\":5,\"message\":\"{0}\"}}", "md5 not match");
                            dict.Add("data", data);
                        }
                        else
                        {
                            Task task = new Task(delegate()
                            {
                                // 这一段猥琐的代码，文件二进制数据跟路径拼一起存数据库了，每次下载完需要检测一遍
                                string str = GetHttpData(bytes, needUnZip);

                                int index = str.IndexOf(SEPARATER);
                                if (index != -1)
                                {
                                    data = str.Substring(0, index);

                                    SaveFile(saveFileFullPath, bytes, index+SEPARATER_LEN);
                                }
                                else
                                {
                                    SaveFile(saveFileFullPath, bytes);
                                }
                            });

                            // 下载文件，需要等到文件写入磁盘才算结束
                            IOTask ioTask = new IOTask();
                            ioTask.task = task;
                            ioTask.callback = (status) =>
                            {
                                if (task.Exception != null)
                                {
                                    string message = "";
                                    foreach (var ex in task.Exception.InnerExceptions)
                                    {
                                        var s = ex.Message.Replace("\\", "\\\\");
                                        message += s.Replace("\"", "\\\"");
                                        message += "\n";
                                    }

                                    data = string.Format("{{\"code\":0,\"message\":\"{0}\"}}", message);
                                    Debug.LogError(data);
                                }

                                dict.Add("data", data);

                                callback(dict);
                            };

                            lock (m_ioTaskList)
                            {
                                m_ioTaskList.Add(ioTask);
                            }

                            return task;
                        }
                        // end else
                    }
                }
                else
                {
                    data = GetHttpData(bytes, needUnZip);
                    dict.Add("data", data);
                }
            }
            else
            {
                string data = null;

                if (data == null && bytes != null)
                {
                    data = GetHttpData(bytes, needUnZip);
                }

                if (data == null && !string.IsNullOrEmpty(request.error))
                {
                    data = request.error;
                }

                if (data == null)
                {
                    data = "request error, respCode:" + respCode.ToString();
                }

                dict.Add("data", data ?? "unknown error");

                if (!string.IsNullOrEmpty(request.error))
                    dict.Add("error", request.error);

                if (request.isHttpError)
                    dict.Add("errorType", "http");
                else if (request.isNetworkError)
                    dict.Add("errorType", "network");
                else
                    dict.Add("errorType", "unknown");
            }

            callback(dict);
            return null;
        }

        //GET
        public void Get(string url, bool needUnZip, int timeout, HttpCallback callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            if (timeout > 0)
            {
                request.timeout = timeout;
            }

            AddRequestTask(request, "", needUnZip, callback);
        }

        //POST
        public void Post(string url, string data, bool needUnZip, int timeout, HttpCallback callback)
        {
            Dictionary<string, string> formFields = new Dictionary<string, string>();

            string[] dataList = data.Split(new char[1] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in dataList)
            {
                // 防止&key=value&的value部分包含'='，第一个'='之后的都算value
                int index = str.IndexOf('=');
                if (index > 0)
                {
                    formFields.Add( str.Substring(0, index).Trim(), str.Substring(index+1, str.Length-(index+1)).Trim() );
                }
                else
                {
                    Debug.LogErrorFormat("Incorrect http param: {0}", str);
                }
            }

            UnityWebRequest request = UnityWebRequest.Post(url, formFields);
            if (timeout > 0)
            {
                request.timeout = timeout;
            }

            AddRequestTask(request, "", needUnZip, callback);
        }

        //PUT
        public void UploadFile(string url, string fileFullPath, bool needUnZip, int timeout, HttpCallback callback)
        {
            if (XFileTools.Exists(fileFullPath))
            {
                byte[] fileBytes = null;
                Task task = new Task(() =>
                {
                    FileStream fileStream = new FileStream(fileFullPath, FileMode.Open);
                    int dataLen = (int)fileStream.Length;
                    if (dataLen > 0)
                    {
                        fileBytes = new byte[dataLen];
                        fileStream.Read(fileBytes, 0, dataLen);
                    }
                    fileStream.Close();
                });

                IOTask ioTask = new IOTask();
                ioTask.task = task;
                ioTask.callback = (status) =>
                {
                    if (status == IOTask.RESULT_SUCCESS)
                    {
                        if (fileBytes == null)
                        {
                            Dictionary<string, object> dict = new Dictionary<string, object>();
                            dict.Add("data", "file data is empty !");
                            dict.Add("status", IOTask.RESULT_FAIL);
                            callback(dict);
                        }
                        else
                        {
                            UnityWebRequest request = UnityWebRequest.Put(url, fileBytes);
                            if (timeout > 0)
                            {
                                request.timeout = timeout;
                            }

                            AddRequestTask(request, "", needUnZip, callback);
                        }
                    }
                    else
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();

                        string message = "";
                        foreach (var ex in task.Exception.InnerExceptions)
                        {
                            message += ex.Message;
                            message += "\n";
                        }

                        string data1 = string.Format("{{\"code\":0,\"message\":\"read error:{0}\"}}", message);
                        dict.Add("data", data1);
                        dict.Add("status", IOTask.RESULT_FAIL);
                        callback(dict);
                    }
                };

                lock (m_ioTaskList)
                {
                    m_ioTaskList.Add(ioTask);
                }
            }
            else
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("data", "file not exists !");
                dict.Add("status", IOTask.RESULT_FAIL);
                callback(dict);
            }
        }


        private HttpRequestTask AddRequestTask(UnityWebRequest request, string saveFileFullPath, bool needUnZip, HttpCallback callback, 
                                                bool topPriority = false, string matchMd5 = null)
        {
            HttpRequestParameters param = new HttpRequestParameters();
            param.saveFileFullPath = saveFileFullPath;
            param.needUnZip = needUnZip;
            param.callback = callback;
            param.md5 = matchMd5;

            HttpRequestTask requestTask = CreateRequestTask(request, param);

            if (topPriority)
            {
                m_mainRequestList.Insert(0, requestTask);
            }
            else
            {
                m_mainRequestList.Add(requestTask);
            }
        
            return requestTask;
        }

        private HttpRequestTask CreateRequestTask(UnityWebRequest request, HttpRequestParameters parameters)
        {
            HttpRequestTask requestTask = new HttpRequestTask();
            requestTask.parameters = parameters;
            requestTask.request = request;
            return requestTask;
        }


        #region Download
        //GET
        public HttpRequestTask DownloadFile(string url, string saveFileFullPath, bool needUnZip, int timeout, HttpCallback callback, bool topPriority = false, string matchMd5 = null)
        {
            return AddDownloadRequestTask(false, url, saveFileFullPath, needUnZip, timeout, callback, topPriority, matchMd5);
        }

        //GET
        public HttpRequestTask DownloadFileInBackground(string url, string saveFileFullPath, bool needUnZip, int timeout, HttpCallback callback, bool topPriority = false, string matchMd5 = null)
        {
            return AddDownloadRequestTask(true, url, saveFileFullPath, needUnZip, timeout, callback, topPriority, matchMd5);
        }

        private HttpRequestTask AddDownloadRequestTask(bool inBackground, string url, string saveFileFullPath, bool needUnZip, int timeout, HttpCallback callback, bool topPriority = false, string matchMd5 = null)
        {
            HttpRequestTask requestTask = null;

            // 已经在主队列中的，直接返回
            int requestTaskInMainIndex = m_mainRequestList.FindIndex((task)=>task.parameters.url == url);
            if (requestTaskInMainIndex >= 0)
            {
                requestTask = m_mainRequestList[requestTaskInMainIndex];

                requestTask.parameters.callback -= callback;
                requestTask.parameters.callback += callback;

                if (inBackground)
                {
                    Debug.LogFormat("主队列中的请求想移动到后台队列？");
                }
                else
                {
                    // 移到最前面
                    if (topPriority)
                    {
                        // index前面的往后移一位，必须倒序移动
                        for(int i = requestTaskInMainIndex-1; i >= 0; i--)
                        {
                            m_mainRequestList[i+1] = m_mainRequestList[i];
                        }

                        // index自己插入到0
                        m_mainRequestList[0] = requestTask;
                        Debug.LogFormat("request in main queue move from {0} to 0", requestTaskInMainIndex);
                    }
                }
                return requestTask;
            }
            else
            {
                // 在后台队列的，判断一下是否需要移到主队列中去.
                int requestTaskInBackgroundIndex = m_backgroundRequestList.FindIndex((task)=>task.parameters.url == url);
                if (requestTaskInBackgroundIndex >= 0)
                {
                    requestTask = m_backgroundRequestList[requestTaskInBackgroundIndex];

                    // 移到主队列中去
                    if (!inBackground)
                    {
                        m_backgroundRequestList.RemoveAt(requestTaskInBackgroundIndex);
                        Debug.LogFormat("request in background queue is removed {0}", requestTaskInBackgroundIndex);
                    }
                }
            }

            if (requestTask == null)
            {
                HttpRequestParameters param = new HttpRequestParameters();
                param.url = url;
                param.saveFileFullPath = saveFileFullPath;
                param.needUnZip = needUnZip;
                param.callback -= callback;
                param.callback += callback;
                param.md5 = matchMd5;
                param.timeout = timeout;
                param.isDownload = true;

                requestTask = CreateRequestTask(null, param);

                if (string.IsNullOrEmpty(saveFileFullPath))
                {
                    Debug.LogWarningFormat("不保存为什么还要用DownloadFile()??? 用Get()不好吗？（url=%s）", url);
                }
            }

            List<HttpRequestTask> queueToAdd = null;
            if (inBackground)
            {
                queueToAdd = m_backgroundRequestList;
            }
            else
            {
                queueToAdd = m_mainRequestList;
            }

            if (topPriority)
            {
                queueToAdd.Insert(0, requestTask);
            }
            else
            {
                queueToAdd.Add(requestTask);
            }
        
            return requestTask;
        }

        /// <summary>
        /// 已经失败的请求，重置之后又重新请求.
        /// </summary>
        public bool ReAddDownloadRequestTask(HttpRequestTask requestTask, bool topPriority = false)
        {
            Debug.LogFormat(">> 尝试将requestTask重新加入下载队列...");
            if (requestTask == null)
                return false;

            if ( m_mainRequestList.Find( (task)=>task.parameters.url == requestTask.parameters.url ) != null )
            {
                return false;
            }

            if (topPriority)
            {
                m_mainRequestList.Insert(0, requestTask);
            }
            else
            {
                m_mainRequestList.Add(requestTask);
            }
            return true;
        }
        #endregion


        public void CancelDownload(HttpRequestTask task)
        {
            if (task == null || task.isDone)
            {
                return;
            }
            task.Cancel();
        }

        public void ClearAllTask()
        {
            foreach (var requestTask in m_mainRequestList)
            {
                requestTask.Dispose();
            }
            m_mainRequestList.Clear();

            foreach (var requestTask in m_backgroundRequestList)
            {
                requestTask.Dispose();
            }
            m_backgroundRequestList.Clear();

            foreach(var task in m_ioTaskList)
            {
                task.callback = null;
            }
            m_ioTaskList.Clear();
        }

    }
}
