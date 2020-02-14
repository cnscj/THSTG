
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
            var handler = new AssetLoadHandler();

            return handler;
        }

        //从下载,到加载能直接用
        public void LoadBundle(string path, string name)
        {
            
        }

    }

}
