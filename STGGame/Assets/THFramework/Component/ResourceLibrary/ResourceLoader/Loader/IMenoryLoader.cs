using System.Collections;
using UnityEngine.Events;


namespace THGame
{
    public interface IMenoryLoader
    {
        //异步加载
        IEnumerator LoadAssetAsync<T>(byte[] binary, string assetName, UnityAction<T> callback) where T : class;
        //同步加载
        T LoadAsset<T>(byte[] binary, string assetName) where T : class;
    }

}
