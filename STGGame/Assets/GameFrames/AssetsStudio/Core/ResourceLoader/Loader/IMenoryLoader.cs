using System.Collections;
using UnityEngine.Events;


namespace ASGame
{
    public interface IMenoryLoader
    {
        //异步加载
        IEnumerator LoadAssetAsync<T>(byte[] binary, string assetName, UnityAction<T> callback) where T : class;
        //同步加载
        T LoadAsset<T>(byte[] binary, string assetName) where T : class;
    }

}
