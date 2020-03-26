using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using System;
using XLibrary.Package;

namespace ASGame
{

    public abstract class BaseManager<M> : MonoSingleton<M> where M : MonoBehaviour
    {
        public virtual void Preload(string path)
        {

        }

        //通用函数
        protected virtual void DoLoadAsset<T>(string path, Action<T> onSuccess = null, Action<int> onFailed = null)
        {
            //TODO:先从本地加载,没有就从服务端下载,加载完塞入缓存
        }

    }
}
