
using System;
using ASGame;
using UnityEngine;
using XLibGame;
using XLibrary.Package;

namespace STGU3D
{
    public class AssetManager2 : MonoSingleton<AssetManager2>
    {

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

        public AssetLoadCallback<AssetBundle, AssetBundle> LoadUI(string module)
        {
            string bytesAbPath = AssetFileBook.GetUIPath(string.Format("{0}_fgui", module));
            string textureAbPath = AssetFileBook.GetUIPath(string.Format("{0}_altas", module));
            var callback = AssetLoadCallback<AssetBundle, AssetBundle>.GetOrNew();
            AssetLoaderManager.GetInstance().LoadAsset<AssetBundle>(bytesAbPath, (bytesAb) =>
            {
                AssetLoaderManager.GetInstance().LoadAsset<AssetBundle>(textureAbPath, (altasAb) =>
                {
                    callback?.onSuccess?.Invoke(bytesAb, altasAb);
                }, (altasBytes) =>
                {
                    callback?.onFailed?.Invoke(altasBytes);
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
    }
}
