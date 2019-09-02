using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Object = UnityEngine.Object;


namespace THGame
{
    //编辑器模式加载器[编辑器模式 同步异步无区别]
    public class NetworkAssetLoader : INetworkLoader
    {

        public T LoadAsset<T>(string urlPath, string assetName) where T : class
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(urlPath);
            request.SendWebRequest();
            Object res = DownloadHandlerAssetBundle.GetContent(request);
            if (assetName != null && assetName != "")
            {
                AssetBundle ab = res as AssetBundle;
                res = ab.LoadAsset(assetName);
            }
            return res as T;
        }

        public IEnumerator LoadAssetAsync<T>(string urlPath, string assetName, UnityAction<T> callback, UnityAction<float> progress) where T : class
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(urlPath);
            if (progress == null)
            {
                yield return request.SendWebRequest();

                Object res = DownloadHandlerAssetBundle.GetContent(request);
                if (assetName != null && assetName != "")
                {
                    AssetBundle ab = res as AssetBundle;
                    res = ab.LoadAsset(assetName);
                }
                callback?.Invoke(res as T);
            }
            else
            {
                while (!request.isDone)
                {
                    progress.Invoke(request.downloadProgress);
                    yield return 1f;
                }
                if (request.isDone)
                {
                    progress.Invoke(request.downloadProgress);
                    Object res = DownloadHandlerAssetBundle.GetContent(request);
                    if (assetName != null && assetName != "")
                    {
                        AssetBundle ab = res as AssetBundle;
                        res = ab.LoadAsset(assetName);
                    }
                    callback?.Invoke(res as T);
                }
                else if (request.isNetworkError || request.isHttpError)
                {
                    progress.Invoke(-1f);
                }

            }
        }
    }

}
