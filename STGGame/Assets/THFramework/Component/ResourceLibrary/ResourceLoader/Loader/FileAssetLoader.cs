using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;


namespace THGame
{
    //编辑器模式加载器[编辑器模式 同步异步无区别]
    public class FileAssetLoader : IFileLoader
    {
        private IFileLoader editorLoader = new EditorAssetLoader();
        private IFileLoader abLoader = new AssetBundleLoader();

        public T LoadAsset<T>(string path) where T : class
        {
            switch (ResourceLoader.GetInstance().loadMode)
            {
                case ResourceLoadMode.Editor:
                    return editorLoader.LoadAsset<T>(path);
                case ResourceLoadMode.AssetBundler:
                    return abLoader.LoadAsset<T>(path);
            }
            return null;
        }

        public IEnumerator LoadAssetAsync<T>(string path, UnityAction<T> callback) where T : class
        {
            switch (ResourceLoader.GetInstance().loadMode)
            {
                case ResourceLoadMode.Editor:
                    yield return editorLoader.LoadAssetAsync<T>(path, callback);
                    break;
                case ResourceLoadMode.AssetBundler:
                    yield return abLoader.LoadAssetAsync<T>(path, callback);
                    break;
            }
            yield return null;
        }
    }
}
