using THEditor;
using THGame;
namespace STGEditor
{
    public static class PrefabBuildConfig
    {
        public static readonly string srcResource = "Assets/ArtEditor/Source";
        public static readonly string srcModels = PathUtil.Combine(srcResource,"Models");
        public static readonly string srcSprites = PathUtil.Combine(srcResource, "Sprites");
        public static readonly string srcPublicFx = PathUtil.Combine(srcResource, "Effects/Prefab/PublicFx");
        public static readonly string srcMaps = PathUtil.Combine(srcResource, "Maps");

        public static readonly string temResource = "Assets/ArtEditor/Temp";
        public static readonly string tempModels = PathUtil.Combine(temResource, "Models");
        public static readonly string tempSprites = PathUtil.Combine(temResource, "Sprites");
        public static readonly string tempPublicFx = PathUtil.Combine(temResource, "Effects");
        public static readonly string tempMaps = PathUtil.Combine(temResource, "Maps");

        public static readonly string temMd5 = PathUtil.Combine(temResource, "Md5");
        public static readonly string tempModelMd5s = PathUtil.Combine(temMd5, "Models");
        public static readonly string tempSpriteMd5s = PathUtil.Combine(temMd5, "Sprites");
        public static readonly string tempPublicFxMd5s = PathUtil.Combine(temMd5, "Effects");
        public static readonly string tempMapMd5s = PathUtil.Combine(temMd5, "Maps");

        public static PostProcesserManager processManager = new PostProcesserManager(new PostProcesser[]{
            new PrefabBuildProcessSprite(tempSpriteMd5s,tempSprites),
            new PrefabBuildProcessPublicFx(tempPublicFxMd5s,tempPublicFx),
            new PrefabBuildProcessMap(tempMapMd5s,tempMaps),

        });
    }
}

