using UnityEngine;
using System.Collections;
using THEditor;

namespace STGEditor
{
    public static class AssetBuilderMain
    {
        
        public static ResourceBuilderManager builderManager = new ResourceBuilderManager(new ResourceBuilder[]
        {
            new AssetBuilderShader(),
            new AssetBuilderEffect(),
            new AssetBuilderSprite(),
            new AssetBuilderUI(),
        });
    }
}
