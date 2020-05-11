using System.Collections;
using UnityEngine.Events;


namespace ASGame
{
    public interface INetworkLoader
    {
        //异步加载
        IEnumerator LoadAssetAsync<T>(string path, string assetName, UnityAction<T> callback, UnityAction<float> progress) where T : class;
        //同步加载
        T LoadAsset<T>(string path, string assetName) where T : class;
    }

}
