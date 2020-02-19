
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
//全部采用异步
namespace ASGame
{
    public class AssetLoader : Singleton<AssetLoader>
    {
        //万能的加载
        public AssetLoadHandler Load(string path)
        {
            return null;
        }


    }

}
