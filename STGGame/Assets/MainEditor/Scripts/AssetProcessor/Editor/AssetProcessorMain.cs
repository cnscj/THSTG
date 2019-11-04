using ASEditor;
using STGGame;

namespace STGEditor
{
    public static class AssetProcessorMain
    {
        public static ResourceProcesserManager processManager = new ResourceProcesserManager(new ResourceProcesser[]{
            new AssetProcessorSprite(AssetProcessorConfig.tempSpriteMd5s,AssetProcessorConfig.tempSprites),
            new AssetProcessorEffect(AssetProcessorConfig.tempEffectMd5s,AssetProcessorConfig.tempEffect),
            new AssetProcessorLevel(AssetProcessorConfig.tempLevelMd5s,AssetProcessorConfig.tempLevels),
            new AssetProcessorUI(AssetProcessorConfig.tempUIMd5s,AssetProcessorConfig.tempUIs),
            new AssetProcessorScript(AssetProcessorConfig.tempScriptMd5s,AssetProcessorConfig.tempScripts),
            new AssetProcessorCfg(AssetProcessorConfig.tempConfigMd5s,AssetProcessorConfig.tempConfigs),
        });
    }
}

