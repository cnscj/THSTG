using System.IO;
using THGame;
using THGame.Package;

namespace STGGame
{
    public class ResManager : Singleton<ResManager>
    {
        public static readonly string srcResource = "Assets/Resource/Source";
        public static readonly string srcModel = PathUtil.Combine(srcResource, "Models");

        //可能是AB,可能是源文件
        public static void GetModel<T>(string path)
        {

        }
    }
}
