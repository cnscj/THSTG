
using UnityEngine;
using XLibrary.Package;
using Object = UnityEngine.Object;
//全部采用异步
namespace ASGame
{
    public class AssetLoadManager : Singleton<AssetLoadManager>
    {

        //万能的加载
        public AssetLoadHandler Load(string path)
        {
            //从Cache取得可用的handler,去进行加载
            return null;
        }


    }

}
