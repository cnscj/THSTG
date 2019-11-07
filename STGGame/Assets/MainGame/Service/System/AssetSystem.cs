using ASGame;
namespace STGService
{
    public static class AssetSystem
    {
        public static void InitAwake()
        {
            ResourceLoader.GetInstance().loadMode = ResourceLoadMode.Editor;      //加载模式
        }

    }

}
