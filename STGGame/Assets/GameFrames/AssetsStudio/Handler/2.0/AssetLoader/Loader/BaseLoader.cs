using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ASGame
{
    public abstract class BaseLoader : MonoBehaviour
    {
        public abstract void LoadAtPath<T>(string path, Action<AssetLoadResult<T>> result) where T : Object;
        public abstract void Unload(string path);

        private Dictionary<string, BaseLoader> _readyList;          //预备加载的列表
        private Dictionary<string, BaseLoader> _loadingList;        //正在加载的列表
        private Dictionary<string, BaseLoader> _loadedList;         //加载完成的列表
        private Dictionary<string, BaseLoader> _unloadList;         //准备卸载的列表
    }
}

