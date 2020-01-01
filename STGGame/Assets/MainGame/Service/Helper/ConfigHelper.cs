using ASGame;
using STGU3D;
using XLibrary;
using AssetManager = STGU3D.AssetManager;
namespace STGService
{
    public static class ConfigHelper
    {
        public static CSVTable LoadConfig(string code)
        {
            var content = ConfigerManager.GetInstance().LoadConfig(code);
            return new CSVTable(content);
        }
    }

}
