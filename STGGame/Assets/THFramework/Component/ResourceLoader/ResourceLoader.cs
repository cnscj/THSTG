using System.Collections;
using THGame.Package;
using UnityEngine;
using UnityEngine.Networking;

namespace THGame
{
    public class ResourceLoader : MonoSingleton<ResourceLoader>
    {
        public enum LoadMode
        {
            Sync,           //同步
            Async,          //异步
        }

        public void LoadFromFile(LoadMode mode, string assetPath, System.Action<AssetBundle> loader, System.Action<float> progress = null)
        {
            LoadFromFile(mode, assetPath, null, (obj) =>
            {
                AssetBundle ab = obj as AssetBundle;
                loader?.Invoke(ab);
            }, progress);
        }

        public void LoadFromMemory(LoadMode mode, byte[] binary, System.Action<AssetBundle> loader, System.Action<float> progress = null)
        {
            LoadFromMemory(mode, binary, null, (obj) =>
            {
                AssetBundle ab = obj as AssetBundle;
                loader?.Invoke(ab);
            }, progress);
        }

        public void LoadFromWWW(LoadMode mode, string assetPath, System.Action<AssetBundle> loader, System.Action<float> progress = null)
        {
            LoadFromWWW(mode, assetPath, null, (obj) =>
            {
                AssetBundle ab = obj as AssetBundle;
                loader?.Invoke(ab);
            }, progress);
        }

        public void LoadFromFile(LoadMode mode,string assetPath, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            switch(mode)
            {
                case LoadMode.Sync:
                    if (loader != null)
                    {
                        Object res = LoadFromFileSync(assetPath, assetName);
                        loader?.Invoke(res);
                    }
                    break;
                case LoadMode.Async:
                    LoadFromFileAsync(assetPath, assetName, loader, progress);
                    break;
            }

        }

        //下面都不推荐使用
        public void LoadFromMemory(LoadMode mode, byte[] binary, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            switch (mode)
            {
                case LoadMode.Sync:
                    if (loader != null)
                    {
                        Object res = LoadFromMemorySync(binary, assetName);
                        loader?.Invoke(res);
                    }
                    break;
                case LoadMode.Async:
                    LoadFromMemoryAsync(binary, assetName, loader, progress);
                    break;
            }
        }

        public void LoadFromWWW(LoadMode mode, string urlPath, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            switch (mode)
            {
                case LoadMode.Sync:
                    if (loader != null)
                    {
                        Object res = LoadFromWWWSync(urlPath, assetName);
                        loader?.Invoke(res);
                    }
                    break;
                case LoadMode.Async:
                    LoadFromWWWAsync(urlPath, assetName, loader, progress);
                    break;
            }
        }

        //
        public void LoadFromFileAsync(string assetPath, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            StartCoroutine(ILoadFromFileAsync(assetPath, assetName, loader, progress));
        }
        private IEnumerator ILoadFromFileAsync(string assetPath, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetPath);
            if (progress == null)
            {
                yield return request;

                Object res = request.assetBundle;
                if (assetName != null && assetName != "")
                {
                    AssetBundle ab = res as AssetBundle;
                    res = ab.LoadAsset(assetName);
                }
                loader?.Invoke(res);
            }
            else
            {
                while (!request.isDone)
                {
                    progress.Invoke(request.progress);
                    yield return 1;
                }
                if (request.isDone)
                {
                    progress.Invoke(request.progress);
                    Object res = request.assetBundle;
                    if (assetName != null && assetName != "")
                    {
                        AssetBundle ab = res as AssetBundle;
                        res = ab.LoadAsset(assetName);
                    }
                    loader?.Invoke(res);
                }
            }
        }

        public Object LoadFromFileSync(string assetPath, string assetName = null)
        {
            Object res = AssetBundle.LoadFromFile(assetPath);
            if (assetName != null && assetName != "")
            {
                AssetBundle ab = res as AssetBundle;
                res = ab.LoadAsset(assetName);
            }
            return res;
        }
        public void LoadFromMemoryAsync(byte[] binary, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            StartCoroutine(ILoadFromMemoryAsync(binary, assetName, loader, progress));
        }
        private IEnumerator ILoadFromMemoryAsync(byte[] binary, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(binary);
            if (progress == null)
            {
                yield return request;

                Object res = request.assetBundle;
                if (assetName != null && assetName != "")
                {
                    AssetBundle ab = res as AssetBundle;
                    res = ab.LoadAsset(assetName);
                }
                loader?.Invoke(res);
            }
            else
            {
                while (!request.isDone)
                {
                    progress.Invoke(request.progress);
                    yield return 1;
                }
                if (request.isDone)
                {
                    progress.Invoke(request.progress);
                    Object res = request.assetBundle;
                    if (assetName != null && assetName != "")
                    {
                        AssetBundle ab = res as AssetBundle;
                        res = ab.LoadAsset(assetName);
                    }
                    loader?.Invoke(res);
                }
            }
        }

        public Object LoadFromMemorySync(byte[] binary, string assetName = null)
        {
            Object res = AssetBundle.LoadFromMemory(binary);
            if (assetName != null && assetName != "")
            {
                AssetBundle ab = res as AssetBundle;
                res = ab.LoadAsset(assetName);
            }
            return res;
        }
        public void LoadFromWWWAsync(string urlPath, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
        {
            StartCoroutine(ILoadFromWWWAsync(urlPath, assetName, loader, progress));
        }
        public IEnumerator ILoadFromWWWAsync(string urlPath, string assetName, System.Action<UnityEngine.Object> loader, System.Action<float> progress = null)
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
                loader?.Invoke(res);
            }
            else
            {
                while (!request.isDone)
                {
                    progress.Invoke(request.downloadProgress);
                    yield return 1;
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
                    loader?.Invoke(res);
                }else if (request.isNetworkError || request.isHttpError)
                {
                    progress.Invoke(-1);
                }
                
            }
           
        }

        public Object LoadFromWWWSync(string urlPath, string assetName = null)
        {
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(urlPath);
            request.SendWebRequest();
            Object res = DownloadHandlerAssetBundle.GetContent(request);
            if (assetName != null && assetName != "")
            {
                AssetBundle ab = res as AssetBundle;
                res = ab.LoadAsset(assetName);
            }
            return res;
        }

    }
}
