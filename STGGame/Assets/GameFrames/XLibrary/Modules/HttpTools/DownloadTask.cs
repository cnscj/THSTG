using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Net;
using XLibrary;

namespace XLibGame
{
    public class DownloadTaskParameters
    {
        public string cdnDomain;
        public string relativePath;  //相对路径，会跟cdnDomain拼接
        public string md5;
        public string saveFileFullPath;
        public int timeout = 10;
        public bool isAssetBundle;
        public HttpCallback callback;

        public void ClearCallback()
        {
            callback = null;
        }
    }

    /// <summary>
    /// 多线程+断点续传 http下载器, 注意用完后要Dispose
    /// </summary>
    public class DownloadTask : HttpTask
    {
        public DownloadTaskParameters parameters;

        private bool _useContinue; // 是否使用断点续传

        /// <summary>
        /// 下载中临时文件的后缀
        /// </summary>
        public const string DOWNLOADING_TEMP_FILE_EXTENSION = ".downloading";
        public const string SAVE_FILE_PATH_KEY = "saveFilePath";

        /// <summary>
        /// 每次读取网络流的字节数
        /// </summary>
        public const int READ_STREAM_CHUNK_SIZE = 10240;

        // 多线程里面要用到
        private string SaveFileFullPath = "";
        private string TmpDownloadPath = "";

        public int TotalSize {get; private set;} = int.MaxValue;

        /// <summary>
        /// 下载限速，单位：字节/秒
        /// 具体换算由外部逻辑来做
        /// </summary>
        public double downloadSpeedLimit = 0d;

        // 这边实现是用的
        protected FileStream m_downloadFileStream = null;
        protected HttpWebRequest m_httpWebRequest = null;

        // 测试用的，查看下载耗时
        protected DateTime startTime;


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="param">网络请求相关的参数</param>
        /// <param name="useContinue">是否断点续传</param>
        public void Init(DownloadTaskParameters param, bool useContinue = true)
        {
            this.parameters = param;

            SaveFileFullPath = param.saveFileFullPath;
            TmpDownloadPath = SaveFileFullPath + DOWNLOADING_TEMP_FILE_EXTENSION;

            this._useContinue = useContinue;

            Reset();
        }

        void Reset()
        {
            progress = 0f;
            TotalSize = int.MaxValue;
            state = ETaskState.Created;
            error = null;

            m_downloadFileStream = null;
            m_httpWebRequest = null;
        }

        public override IEnumerator Start()
        {
            state = ETaskState.Running;
            startTime = DateTime.Now;

            // 拼接一下，cdnDomain可能会被修改
            string fullUrl;
            if (string.IsNullOrEmpty(parameters.cdnDomain))
            {
                fullUrl = parameters.relativePath;
            }
            else
            {
                fullUrl = parameters.cdnDomain + parameters.relativePath;
            }

            // 创建目录
            string dir = Path.GetDirectoryName(SaveFileFullPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // 同步线程中的数据
            int totalSize = int.MaxValue;
            int downloadSize = 0;

            bool isThreadStart = false;
            bool isThreadFinish = false;
            bool isThreadError = false;
            string threadError = null;

            // 开始线程
            ThreadPool.QueueUserWorkItem((_) =>
            {
                DownloadInThread(fullUrl, (totalSizeNow, downSizeNow) =>
                {
                    totalSize = totalSizeNow;
                    downloadSize = downSizeNow;
                    isThreadStart = true;
                },
                (errorMsg) =>
                {
                    isThreadError = true;
                    isThreadFinish = true;
                    isThreadStart = true;
                    threadError = errorMsg;
                },
                () => 
                { 
                    isThreadFinish = true; 
                });
            });

            // 检查开始超时，在主线程
            float timeCounter = 0f;
            float MaxTime = (float)parameters.timeout;

            while (!isThreadFinish && !isThreadError)
            {
                timeCounter += Time.deltaTime;
                if (timeCounter > MaxTime && !isThreadStart)
                {
                    Debug.LogError(string.Format("下载线程超时: {0}", fullUrl));

                    threadError = "download thread time out";
                    break;
                }

                progress = downloadSize/(float)totalSize;
                yield return null;
            }

            // 处理错误
            error = threadError;

            if (isThreadError)
            {
                Debug.LogError(string.Format("Thread Error op: {0} when downloading {1}", error, fullUrl));

                // TODO: 删除临时文件
                // DeleteTempFile();

                OnFinish();
                yield break;
            }

            progress = 1f;
            OnFinish();
        }

        /// <summary>
        /// 在另一个线程中下载
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stepCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finishCallback"></param>
        void DownloadInThread(string url, Action<int, int> stepCallback, Action<string> errorCallback, Action finishCallback)
        {
            //打开上次下载的文件或新建文件 
            long lStartPos = 0L;

            if (_useContinue && File.Exists(TmpDownloadPath))
            {
                m_downloadFileStream = File.OpenWrite(TmpDownloadPath);
                lStartPos = m_downloadFileStream.Length;
                m_downloadFileStream.Seek(lStartPos, SeekOrigin.Current); //移动文件流中的当前指针 

                /*Console.WriteLine*/ 
                Debug.LogFormat("[Thread] Resume.... from {0}", lStartPos);
            }
            else
            {
                m_downloadFileStream = new FileStream(TmpDownloadPath, FileMode.OpenOrCreate);
                lStartPos = 0L;
            }

            // 打开网络连接 
            try
            {
                m_httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                if (lStartPos > 0L)
                    m_httpWebRequest.AddRange((int)lStartPos); //设置Range值

                // TODO: IfRange可以传 Etag或者Last-Modified返回的值，如果上次没下载完成，那么可能保存在临时文件中
                // m_httpWebRequest.Headers.Add(HttpRequestHeader.IfRange, "");

                /*Console.WriteLine*/
                Debug.LogFormat("[Thread] Getting Response: {0}", url);
                
                //向服务器请求，获得服务器回应数据流 
                using (var response = m_httpWebRequest.GetResponse()) // TODO: Async Timeout
                {
                    TotalSize = (int)response.ContentLength;

                    // TODO: 保存到本地，如果文件没下载完的话.
                    // string ETag = response.Headers[HttpResponseHeader.ETag];// response.Headers.Get("ETag");
                    // string lastModified = response.Headers[HttpResponseHeader.LastModified];//response.Headers.Get("Last-Modified");

                    /*Console.WriteLine*/
                    Debug.LogFormat("[Thread] Get Response : {0}", url);
                    if (isDone)
                    {
                        throw new Exception(string.Format("Get Response ok, but it is finished, maybe timeout: {0}", url));
                    }
                    else
                    {
                        int totalSize = TotalSize;

                        using (var stream = response.GetResponseStream())
                        {
                            /*Console.WriteLine*/
                            Debug.LogFormat("[Thread] Start Stream: {0}", url);

                            int downloadedSize = (int)lStartPos;
                            int chunkSize = READ_STREAM_CHUNK_SIZE;  //字节
                            byte[] nbytes = new byte[chunkSize];
                            int nReadSize = (int)lStartPos;

                            DateTime startTime = DateTime.Now;
                            double sleepTime = 0d;

                            double packetTime = 0d;  //ms，在指定速度下，每一次请求最多消耗的时间
                            if (downloadSpeedLimit > 0d)
                                packetTime = chunkSize * 1000d / downloadSpeedLimit;

                            do
                            {
                                if (isDone)
                                    throw new Exception("When reading response stream, the downloder finished!");

                                // 限速
                                if (downloadSpeedLimit > 0d)
                                {
                                    startTime = DateTime.Now;
                                }
 
                                nReadSize = stream.Read(nbytes, 0, chunkSize);
                                if (nReadSize <= 0)
                                    break;

                                m_downloadFileStream.Write(nbytes, 0, nReadSize);

                                downloadedSize += nReadSize;
                                stepCallback(totalSize, downloadedSize);

                                // 限速：主要是限制读的速度
                                if (downloadSpeedLimit > 0d)
                                {
                                    // 实际消耗的时间小于指定速度的时间，剩下的时间就要等待。C#的接口还有线程的等待时间只能精确到毫秒。
                                    sleepTime = Math.Round(packetTime - (DateTime.Now - startTime).TotalMilliseconds);
                                    if( sleepTime > 0d && sleepTime < 0x7FFFFFFF )
                                    {
                                        Thread.Sleep((int)sleepTime);  // 休息一下，怎么换算需要再看
                                    }
                                }
                            }
                            while ( nReadSize > 0);

                            stepCallback(totalSize, totalSize);

                            m_httpWebRequest.Abort();
                            m_downloadFileStream.Close();
                        }
                    }
                }

                // 把临时文件存成正式文件
                /*Console.WriteLine*/
                Debug.LogFormat("[Thread] 下载完成: {0}", url);

                MoveTempFile();
            }
            catch (WebException ex)
            {
                /*Console.WriteLine*/
                Debug.LogFormat("[Thread] 下载过程中出现错误: WebExceptionStatus={0}, Exception:{1}", ex.Status.ToString(), ex.ToString());

                if (m_httpWebRequest != null)
                {
                    m_httpWebRequest.Abort();
                }
                m_downloadFileStream.Close();

                // 出任何错误都会把临时文件删除，这样断点续传就没用了.
                // DeleteTempFile();

                var hwr = ex.Response as HttpWebResponse;
                if (hwr != null)
                    errorCallback(ex.Message + string.Format(", HttpStatusCode = {0}", hwr.StatusCode));
                else
                    errorCallback(ex.Message);
            }
            finally
            {
                m_httpWebRequest = null;
                m_downloadFileStream = null;
            }

            finishCallback();
        }

        /// <summary>
        /// 这边处理 DownloadTaskParameters 的回调
        /// </summary>
        void OnFinish()
        {
            state = ETaskState.Completed;

            double costTime = (DateTime.Now-startTime).TotalMilliseconds;
            if (costTime > 5000d)
            {
                Debug.LogWarningFormat("[Task] {0} finished, and totally costs {1}ms", parameters.relativePath, costTime.ToString());
            }
            else
            {
                 //Debug.LogFormat("[Task] {0} finished, and totally costs {1}ms", parameters.relativePath, costTime.ToString());
            }

            // 准备回调
            bool isAssetBundle = parameters.isAssetBundle;
            string matchMd5 = parameters.md5;

            // result
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(error))  //succeed
            {
                dict.Add("status", 200);  //成功都用200；但是没有"saveFilePath"这个本地保存路径的话还是属于失败.
                dict.Add("size", TotalSize);
                string data = "";

                byte[] bytes = null;
                if ( !string.IsNullOrEmpty(matchMd5) )
                {
                    bytes = File.ReadAllBytes(SaveFileFullPath);
                }

                // 比对md5
                if (!string.IsNullOrEmpty(matchMd5) && XStringTools.ToMD5(bytes) != matchMd5)
                {
                    data = string.Format("{{\"code\":5,\"message\":\"{0}\"}}", "md5 not match");
                    dict.Add("data", data);
                }
                else
                {
                    dict.Add("data", data);

                    // NOTE:
                    // 只回调一个文件保存路径，因为AssetBundle.LoadFromMemory()实在太慢了，同步调用太容易卡顿，还是给逻辑部分异步加载吧。
                    dict.Add(SAVE_FILE_PATH_KEY, SaveFileFullPath);
                }
                bytes = null;
            }
            else
            {
                dict.Add("status", 500);  //失败都用500
                dict.Add("data", error);
            }

            // 回调
            HttpCallback callback = parameters.callback;
            if (callback == null)
            {
                Debug.LogWarningFormat("回调为空，跳过, status: {0}, data: {1}", dict["status"], dict["data"]);
            }
            else
            {
                callback(dict);
                //Debug.Log("回调！");
            }
        }

        void MoveTempFile()
        {
            try
            {
                if (File.Exists(SaveFileFullPath))
                {
                    File.Delete(SaveFileFullPath);
                }
                File.Move(TmpDownloadPath, SaveFileFullPath);
            }
            catch (Exception e)
            {
                /*Console.WriteLine*/
                Debug.LogFormat(e.Message);
            }
        }

        void DeleteTempFile()
        {
            try
            {
                if (File.Exists(TmpDownloadPath))
                {
                    File.Delete(TmpDownloadPath); // delete temporary file
                }
            }
            catch (Exception e)
            {
                /*Console.WriteLine*/
                Debug.LogFormat(e.Message);
            }
        }

        // 强制中断
        public override void Cancel()
        {
            // 设置这2个变量，让isDone改成true，如果线程还在运行状态，线程内部会抛异常.
            // isDone也算关掉线程的一个信号
            if (state <= ETaskState.Running)
            {
                error = "aborted by user";
                state = ETaskState.Canceled;
            }
        }

        protected override void OnDispose()
        {
            Cancel();

            ClearCallback();
        }

        // 清除回调
        public override void ClearCallback()
        {
            if (parameters != null)
                parameters.ClearCallback();
        }

        public void ChangeUrlAndRestart(string relativePath, string cdnDomain)
        {
            if (state == ETaskState.Running)
            {
                Debug.LogWarning("You can't change url when the task is running!");
                return;
            }

            parameters.relativePath = relativePath;
            parameters.cdnDomain = cdnDomain;

            DeleteTempFile();

            Reset();
        }

    }

}
