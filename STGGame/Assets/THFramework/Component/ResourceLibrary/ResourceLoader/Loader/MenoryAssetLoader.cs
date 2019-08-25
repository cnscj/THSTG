using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


namespace THGame
{
    //编辑器模式加载器[编辑器模式 同步异步无区别]
    public class MenoryAssetLoader : IMenoryLoader
    {

        public T LoadAsset<T>(byte[] binary, string assetName) where T : class
        {
            Object res = AssetBundle.LoadFromMemory(binary);
            return res as T;
        }

        public IEnumerator LoadAssetAsync<T>(byte[] binary, string assetName, UnityAction<T> callback) where T : class
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(binary);
            yield return request;

            Object res = request.assetBundle;
            if (assetName != null && assetName != "")
            {
                AssetBundle ab = res as AssetBundle;
                res = ab.LoadAsset(assetName);
            }
            callback?.Invoke(res as T);
        }
    }

}
