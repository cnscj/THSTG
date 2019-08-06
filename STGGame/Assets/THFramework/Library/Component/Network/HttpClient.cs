using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Net;
using System.IO;
using System;
using System.Threading.Tasks;


namespace THGame
{
    public class DownloadHandle
    {
        private UnityWebRequest m_request;
        private bool m_isClosed = false;

        public UnityWebRequest Request { get { return m_request; } }
        public bool IsClosed { get { return m_isClosed; } }

        public DownloadHandle(UnityWebRequest request)
        {
            m_request = request;
        }

        public float progress
        {
            get
            {
                if (m_isClosed)
                {
                    return 1f;
                }
                if (m_request.isNetworkError || m_request.isHttpError)
                {
                    return 0f;
                }
                var n = m_request.downloadProgress;
                if (n < 0f)
                {
                    return 0f;
                }
                return n;
            }
        }

        public int progressPercent
        {
            get
            {
                return Convert.ToInt32(this.progress * 100f);
            }
        }

        public void Close(UnityWebRequest request)
        {
            if (request == m_request)
            {
                m_isClosed = true;
                HttpClient.Instance.s_disposeWebRequestCast -= Close;
            }
        }
    }

    public delegate void HttpCallback(Dictionary<string, object> dict);


    public class HttpClient : MonoBehaviour
    {
        private static HttpClient s_instance = null;
        public static HttpClient Instance
        {
            get
            {
                return s_instance;
            }
        }

        private static readonly string SEPARATER = "|{&&}|";

        private static readonly int SEPARATER_LEN = SEPARATER.Length;

        List<IOTask> m_ioTaskList = new List<IOTask>();
        List<RequestTask> m_httpRequestList = new List<RequestTask>();

        public Action<UnityWebRequest> s_disposeWebRequestCast;

        //最大io线程数量
        public int maxIOTask { get; set; } = 5;
        //最大下载线程数量
        public int maxRequestTask { get; set; } = 5;

        public struct IOTask
        {
            public Task task;
            public Action<int> callback;
        }

        public class RequestTask
        {
            public enum EStatus
            {
                Created = 0,
                Running,
                Completed
            }
            public Action<RequestTask> runFunc;
            public EStatus status;
            public Task ioTask;//  下载时有io任务
            public UnityWebRequest request;
        }


        void Awake()
        {
            s_instance = this;
        }

        void Update()
        {
            if (m_ioTaskList.Count > 0)
            {
                int notStart = 0;
                Task canRunTask = null;
                //倒序删除
                for (int i = m_ioTaskList.Count - 1; i >= 0; i--)
                {
                    IOTask ioTask = m_ioTaskList[i];
                    if (ioTask.task.IsCompleted)
                    {
                        m_ioTaskList.Remove(ioTask);
                        if (ioTask.task.Status == TaskStatus.RanToCompletion)
                        {
                            ioTask.callback(0);
                        }
                        else
                        {
                            ioTask.callback(-1);
                        }
                        continue;
                    }
                    if (ioTask.task.Status == TaskStatus.Created)
                    {
                        canRunTask = ioTask.task;
                    }
                    else
                    {
                        notStart++;
                    }

                }

                if (canRunTask != null && notStart < maxIOTask)
                {
                    canRunTask.Start();
                }
            }

            //请求管理
            if (m_httpRequestList.Count > 0)
            {
                RequestTask canRunRequest = null;
                int runningCount = 0;
                for (int i = m_httpRequestList.Count - 1; i >= 0; i--)
                {
                    var requestTask = m_httpRequestList[i];
                    if (requestTask.status == RequestTask.EStatus.Completed)
                    {
                        m_httpRequestList.Remove(requestTask);
                        continue;
                    }
                    if (requestTask.status == RequestTask.EStatus.Running)
                    {
                        if (requestTask.ioTask != null && requestTask.ioTask.IsCompleted)
                        {
                            requestTask.status = RequestTask.EStatus.Completed;
                        }
                        else
                        {
                            runningCount++;
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

                if (canRunRequest != null && runningCount < maxRequestTask)
                {
                    canRunRequest.runFunc(canRunRequest);
                }
            }
        }


        private void SaveFile(string fullpath, byte[] data)
        {
            if (data == null)
            {
                return;
            }
            var dir = Path.GetDirectoryName(fullpath);
            if (!XFolderTools.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllBytes(fullpath, data);
        }


        public int HttpDataUnzip(byte[] srcData, out string outstring)
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

        public Task OnRequestComplete(UnityWebRequest request, float startTime, bool needUnZip, string saveFileFullPath, HttpCallback callback, string matchMd5 = null)
        {
            int respCode = (int)request.responseCode;
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "status", respCode }
            };

            if (!request.isHttpError && !request.isNetworkError)
            {
                float endTime = Time.realtimeSinceStartup;
                ulong size = request.downloadedBytes;
                float speed = size / (endTime - startTime);
                string data = "";

                if (needUnZip)
                {
                    int ret = HttpDataUnzip(request.downloadHandler.data, out data);
                    if (ret != 0)
                    {
                        data = "{\"code\":0,\"data\":\"data unzip fail\"}";
                    }
                }
                else
                {
                    if (saveFileFullPath.Length > 0)
                    {
                        var bytes = request.downloadHandler.data;
                        if (matchMd5 != null && XStringTools.ToMD5(bytes) != matchMd5)
                        {
                            data = string.Format("{{\"code\":5,\"message\":\"{0}\"}}", "md5 not match");
                        }
                        else
                        {
                            Task task = new Task((phpData) =>
                           {
                               string str = Encoding.UTF8.GetString(bytes);
                               int index = str.IndexOf(SEPARATER);
                               if (index != -1)
                               {
                                   data = str.Substring(0, index);
                                   int saveDataLen = bytes.Length - index - SEPARATER_LEN;
                                   byte[] saveData = new byte[saveDataLen];
                                   Array.Copy(bytes, index + SEPARATER_LEN, saveData, 0, saveDataLen);
                                   SaveFile(saveFileFullPath, saveData);
                               }
                               else
                               {
                                   SaveFile(saveFileFullPath, bytes);
                               }
                           }, request.downloadHandler.data);

                            lock (m_ioTaskList)
                            {
                                m_ioTaskList.Add(new IOTask
                                {
                                    task = task,
                                    callback = (status) =>
                                    {
                                        if (status == 0)
                                        {
                                            dict.Add("size", size);
                                            dict.Add("speed", speed);
                                            dict.Add("data", data);
                                        }
                                        else
                                        {
                                            string message = "";
                                            AggregateException excep = task.Exception;
                                            foreach (var ex in excep.InnerExceptions)
                                            {
                                                var s = ex.Message;
                                                s = s.Replace("\\", "\\\\");
                                                message += s.Replace("\"", "\\\"");
                                                message += "\n";
                                            }
                                            data = string.Format("{{\"code\":0,\"message\":\"{0}\"}}", message);
                                            Debug.Log(data);
                                            dict.Add("data", data);
                                        }
                                        DisposeWebRequest(request);
                                        callback(dict);
                                    }
                                });
                            }
                            return task;
                        }
                    }
                    else
                    {
                        data = request.downloadHandler.text;
                    }
                }
                dict.Add("size", size);
                dict.Add("speed", speed);
                dict.Add("data", data);
            }
            else
            {
                if (request.error != null)
                {
                    dict.Add("data", request.error);
                }
                else
                {
                    dict.Add("data", "request error, respCode:" + respCode);
                }
            }
            callback(dict);
            DisposeWebRequest(request);
            return null;
        }

        public void OnRequestAssetBundleComplete(UnityWebRequest request, float startTime, string saveFileFullPath, HttpCallback callback, string matchMd5 = null)
        {
            int respCode = (int)request.responseCode;
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "status", respCode }
            };

            if (!request.isHttpError && !request.isNetworkError)
            {
                float endTime = Time.realtimeSinceStartup;
                ulong size = request.downloadedBytes;
                float speed = size / (endTime - startTime);
                string data = "";
                dict.Add("size", size);
                dict.Add("speed", speed);

                byte[] bytes = request.downloadHandler.data;
                byte[] assetBundleBytes;

                string str = Encoding.UTF8.GetString(bytes);
                int index = str.IndexOf(SEPARATER);
                if (index != -1)
                {
                    data = str.Substring(0, index);
                    int saveDataLen = bytes.Length - index - SEPARATER_LEN;
                    assetBundleBytes = new byte[saveDataLen];
                    Array.Copy(bytes, index + SEPARATER_LEN, assetBundleBytes, 0, saveDataLen);
                }
                else
                {
                    assetBundleBytes = bytes;
                }
                if (matchMd5 != null && XStringTools.ToMD5(assetBundleBytes) != matchMd5)
                {
                    data = string.Format("{{\"code\":5,\"message\":\"{0}\"}}", "md5 not match");
                    dict.Add("data", data);
                }
                else
                {
                    AssetBundle assetBundle = AssetBundle.LoadFromMemory(assetBundleBytes);
                    dict.Add("assetBundle", assetBundle);
                    dict.Add("data", data);

                    if (saveFileFullPath.Length > 0)
                    {
                        Task task = new Task((phpData) =>
                       {
                           byte[] abdata = (byte[])phpData;
                           SaveFile(saveFileFullPath, abdata);
                       }, assetBundleBytes);

                        lock (m_ioTaskList)
                        {
                            m_ioTaskList.Add(new IOTask
                            {
                                task = task,
                                callback = (status) =>
                                {
                                    if (status != 0)
                                    {
                                        string message = "";
                                        AggregateException excep = task.Exception;
                                        foreach (var ex in excep.InnerExceptions)
                                        {
                                            message += ex.Message;
                                            message += "\n";
                                        }
                                        Debug.Log(message);
                                    }
                                }
                            });
                        }
                    }
                }
            }
            else
            {
                if (request.error != null)
                {
                    dict.Add("data", request.error);
                }
                else
                {
                    dict.Add("data", "request error, respCode:" + respCode);
                }
            }
            callback(dict);
            DisposeWebRequest(request);
        }

        public void Get(string url, bool needUnZip, int timeout, HttpCallback callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            if (timeout > 0)
            {
                request.timeout = timeout;
            }
            AddRequestTask(request, url, "", needUnZip, callback);
        }

        public void Post(string url, string data, bool needUnZip, int timeout, HttpCallback callback)
        {
            Dictionary<string, string> formFields = new Dictionary<string, string>();
            string[] dataList = data.Split(new char[1] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in dataList)
            {
                string[] strList = str.Split(new char[1] { '=' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (strList.Length >= 2)
                {
                    formFields.Add(strList[0], strList[1]);
                }
            }
            //UnityWebRequest request = UnityWebRequest.Post(url, data);
            UnityWebRequest request = UnityWebRequest.Post(url, formFields);
            if (timeout > 0)
            {
                request.timeout = timeout;
            }
            AddRequestTask(request, url, "", needUnZip, callback);
        }

        public void UploadFile(string url, string fileFullPath, bool needUnZip, int timeout, HttpCallback callback)
        {
            byte[] data = null;
            if (XFileTools.Exists(fileFullPath))
            {
                Task task = new Task(() =>
               {
                   FileStream fileStream = new FileStream(fileFullPath, FileMode.Open);
                   int dataLen = (int)fileStream.Length;
                   if (dataLen > 0)
                   {
                       data = new byte[dataLen];
                       fileStream.Read(data, 0, dataLen);
                   }
                   fileStream.Close();
               });
                lock (m_ioTaskList)
                {
                    m_ioTaskList.Add(new IOTask
                    {
                        task = task,
                        callback = (status) =>
                        {
                            if (status == 0)
                            {
                                if (data == null)
                                {
                                    Dictionary<string, object> dict = new Dictionary<string, object>();
                                    dict.Add("data", "file not empty !");
                                    dict.Add("status", -1);
                                    callback(dict);
                                }
                                else
                                {
                                    UnityWebRequest request = UnityWebRequest.Put(url, data);
                                    if (timeout > 0)
                                    {
                                        request.timeout = timeout;
                                    }
                                    AddRequestTask(request, url, "", needUnZip, callback);
                                }
                            }
                            else
                            {
                                Dictionary<string, object> dict = new Dictionary<string, object>();
                                string message = "";
                                AggregateException excep = task.Exception;
                                foreach (var ex in excep.InnerExceptions)
                                {
                                    message += ex.Message;
                                    message += "\n";
                                }
                                string data1 = string.Format("{{\"code\":0,\"message\":\"read error:{0}\"}}", message);
                                dict.Add("data", data1);
                                dict.Add("status", -1);
                                callback(dict);
                            }
                        }
                    });
                }
            }
            else
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                dict.Add("data", "file not exists !");
                dict.Add("status", -1);
                callback(dict);
            }
        }

        public DownloadHandle DownloadFile(string url, string fileFullPath, bool needUnZip, int timeout, HttpCallback callback, bool first = false, string matchMd5 = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            if (timeout > 0)
            {
                request.timeout = timeout;
            }
            AddRequestTask(request, url, fileFullPath, needUnZip, callback, first, false, matchMd5);

            var handle = new DownloadHandle(request);
            s_disposeWebRequestCast += handle.Close;

            return handle;
        }

        public DownloadHandle DownloadAssetBundle(string url, string fileFullPath, int timeout, HttpCallback callback, bool first = true, string matchMd5 = null)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            if (timeout > 0)
            {
                request.timeout = timeout;
            }
            AddRequestTask(request, url, fileFullPath, false, callback, first, true, matchMd5);

            var handle = new DownloadHandle(request);
            s_disposeWebRequestCast += handle.Close;

            return handle;
        }

        private void AddRequestTask(UnityWebRequest request, string url, string fileFullPath, bool needUnZip, HttpCallback callback, bool first = false, bool isAssetBundle = false, string matchMd5 = null)
        {
            var requestTask = new RequestTask
            {
                request = request,
                runFunc = (self) =>
                {
                    self.status = RequestTask.EStatus.Running;
                    float startTime = Time.realtimeSinceStartup;

                    UnityWebRequestAsyncOperation asyncOper = request.SendWebRequest();
                    asyncOper.completed += (result) =>
                    {
                        try
                        {
                            if (isAssetBundle)
                            {
                                OnRequestAssetBundleComplete(self.request, startTime, fileFullPath, callback, matchMd5);
                                self.status = RequestTask.EStatus.Completed;
                            }
                            else
                            {
                                Task task = OnRequestComplete(self.request, startTime, needUnZip, fileFullPath, callback, matchMd5);
                                if (task == null)
                                {
                                    self.status = RequestTask.EStatus.Completed;
                                }
                                else
                                {
                                    self.ioTask = task;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e);
                            self.status = RequestTask.EStatus.Completed;
                        }
                    };
                },
                status = RequestTask.EStatus.Created
            };
            if (first)
            {
                m_httpRequestList.Insert(0, requestTask);
            }
            else
            {
                m_httpRequestList.Add(requestTask);
            }
        }

        public void DisposeWebRequest(UnityWebRequest request)
        {
            if (request == null) return;

            //可能request是Get, Post之后传过来的，不是Download，此时s_disposeWebRequestCast还没有。
            if (s_disposeWebRequestCast != null)
                s_disposeWebRequestCast.Invoke(request);

            request.Dispose();
        }

        public void CancelDownload(DownloadHandle downloadHandle)
        {
            if (downloadHandle == null || downloadHandle.IsClosed) return;

            downloadHandle.Request.Abort();
        }

        public void ClearAllTask()
        {
            foreach (var requestTask in m_httpRequestList)
            {
                if (requestTask.status == RequestTask.EStatus.Running)
                {
                    if (!requestTask.request.isDone)
                    {
                        requestTask.request.Abort();
                    }
                    else
                    {
                        DisposeWebRequest(requestTask.request);
                    }
                }
                else
                {
                    DisposeWebRequest(requestTask.request);
                }
            }
            m_httpRequestList.Clear();
            m_ioTaskList.Clear();
        }

        public static string GetIpByDomainName(string domainName)
        {
            IPAddress[] ips = Dns.GetHostAddresses(domainName);
            if (ips.Length > 0)
            {
                return ips[0].ToString();
            }
            return "0.0.0.0";
        }
    }
}
