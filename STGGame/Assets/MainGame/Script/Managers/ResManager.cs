using System;
using System.IO;
using THGame;
using THGame.Package;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace STGGame
{
    public class ResManager : MonoSingleton<ResManager>
    {
        public static readonly string srcResource = "Assets/ResEditor/Z_AutoProcess/AssetBundle";
        public static readonly string srcModelPath = PathUtil.Combine(srcResource,"models");
        public static readonly string srcSpritePath = PathUtil.Combine(srcResource, "sprites");

        public static readonly string srcUIPath = PathUtil.Combine(srcResource, "ui");

        public static readonly string[] residentABPaths =
        {
            PathUtil.Combine(srcModelPath, "share.ab"),
            PathUtil.Combine(srcSpritePath, "share.ab"),
            PathUtil.Combine(srcUIPath, "share.ab"),
        };

        private ResourceLoader m_loader;
        //
        public UnityEngine.Object GetObject(string assetPath,string assetName)
        {
            return m_loader.LoadFromFileSync(assetPath, assetName);
        }
        //可能是AB,可能是源文件
        public GameObject GetModel(string uid)
        {
            string assetPath = PathUtil.Combine(srcModelPath, string.Format("{0}.ab", uid));
            string assetName = string.Format("{0}.prefab", uid);
            return m_loader.LoadFromFileSync(assetPath, assetName) as GameObject;
        }

        public GameObject GetSprite(string uid)
        {
            string assetPath = PathUtil.Combine(srcSpritePath, string.Format("{0}.ab", uid));
            string assetName = string.Format("{0}.prefab", uid);
            return m_loader.LoadFromFileSync(assetPath, assetName) as GameObject;
        }

        public GameObject GetUI(string module,string vieww)
        {
            return null;
        }

        private void Awake()
        {
            m_loader = ResourceLoader.GetInstance();
        }

        private void Start()
        {
            //优先加载公共包
            foreach (var abPath in residentABPaths)
            {
                if (File.Exists(abPath))
                {
                    m_loader.LoadFromFileSync(abPath);
                }
            }
        }
    }
}
