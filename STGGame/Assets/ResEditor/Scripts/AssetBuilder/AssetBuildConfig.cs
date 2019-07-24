using UnityEngine;
using System.Collections;
using THEditor;

namespace STGEditor
{
    public static class AssetBuilderConfig
    {
        
        public static BundleBuilderManager builderManager = new BundleBuilderManager(new BundleBuilder[]
        {
            new AssetBuilderEffect(),
        });
    }
}
