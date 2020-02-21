using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace XLibGame
{
    public class HttpRequestParameters
    {
        public string cdnDomain;

        public string url;
        public string md5;
        public string saveFileFullPath;
        public int timeout;
        public bool needUnZip;
        public HttpCallback callback;

        public bool isDownload;

        public void Clear()
        {
            callback = null;
        }
    }

    public class HttpRequestTask : HttpTask
    {
        public HttpRequestParameters parameters;

        public UnityWebRequest request;
        public Task ioTask;//  下载时有io任务

        public float startTime;


        public void ChangeUrlAndRestart(string relativePath, string cdnDomain)
        {
            if (state == ETaskState.Running)
            {
                Debug.LogWarning("You can't change url when the task is running!");
                return;
            }

            parameters.url = relativePath;
            parameters.cdnDomain = cdnDomain;

            Reset();
        }

        void Reset()
        {
            progress = 0f;
            state = ETaskState.Created;
            error = null;

            ioTask = null;
            if (request != null)
            {
                request.Abort();

                request.Dispose();
                request = null;
            }
        }

        // 重新激活
        public override void Revive()
        {

        }

        public override IEnumerator Start()
        {
            state = ETaskState.Running;

            if (this.request == null)
            {
                // 把下载的请求放最后再初始化，因为cdn可能在等待的阶段被修改，下载失败也可能会修改，然后重新下载。
                if (parameters.isDownload)
                {
                    string url;
                    if (string.IsNullOrEmpty(parameters.cdnDomain))
                    {
                        url = parameters.url;
                    }
                    else
                    {
                        url = parameters.cdnDomain + parameters.url;
                    }

                    this.request = UnityWebRequest.Get(url);
                    if (parameters.timeout > 0)
                    {
                        this.request.timeout = parameters.timeout;
                    }
                }
                else
                {
                    Debug.LogErrorFormat("不是下载请求也没有初始化request?");
                }
            }

            this.startTime = Time.realtimeSinceStartup;

            UnityWebRequestAsyncOperation asyncOper = this.request.SendWebRequest();
            while (!asyncOper.isDone)
            {
                if (request.method == UnityWebRequest.kHttpVerbPUT)
                    progress = Mathf.Max(0f, request.uploadProgress);
                else
                    progress = Mathf.Max(0f, request.downloadProgress);

                if (hasError)
                    yield break;

                yield return null;
            }

            if (string.IsNullOrEmpty(error))
                error = request.error;

            progress = 1f;
            OnFinish();
        }

        void OnFinish()
        {
            try
            {
                Task task = HttpClient.GetInstance().OnRequestTaskComplete(this);
                if (task == null)
                {
                    this.state = ETaskState.Completed;
                }
                else
                {
                    this.ioTask = task;
                    //这边不改变状态，Update每帧检查ioTask的状态.
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);

                this.state = ETaskState.Completed;
            }
        }

        public void OnComplete()
        {
            this.state = ETaskState.Completed;
        }

        // 取消也等于结束
        public override void Cancel()
        {
            // 设置这2个变量，让isDone改成true，如果线程还在运行状态，线程内部会抛异常.
            // isDone也算关掉线程的一个信号
            if (state <= ETaskState.Running)
            {
                error = "aborted by user";
                state = ETaskState.Canceled;
            }

            if (request != null)
            {
                request.Abort();
            }
        }

        protected override void OnDispose()
        {
            if (state <= ETaskState.Running)
            {
                Cancel();
            }

            if (request != null)
            {
                request.Abort();
                request = null;
            }

            ioTask = null;

            ClearCallback();
        }

        // 清除回调
        public override void ClearCallback()
        {
            if (parameters != null)
                parameters.Clear();
        }
    }

}