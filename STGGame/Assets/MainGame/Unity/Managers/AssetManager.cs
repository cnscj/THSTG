
using System.Collections.Generic;
using System.IO;
using ASGame;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace STGU3D
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        public List<string> residentABPaths = new List<string>();   //预加载的共享包

        public static string Combine2BundlePath(EResType resType, string fileName, string assetName)
        {
            return ResourceLoaderUtil.CombineBundlePath(XPathTools.Combine(ResourceBookConfig.bundleRes, GameConfig.resTypeMap[resType].ToLower(), fileName), assetName);
        }
        public static string Combine2EditPath(EResType resType, string assetName)
        {
            return XPathTools.Combine(ResourceBookConfig.srcRes, GameConfig.resTypeMap[resType], assetName);
        }
        public static string Combine2FixPath(EResType resType, string fileName, string assetName)
        {
            string path = "";
            if (ResourceXLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            {
                path = Combine2BundlePath(resType, fileName, assetName);
            }
            else if (ResourceXLoader.GetInstance().loadMode == ResourceLoadMode.Editor)
            {
                path = Combine2EditPath(resType, assetName);
            }

            return path;
        }

        private void Awake()
        {
            ResourceXLoader.GetInstance().loadMode = ResourceLoadMode.Editor;      //加载模式

        }

        private void Start()
        {
            //如果以AB方式加载,优先加载公共包
            if (ResourceXLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            {
                foreach (var abPath in residentABPaths)
                {
                    if (File.Exists(abPath))
                    {
                        ResourceXLoader.GetInstance().LoadFromFile<AssetBundle>(abPath);
                    }
                }
            }
        }

        //可能是AB,可能是源文件
        public GameObject LoadModel(string uid)
        {
            string resPath = Combine2FixPath(EResType.Model, string.Format("{0}.ab", uid), string.Format("{0}.prefab", uid));
            return ResourceXLoader.GetInstance().LoadFromFile<GameObject>(resPath);
        }

        public GameObject LoadEffect(string uid)
        {
            string resPath = Combine2FixPath(EResType.Effect, string.Format("{0}.ab", uid), string.Format("{0}.prefab", uid));
            return ResourceXLoader.GetInstance().LoadFromFile<GameObject>(resPath);
        }

        public GameObject LoadSprite(string uid)
        {
            string resPath = Combine2FixPath(EResType.Sprite, string.Format("{0}.ab", uid), string.Format("{0}.prefab", uid));
            return ResourceXLoader.GetInstance().LoadFromFile<GameObject>(resPath);
        }

        public KeyValuePair<int, System.Object> LoadUI(string module)
        {
            if (ResourceXLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            {
                string descPath = Combine2FixPath(EResType.UI, string.Format("{0}_bytes.ab", module.ToLower()), string.Format(""));
                string resPath = Combine2FixPath(EResType.UI, string.Format("{0}_atlas.ab", module.ToLower()), string.Format(""));

                AssetBundle descBundle = ResourceXLoader.GetInstance().LoadFromFile<AssetBundle>(descPath);
                AssetBundle resBundle = ResourceXLoader.GetInstance().LoadFromFile<AssetBundle>(resPath);

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
            var textAsset = ResourceXLoader.GetInstance().LoadFromFile<TextAsset>(resPath);
            return textAsset.text;
        }

        public string LoadLevel(string uid)
        {
            string sceneName = Combine2FixPath(EResType.Level, string.Format("{0}.ab", uid), string.Format("{0}.unity", uid));

            if (ResourceXLoader.GetInstance().loadMode == ResourceLoadMode.AssetBundler)
            {
                string resPath = Combine2FixPath(EResType.Level, string.Format("{0}.ab", uid), null);
                var bundle = ResourceXLoader.GetInstance().LoadFromFile<AssetBundle>(resPath);

                if (bundle.isStreamedSceneAssetBundle)
                {
                    var scenePaths = bundle.GetAllScenePaths();
                    sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);

                }
            }

            return sceneName;
        }
    }
}
