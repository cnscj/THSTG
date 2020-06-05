
using System;
using ASGame;
using UnityEngine;
using XLibrary.Package;

namespace STGU3D
{
    public class AssetManager2 : MonoSingleton<AssetManager2>
    {
        //可能是AB,可能是源文件
        public AssetLoadCallback LoadModel(string uid)
        {
            string assetPath = AssetFileBook.GetModelPath(uid);
            var callback = new AssetLoadCallback();
            AssetLoaderManager.GetInstance().LoadAsset<GameObject>(assetPath, (obj) =>
            {
                callback.onSuccess?.Invoke(obj);
            }, (reason) =>
            {
                callback.onFailed?.Invoke(reason);
            });
            return callback;
        }

        public AssetLoadCallback LoadEffect(string uid)
        {
            string assetPath = AssetFileBook.GetEffectPath(uid);
            var callback = new AssetLoadCallback();
            AssetLoaderManager.GetInstance().LoadAsset<GameObject>(assetPath, (obj) =>
            {
                callback.onSuccess?.Invoke(obj);
            }, (reason) =>
            {
                callback.onFailed?.Invoke(reason);
            });
            return callback;

        }

        public AssetLoadCallback LoadSprite(string uid)
        {
            string assetPath = AssetFileBook.GetSpritePath(uid);
            var callback = new AssetLoadCallback();
            AssetLoaderManager.GetInstance().LoadAsset<GameObject>(assetPath,(obj)=>
            {
                callback.onSuccess?.Invoke(obj);
            }, (reason)=>
            {
                callback.onFailed?.Invoke(reason);
            });
            return callback;
        }

        public void LoadUI(string module, Action<AssetBundle, AssetBundle> action)
        {
            string bytesAbPath = AssetFileBook.GetUIPath(string.Format("{0}_fgui", module));
            string textureAbPath = AssetFileBook.GetUIPath(string.Format("{0}_altas", module));

            AssetLoaderManager.GetInstance().LoadAsset<AssetBundle>(bytesAbPath, (bytesAb)=>
            {
                AssetLoaderManager.GetInstance().LoadAsset<AssetBundle>(textureAbPath, (altasAb) =>
                {
                    action?.Invoke(bytesAb, altasAb);
                });
            });
        }

        public void LoadConfig(string fileName, Action<string> action)
        {
            string assetPath = AssetFileBook.GetConfigPath(fileName);
            AssetLoaderManager.GetInstance().LoadAsset<TextAsset>(assetPath, (textAsset)=>
            {
                action?.Invoke(textAsset.text);
            });
        }
    }
}
