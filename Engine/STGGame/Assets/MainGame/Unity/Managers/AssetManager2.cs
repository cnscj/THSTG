
using System;
using ASGame;
using UnityEngine;
using XLibrary.Package;

namespace STGU3D
{
    public class AssetManager2 : MonoSingleton<AssetManager>
    {
        //可能是AB,可能是源文件
        public void LoadModel(string uid, Action<GameObject> action)
        {
            string assetPath = AssetFileBook.GetModelPath(uid);
            AssetLoaderManager.GetInstance().LoadAsset(assetPath, action);

        }

        public void LoadEffect(string uid, Action<GameObject> action)
        {
            string assetPath = AssetFileBook.GetEffectPath(uid);
            AssetLoaderManager.GetInstance().LoadAsset(assetPath, action);

        }

        public void LoadSprite(string uid, Action<GameObject> action)
        {
            string assetPath = AssetFileBook.GetSpritePath(uid);
            AssetLoaderManager.GetInstance().LoadAsset(assetPath, action);

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
