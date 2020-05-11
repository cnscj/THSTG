
using UnityEngine;
using System.Collections;
using System;
using Object = UnityEngine.Object;

namespace ASGame
{

    public class ResourcesManager : BaseManager<ResourcesManager>
    {
        public void LoadAsset<T>(string path, Action<T> onSuccess = null, Action<int> onFailed = null) where T : Object
        {
            var retObj = AssetCacheManager.GetInstance().GetObject<T>(path);
            if (retObj != null)
            {
                onSuccess?.Invoke(retObj);
            }
            else
            {
                DoLoadAsset<T>(path, (result) =>
                {
                    AssetCacheManager.GetInstance().AddObject(path, result, true);

                    onSuccess?.Invoke(result);
                }, onFailed);
            }
        }
    }
}
