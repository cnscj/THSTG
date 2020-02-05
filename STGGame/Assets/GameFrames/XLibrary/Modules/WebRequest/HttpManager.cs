using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using XLibrary.Package;

namespace XLibGame
{
    public class HttpManager : MonoSingleton<HttpManager>
    {
        public enum MethodType
        {
            GET,
            POST
        }

        public enum DownloadHanlderType
        {
            Byte,
            Text
        }

        public static string CreateGetData(string url, RequestForm formData)
        {
            string ret = url;
            StringBuilder data = new StringBuilder();

            if (formData != null && formData.Count() > 0)
            {
                foreach (var item in formData.GetParams())
                {
                    data.Append(item.Key + "=");
                    data.Append(item.Value.ToString() + "&");
                }
                data.Remove(data.Length - 1, 1);
            }

            if (url.IndexOf("?") == -1)
            {
                url += "?";
            }
            else
            {
                url += "&";
            }

            ret = url + data.ToString();

            return ret;
        }

        public static WWWForm CreatePostData(string url, RequestForm formData)
        {
            WWWForm ret = new WWWForm();
            if (formData != null && formData.Count() > 0)
            {
                foreach (var item in formData.GetParams())
                {
                    if (item.Value is byte[])
                        ret.AddBinaryData(item.Key, item.Value as byte[]);
                    else
                        ret.AddField(item.Key, item.Value.ToString());
                }
            }
            return ret;
        }

        public void Get(string url, RequestForm form, Action<object> onSuccess = null, Action<string> onFailed = null)
        {
            StartCoroutine(InvokeRequest(MethodType.GET, url, form, CreateCallback(onSuccess, onFailed, DownloadHanlderType.Byte)));
        }

        public void Post(string url, RequestForm form, Action<object> onSuccess = null, Action<string> onFailed = null)
        {
            StartCoroutine(InvokeRequest(MethodType.POST, url, form, CreateCallback(onSuccess, onFailed, DownloadHanlderType.Byte)));
        }
        public void Get(string url, Action<object> onSuccess = null, Action<string> onFailed = null)
        {
            Get(url, null, onSuccess, onFailed);
        }
        public void Post(string url, Action<object> onSuccess = null, Action<string> onFailed = null)
        {
            Post(url, null, onSuccess, onFailed);
        }

        public void Request(MethodType methodType, string url, RequestForm form = null, Action<UnityWebRequest> callback = null)
        {
            StartCoroutine(InvokeRequest(methodType, url, form, callback));
        }

        IEnumerator InvokeRequest(MethodType methodType, string url, RequestForm form, Action<UnityWebRequest> callback)
        {
            UnityWebRequest request = null;
            if (methodType == MethodType.GET)
            {
                string newUrl = CreateGetData(url, form);
                request = UnityWebRequest.Get(url);
            }
            else if(methodType == MethodType.POST)
            {
                WWWForm formData = CreatePostData(url, form);
                request = UnityWebRequest.Post(url, formData);
            }

            yield return request.SendWebRequest();
            callback?.Invoke(request);
        }

        Action<UnityWebRequest> CreateCallback(Action<object> onSuccess, Action<string> onFailed, DownloadHanlderType dateType)
        {
            if (onSuccess == null && onFailed == null)
                return null;

            Action<UnityWebRequest> callback = (request) => {
                if (request.isHttpError || request.isNetworkError)
                {
                    onFailed?.Invoke(request.error);
                }

                if (request.isDone)
                {
                    if (dateType == DownloadHanlderType.Text)
                    {
                        onSuccess?.Invoke(request.downloadHandler.text);
                    }
                    else if (dateType == DownloadHanlderType.Byte)
                    {
                        onSuccess?.Invoke(request.downloadHandler.data);
                    }
                }
            };

            return callback;
        }
    }
}



