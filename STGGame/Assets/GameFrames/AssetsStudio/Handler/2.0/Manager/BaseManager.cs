using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using System;

namespace ASGame
{

    public abstract class BaseManager : MonoBehaviour
    {
        //通用函数
        public virtual void LoadAsset<T>(string path, Action<Object> onSuccess = null, Action<int> onFailed = null)
        {
            //TODO:先从本地加载,没有就从服务端下载,加载完塞入缓存
        }

    }
}
