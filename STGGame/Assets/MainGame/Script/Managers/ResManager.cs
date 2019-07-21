using System;
using System.IO;
using THGame;
using THGame.Package;
using UnityEngine;

namespace STGGame
{
    public class ResManager : Singleton<ResManager>
    {
        public static readonly string srcResource = "Assets/ArtEditor/Temp";
        public static readonly string srcModel = PathUtil.Combine(srcResource, "Models");
        public static readonly string srcSprite = PathUtil.Combine(srcResource, "Sprites");
        //可能是AB,可能是源文件
        public GameObject GetModel(int id)
        {

            return null;
        }

        public GameObject GetSprite(int id)
        {
            string filePath = PathUtil.Combine(srcSprite, string.Format("{0}.prefab", id));
            return Resources.Load<GameObject>(filePath);
        }


        public void LoadAsync(string path, Action<UnityEngine.Object> loaded, Action<float> progress = null)
        {

            
        }
    }
}
