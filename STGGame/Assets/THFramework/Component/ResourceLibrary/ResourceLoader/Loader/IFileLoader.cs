using System.Collections;
using UnityEngine.Events;


namespace THGame
{
    public interface IFileLoader
    {
        //异步加载
        IEnumerator LoadAssetAsync<T>(string path, UnityAction<T> callback) where T : class;
        //同步加载
        T LoadAsset<T>(string path) where T : class;
    }

}
