
using System;
using System.Collections.Generic;
using System.IO;
using ASGame;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace STGGame
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        public List<string> residentABPaths = new List<string>();   //预加载的共享包

        public static string Combine2BundlePath(EResType resType, string fileName, string assetName)
        {
            return ResourceLoaderUtil.CombineBundlePath(PathUtil.Combine(ResourceBookConfig.bundleRes, GameConfig.resTypeMap[resType].ToLower(), fileName), assetName);
        }
        public static string Combine2EditPath(EResType resType, string assetName)
        {
            return PathUtil.Combine(ResourceBookConfig.srcRes, GameConfig.resTypeMap[resType], assetName);
        }
        public static string Combine2FixPath(EResType resType, string fileName, string assetName)
        {
            string path = "";
            if (ResourceLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            {
                path = Combine2BundlePath(resType, fileName, assetName);
            }
            else if (ResourceLoader.GetInstance().loadMode == ResourceLoadMode.Editor)
            {
                path = Combine2EditPath(resType,assetName);
            }

            return path;
        }

        private void Awake()
        {
            ResourceLoader.GetInstance().loadMode = ResourceLoadMode.Editor;      //加载模式

        }

        private void Start()
        {
            //如果以AB方式加载,优先加载公共包
            if (ResourceLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            {
                foreach (var abPath in residentABPaths)
                {
                    if (File.Exists(abPath))
                    {
                        ResourceLoader.GetInstance().LoadFromFile<AssetBundle>(abPath);
                    }
                }
            }
        }

        //可能是AB,可能是源文件
        public GameObject LoadModel(string uid)
        {
            string resPath = Combine2FixPath(EResType.Model, string.Format("{0}.ab", uid), string.Format("{0}.prefab", uid));
            return ResourceLoader.GetInstance().LoadFromFile<GameObject>(resPath);
        }

        public GameObject LoadSprite(string uid)
        {
            string resPath = Combine2FixPath(EResType.Sprite, string.Format("{0}.ab", uid), string.Format("{0}.prefab", uid));
            return ResourceLoader.GetInstance().LoadFromFile<GameObject>(resPath);
        }

        public KeyValuePair<int, System.Object> LoadUI(string module)
        {
            if (ResourceLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            { 
                string descPath = Combine2FixPath(EResType.UI, string.Format("{0}_bytes.ab", module.ToLower()), string.Format(""));
                string resPath = Combine2FixPath(EResType.UI, string.Format("{0}_atlas.ab", module.ToLower()), string.Format(""));

                AssetBundle descBundle = ResourceLoader.GetInstance().LoadFromFile<AssetBundle>(descPath);
                AssetBundle resBundle = ResourceLoader.GetInstance().LoadFromFile<AssetBundle>(resPath);

                return new KeyValuePair<int, System.Object>((int)ResourceLoadMode.AssetBundler, new KeyValuePair<AssetBundle, AssetBundle>(descBundle, resBundle));
            }
            else
            {
                string uiPath = Combine2FixPath(EResType.UI, string.Format(""), string.Format("{0}", module));
                return new KeyValuePair<int, System.Object>((int)ResourceLoadMode.Editor, uiPath);
            }
        }

        public string LoadConfig(string fileName)
        {
            string fileNameWithoutEx = Path.GetFileNameWithoutExtension(fileName);
            string fileExtName = Path.GetExtension(fileName);
            fileExtName = fileExtName == "" ? ".csv" : fileExtName;
            string resPath = Combine2FixPath(EResType.Config, string.Format("{0}.ab", fileNameWithoutEx), string.Format("{0}{1}", fileNameWithoutEx, fileExtName));
            var textAsset = ResourceLoader.GetInstance().LoadFromFile<TextAsset>(resPath);
            return textAsset.text;
        }

        public string LoadLevel(string uid)
        {
            string sceneName = Combine2FixPath(EResType.Level, string.Format("{0}.ab", uid), string.Format("{0}.unity", uid));

            if (ResourceLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            {
                string resPath = Combine2FixPath(EResType.Level, string.Format("{0}.ab", uid), null);
                var bundle = ResourceLoader.GetInstance().LoadFromFile<AssetBundle>(resPath);

                if (bundle.isStreamedSceneAssetBundle)
                {
                    var scenePaths = bundle.GetAllScenePaths();
                    sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);

                }
            }
           
            return sceneName;
        }


        public byte[] LoadScript(string fileName)
        {
            string resPath = Combine2FixPath(EResType.Script, string.Format("{0}.ab", fileName), string.Format("{0}.bytes", fileName));
            var dllData = ResourceLoader.GetInstance().LoadFromFile<TextAsset>(resPath);

            return dllData.bytes;
        }
    }
}
