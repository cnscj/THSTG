﻿using ASEditor;
using STGGame;

namespace STGEditor
{
    public static class PrefabBuilderMain
    {
        public static ResourceProcesserManager processManager = new ResourceProcesserManager(new ResourceProcesser[]{
            new PrefabBuilderProcessSprite(PrefabBuildConfig.tempSpriteMd5s,PrefabBuildConfig.tempSprites),
            new PrefabBuilderProcessPublicFx(PrefabBuildConfig.tempPublicFxMd5s,PrefabBuildConfig.tempPublicFx),
            new PrefabBuilderProcessLevel(PrefabBuildConfig.tempLevelMd5s,PrefabBuildConfig.tempLevels),
            new PrefabBuilderProcessUI(PrefabBuildConfig.tempUIMd5s,PrefabBuildConfig.tempUIs),
            new PrefabBuilderProcessCfg(PrefabBuildConfig.tempConfigMd5s,PrefabBuildConfig.tempConfigs),
        });
    }
}
