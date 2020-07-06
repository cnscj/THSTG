using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLibGame
{
    public class HttpParams
    {
        public string url;
        public HttpForm data;
        public int timeout;
        public Action<HttpResult> onCallback;
        public Action<object> onSuccess;
        public Action<string> onFailed;
    }
}