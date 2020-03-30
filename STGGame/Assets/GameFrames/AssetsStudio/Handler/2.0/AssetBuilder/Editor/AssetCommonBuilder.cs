using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    public class AssetCommonBuilder : AssetBaseBuilder
    {
        public AssetCommonBuilder(string name) : base(name)
        {

        }
        protected override string[] OnFiles()
        {
            return null;
        }
        protected override string OnName(string assetPath)
        {
            return null;
        }

    }
}
