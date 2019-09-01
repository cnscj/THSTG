using System;
using System.Collections.Generic;
using System.IO;
using THGame;
using THGame.Package;
using UnityEngine;

namespace STGGame
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        public static readonly string bundleRes = PathUtil.Combine(Application.streamingAssetsPath, "ABRes", PlatformUtil.GetCurPlatformName());
        public static readonly string srcRes = PathUtil.Combine("Assets", "ResEditor" , "Z_AutoProcess" , "ResTemp");

        public static readonly Dictionary<EResType, string> m_resType = new Dictionary<EResType, string>()
        {
            [EResType.Entity] = "entities",
            [EResType.Audio] = "audios",
            [EResType.Level] = "levels",
            [EResType.Model] = "models",
            [EResType.Sprite] = "sprites",
            [EResType.Effect] = "effects",

            [EResType.UI] = "uis",
            [EResType.Shader] = "shaders",
        };

        public static readonly string[] residentABPaths =
        {
            Combine2BundlePath(EResType.Shader, "share.ab", null),
            Combine2BundlePath(EResType.Model, "share.ab", null),
            Combine2BundlePath(EResType.Sprite, "share.ab", null),
            Combine2BundlePath(EResType.UI, "share.ab", null),
            Combine2BundlePath(EResType.Level, "share.ab", null),
        };
        public static string Combine2BundlePath(EResType resType, string fileName, string assetName)
        {
            return ResourceLoaderUtil.CombineBundlePath(PathUtil.Combine(bundleRes, m_resType[resType], fileName), assetName);
        }
        public static string Combine2EditPath(EResType resType, string assetName)
        {
            return PathUtil.Combine(srcRes, m_resType[resType], assetName);
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

        public GameObject LoadUI(string module,string view)
        {
            string resPath = Combine2FixPath(EResType.UI, string.Format("{0}_{1}.ab", module, view), string.Format("{0}_{1}.prefab", module, view));

            return ResourceLoader.GetInstance().LoadFromFile<GameObject>(resPath);
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

        public string GetResPathById(string uid)
        {
            //取ID
            string resultPath = "";
            long idNum = 0;
            if (long.TryParse(uid, out idNum))
            {
                int categoryNum = int.Parse(uid.Substring(1, 1));   //FIXME:超过2位就不行了
                
                EResType category = (EResType)categoryNum;
                switch (category)
                {
                    case EResType.Entity:
                        resultPath = Combine2FixPath(category,string.Format("{0}.ab", uid), null);
                        break;
                    case EResType.Level:
                        resultPath = Combine2FixPath(category, string.Format("{0}.ab", uid), null);
                        break;
                    case EResType.Model:
                        resultPath = Combine2FixPath(category, string.Format("{0}.ab", uid), null);
                        break;
                    case EResType.Sprite:
                        resultPath = Combine2FixPath(category, string.Format("{0}.ab", uid), null);
                        break;
                }
            }
            

            return resultPath;

        }

       
    }
}
