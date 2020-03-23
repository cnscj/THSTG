
using UnityEngine;
using System.Collections;
using System;

namespace ASGame
{

    public class ResourceManager : BaseManager<ResourceManager>
    {
        public void LoadAsset<T>(string path, Action<T> onSuccess = null, Action<int> onFailed = null)
        {
            DoLoadAsset<T>(path, onSuccess, onFailed);
        }
    }

}
