﻿using UnityEngine;
using System.Collections;
using THEditor;

namespace STGEditor
{
    public static class AssetBuilderMain
    {
        
        public static BundleBuilderManager builderManager = new BundleBuilderManager(new BundleBuilder[]
        {
            new AssetBuilderEffect(),
        });
    }
}
