using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using XLibrary.Package;

namespace XLibGame
{
    public class HttpManager : MonoSingleton<HttpManager>
    {
        protected int m_coroutineId;
        private Dictionary<int, Coroutine> m_coroutines;
        public HttpManager()
        {
            m_coroutines = new Dictionary<int, Coroutine>();
        }

        public int Post(HttpParams args)
        {
            return StartRequest(true, args);
        }

        public int Get(HttpParams args)
        {
            return StartRequest(false, args);
        }

        public void Stop(int id)
        {
            if (m_coroutines.TryGetValue(id, out var coroutine))
            {
                StopCoroutine(coroutine);
                m_coroutines.Remove(id);
            }
        }

        public void Clear()
        {
            foreach(var coroutine in m_coroutines.Values)
            {
                StopCoroutine(coroutine);
            }
            m_coroutines.Clear();
        }

        private int StartRequest(bool isPost, HttpParams args)
        {
            if (args == null)
                return -1;

            if (string.IsNullOrEmpty(args.url))
                return -1;

            var newId = m_coroutineId++;
            Coroutine coroutine = null;
            if (isPost)
            {
                coroutine = StartCoroutine(OnPostCoroutine(newId, args));
            }
            else
            {
                coroutine = StartCoroutine(OnGetCoroutine(newId, args));
            }

            m_coroutines[newId] = coroutine;
            return newId;
        }

        IEnumerator OnPostCoroutine(int id, HttpParams args)
        {
            var newUrl = args.data.ToGetData(args.url);
            var request = UnityWebRequest.Get(newUrl);
            InitRequest(request, args);

            yield return request.SendWebRequest();
            OnRequestCallback(id, request, args);
        }

        IEnumerator OnGetCoroutine(int id, HttpParams args)
        {
            var wwwForm = args.data.ToPostData(args.url);
            var request = UnityWebRequest.Post(args.url, wwwForm);
            InitRequest(request, args);

            yield return request.SendWebRequest();
            OnRequestCallback(id, request, args);
        }

        private void InitRequest(UnityWebRequest webRequest, HttpParams args)
        {
            webRequest.timeout = args.timeout;
        }

        void OnRequestCallback(int id, UnityWebRequest webRequest, HttpParams args)
        {
            var result = new HttpResult(webRequest);

            if (webRequest.isHttpError || webRequest.isNetworkError || !webRequest.isDone)
            {
                args.onFailed?.Invoke(webRequest.error);
            }
            else
            {
                args.onSuccess?.Invoke(webRequest.downloadHandler.data);
            }
            args.onCallback?.Invoke(result);
            m_coroutines.Remove(id);
        }


    }
}
