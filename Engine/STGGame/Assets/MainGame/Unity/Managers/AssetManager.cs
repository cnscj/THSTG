
using System;
using System.IO;
using ASGame;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace STGU3D
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        private void Start()
        {
            string assetPath = AssetFileBook.GetBundleMainfest();
            AssetLoaderManager.GetInstance().LoadBundleMainfest(assetPath);
        }

        //可能是AB,可能是源文件
        public AssetLoadCallback<GameObject> LoadModel(string uid)
        {
            string assetPath = AssetFileBook.GetModelPath(uid);
            var callback = AssetLoadCallback<GameObject>.GetOrNew();

            AssetLoaderManager.GetInstance().LoadAsset<GameObject>(assetPath, (obj) =>
            {
                callback.onSuccess?.Invoke(obj);
            }, (reason) =>
            {
                callback.onFailed?.Invoke(reason);
            });
            return callback as AssetLoadCallback<GameObject>;
        }

        public AssetLoadCallback<GameObject> LoadEffect(string uid)
        {
            string assetPath = AssetFileBook.GetEffectPath(uid);
            var callback = AssetLoadCallback<GameObject>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAsset<GameObject>(assetPath, (obj) =>
            {
                callback.onSuccess?.Invoke(obj);
            }, (reason) =>
            {
                callback.onFailed?.Invoke(reason);
            });
            return callback as AssetLoadCallback<GameObject>;

        }

        public AssetLoadCallback<GameObject> LoadSprite(string uid)
        {
            string assetPath = AssetFileBook.GetSpritePath(uid);
            var callback = AssetLoadCallback<GameObject>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAsset<GameObject>(assetPath, (obj) =>
             {
                 callback.onSuccess?.Invoke(obj);
             }, (reason) =>
             {
                 callback.onFailed?.Invoke(reason);
             });
            return callback as AssetLoadCallback<GameObject>;
        }

        public AssetLoadCallback<AssetBundle[]> LoadUI(string module)
        {
            string bytesAbPath = AssetFileBook.GetUIPath(string.Format("{0}_fgui", module));
            string textureAbPath = AssetFileBook.GetUIPath(string.Format("{0}_altas", module));
            var callback = AssetLoadCallback<AssetBundle[]>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAsset<AssetBundle>(bytesAbPath, (bytesAb) =>
            {
                AssetLoaderManager.GetInstance().LoadAsset<AssetBundle>(textureAbPath, (altasAb) =>
                {
                    callback?.onSuccess?.Invoke(new AssetBundle[] { bytesAb, altasAb });
                }, (altasBytes) =>
                {
                    if(bytesAb != null)
                    {
                        callback?.onSuccess?.Invoke(new AssetBundle[] { bytesAb, null });
                    }
                    else
                    {
                        callback?.onFailed?.Invoke(altasBytes);
                    }

                });
            }, (reasonBytes) =>
             {
                 callback?.onFailed?.Invoke(reasonBytes);
             });
            return callback;
        }

        public AssetBundle[] LoadUISync(string module)
        {
            string bytesAbPath = AssetFileBook.GetUIPath(string.Format("{0}_fgui", module));
            string textureAbPath = AssetFileBook.GetUIPath(string.Format("{0}_altas", module));

            var bytesAb = AssetBundle.LoadFromFile(bytesAbPath);
            var altasAb = AssetBundle.LoadFromFile(textureAbPath);

            return new AssetBundle[] {bytesAb,altasAb };
    }

        public AssetLoadCallback<string> LoadConfig(string fileName)
        {
            string assetPath = AssetFileBook.GetConfigPath(fileName);
            var callback = AssetLoadCallback<string>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAsset<TextAsset>(assetPath, (textAsset)=>
            {
                callback.onSuccess?.Invoke(textAsset.text);
            },(reason) => {
                callback.onFailed?.Invoke(reason);
            });
            return callback;
        }
        //public string LoadConfigSync(string fileName)
        //{
        //    string abPath = Path.Combine(AssetFileBook.GetAbAssetRoot(), string.Format("{0}.ab", fileName));
        //    string assetName = Path.Combine(AssetFileBook.CONFIG_ROOT, string.Format("{0}.csv", fileName));

        //    var ab = AssetBundle.LoadFromFile(abPath);
        //    TextAsset textAsset = ab.LoadAsset<TextAsset>(assetName);

        //    return textAsset.text;
        //}

    }
}
