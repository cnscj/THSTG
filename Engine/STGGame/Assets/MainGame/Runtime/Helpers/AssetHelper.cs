using ASGame;
using STGU3D;
using XLibrary;
using AssetManager = STGU3D.AssetManager;
namespace STGRuntime
{
    public static class AssetHelper
    {
        public static void InitAwake()
        {
            ResourceLoader.GetInstance().loadMode = ResourceLoadMode.Editor;      //加载模式
        }

        public static AssetManager2 GetManager()
        {
            return AssetManager2.GetInstance();
        }

        public static CSVTable LoadConfig(string code)
        {
            var content = GetManager().LoadConfigSync(code);
            return new CSVTable(content);
        }
    }

}
