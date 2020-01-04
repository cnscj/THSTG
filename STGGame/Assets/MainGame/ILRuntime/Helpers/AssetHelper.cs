using ASGame;
using STGU3D;
using XLibrary;
using AssetManager = STGU3D.AssetManager;
namespace STGService
{
    public static class AssetHelper
    {
        public static void InitAwake()
        {
            ResourceLoader.GetInstance().loadMode = ResourceLoadMode.Editor;      //加载模式
        }

        public static AssetManager GetManager()
        {
            return AssetManager.GetInstance();
        }

        public static CSVTable LoadConfig(string code)
        {
            var content = GetManager().LoadConfig(code);
            return new CSVTable(content);
        }
    }

}
