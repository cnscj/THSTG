
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
        public Callback<GameObject, int> LoadModel(string uid)
        {
            string assetPath = AssetFileBook.GetModelPath(uid);
            var callback = Callback<GameObject, int>.GetOrNew();

            AssetLoaderManager.GetInstance().LoadAssetAsync<GameObject>(assetPath, (obj) =>
            {
                callback.onSuccess?.Invoke(obj);
            }, (reason) =>
            {
                callback.onFailed?.Invoke(reason);
            });
            return callback;
        }

        public Callback<GameObject, int> LoadEffect(string uid)
        {
            string assetPath = AssetFileBook.GetEffectPath(uid);
            var callback = Callback<GameObject, int>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAssetAsync<GameObject>(assetPath, (obj) =>
            {
                callback.onSuccess?.Invoke(obj);
            }, (reason) =>
            {
                callback.onFailed?.Invoke(reason);
            });
            return callback;

        }

        public Callback<GameObject, int> LoadSprite(string uid)
        {
            string assetPath = AssetFileBook.GetSpritePath(uid);
            var callback = Callback<GameObject, int>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAssetAsync<GameObject>(assetPath, (obj) =>
             {
                 callback.onSuccess?.Invoke(obj);
             }, (reason) =>
             {
                 callback.onFailed?.Invoke(reason);
             });
            return callback;
        }

        
        public Callback<AssetBundle[], int> LoadUI(string module)
        {
            string bytesAbPath = AssetFileBook.GetUIPath(string.Format("{0}_fgui", module));
            string textureAbPath = AssetFileBook.GetUIPath(string.Format("{0}_altas", module));
            var callback = Callback<AssetBundle[], int>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAssetSync<AssetBundle>(bytesAbPath, (bytesAb) =>
            {
                AssetLoaderManager.GetInstance().LoadAssetSync<AssetBundle>(textureAbPath, (altasAb) =>
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

        
        public string LoadConfig(string fileName)
        {
            string content = null;
            string assetPath = AssetFileBook.GetConfigPath(fileName);
            var callback = Callback<string,int >.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAssetSync<TextAsset>(assetPath, (textAsset)=>
            {
                content = textAsset.text;
            },  (reason) =>
            {
                callback.onFailed?.Invoke(reason);
            });
            return content;
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
