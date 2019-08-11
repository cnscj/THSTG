using UnityEngine;
using System.Collections;
using THEditor;

namespace STGEditor
{
    public static class AssetBuilderMain
    {
        
        public static BundleBuilderManager builderManager = new BundleBuilderManager(new BundleBuilder[]
        {
            new AssetBuilderShader(),
            new AssetBuilderEffect(),
            new AssetBuilderSprite(),
            new AssetBuilderUI(),
        });
    }
}
