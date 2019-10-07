using ASEditor;

namespace STGEditor
{
    public static class AssetBuilderMain
    {
        
        public static ResourceBuilderManager builderManager = new ResourceBuilderManager(new ResourceBuilder[]
        {
            new AssetBuilderShader(),
            new AssetBuilderEffect(),
            new AssetBuilderSprite(),
            new AssetBuilderLevel(),
            new AssetBuilderUI(),
            new AssetBuilderCfg(),

            new AssetBuilderCustom(),
        });
    }
}
