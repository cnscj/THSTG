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
        public static GameObject GetModel(int id)
        {

            return null;
        }

        public static GameObject GetSprite(int id)
        {
            string filePath = PathUtil.Combine(srcSprite, string.Format("{0}.prefab", id));
            return Resources.Load<GameObject>(filePath);
        }
    }
}
