using ASEditor;
using STGGame;

namespace STGEditor
{
    public static class PrefabBuilderMain
    {
        public static ResourceProcesserManager processManager = new ResourceProcesserManager(new ResourceProcesser[]{
            new PrefabBuilderProcessSprite(PrefabBuildConfig.tempSpriteMd5s,PrefabBuildConfig.tempSprites),
            new PrefabBuilderProcessEffect(PrefabBuildConfig.tempEffectMd5s,PrefabBuildConfig.tempEffect),
            new PrefabBuilderProcessLevel(PrefabBuildConfig.tempLevelMd5s,PrefabBuildConfig.tempLevels),
            new PrefabBuilderProcessUI(PrefabBuildConfig.tempUIMd5s,PrefabBuildConfig.tempUIs),
            new PrefabBuilderProcessScript(PrefabBuildConfig.tempScriptMd5s,PrefabBuildConfig.tempScripts),
            new PrefabBuilderProcessCfg(PrefabBuildConfig.tempConfigMd5s,PrefabBuildConfig.tempConfigs),
        });
    }
}

