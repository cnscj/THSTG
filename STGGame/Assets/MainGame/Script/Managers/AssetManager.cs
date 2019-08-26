using System;
using System.IO;
using THGame;
using THGame.Package;
using UnityEngine;

namespace STGGame
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        public static readonly string srcResource = PathUtil.Combine(Application.streamingAssetsPath, "ABRes", PlatformUtil.GetCurPlatformName());
        public static readonly string srcShaderPath = PathUtil.Combine(srcResource, "shaders");
        public static readonly string srcUIPath = PathUtil.Combine(srcResource, "ui");

        public static readonly string srcEntityPath = PathUtil.Combine(srcResource, "entities");
        public static readonly string srcLevelPath = PathUtil.Combine(srcResource, "levels");
        public static readonly string srcModelPath = PathUtil.Combine(srcResource,"models");
        public static readonly string srcSpritePath = PathUtil.Combine(srcResource, "sprites");


        public static readonly string[] residentABPaths =
        {
            PathUtil.Combine(srcShaderPath, "share.ab"),
            PathUtil.Combine(srcModelPath, "share.ab"),
            PathUtil.Combine(srcSpritePath, "share.ab"),
            PathUtil.Combine(srcUIPath, "share.ab"),
            PathUtil.Combine(srcLevelPath, "share.ab"),
        };

        //可能是AB,可能是源文件
        public GameObject LoadModel(string uid)
        {
            string abPath = PathUtil.Combine(srcModelPath, string.Format("{0}.ab", uid));
            string assetName = string.Format("{0}.prefab", uid);
            string bundlePath = ResourceLoaderUtil.CombineBundlePath(abPath, assetName);
            return ResourceLoader.GetInstance().LoadFromFile<GameObject>(bundlePath);
        }

        public GameObject LoadSprite(string uid)
        {
            string abPath = PathUtil.Combine(srcSpritePath, string.Format("{0}.ab", uid));
            string assetName = string.Format("{0}.prefab", uid);
            string bundlePath = ResourceLoaderUtil.CombineBundlePath(abPath, assetName);
            return ResourceLoader.GetInstance().LoadFromFile<GameObject>(bundlePath);
        }

        public GameObject LoadUI(string module,string view)
        {
            string abPath = PathUtil.Combine(srcSpritePath, string.Format("{0}_{1}.ab", module, view));
            string assetName = string.Format("{0}_{1}.prefab", module, view);
            string bundlePath = ResourceLoaderUtil.CombineBundlePath(abPath, assetName);
            return ResourceLoader.GetInstance().LoadFromFile<GameObject>(bundlePath);
        }

        public AssetBundle LoadLevel(string uid)
        {
            string bundlePath = PathUtil.Combine(srcLevelPath, string.Format("{0}.ab", uid));
            var bundle = ResourceLoader.GetInstance().LoadFromFile<AssetBundle>(bundlePath);
            return bundle;
        }

        public string GetResPathById(string uid)
        {
            //取ID
            string resultPath = "";
            long idNum = 0;
            if (long.TryParse(uid, out idNum))
            {
                int categoryNum = int.Parse(uid.Substring(1, 1));   //FIXME:超过2位就不行了
                
                EResCategory category = (EResCategory)categoryNum;
                switch (category)
                {
                    case EResCategory.Entity:
                        resultPath = PathUtil.Combine(srcEntityPath, string.Format("{0}.ab", uid));
                        break;
                    case EResCategory.Level:
                        resultPath = PathUtil.Combine(srcLevelPath, string.Format("{0}.ab", uid));
                        break;
                    case EResCategory.Model:
                        resultPath = PathUtil.Combine(srcModelPath, string.Format("{0}.ab", uid));
                        break;
                    case EResCategory.Sprite:
                        resultPath = PathUtil.Combine(srcSpritePath, string.Format("{0}.ab", uid));
                        break;
                }
            }
            

            return resultPath;

        }

        private void Start()
        {
            //优先加载公共包
            foreach (var abPath in residentABPaths)
            {
                if (File.Exists(abPath))
                {
                    ResourceLoader.GetInstance().LoadFromFile<AssetBundle>(abPath);
                }
            }
        }
    }
}
