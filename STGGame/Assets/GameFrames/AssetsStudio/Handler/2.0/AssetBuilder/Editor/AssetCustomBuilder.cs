using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XLibrary;
using XLibrary.Package;

namespace ASEditor
{
    public abstract class AssetCustomBuilder : AssetBaseBuilder
    {
        protected AssetCustomBuilderInfo _builderInfo;
        public AssetCustomBuilder():base(null)
        {

        }
        public void Init()
        {
            _builderInfo = OnInit();

            _builderName = _builderInfo.name;
        }
        public int GetPriority()
        {
            return _builderInfo.priority;
        }
        public abstract AssetCustomBuilderInfo OnInit();
    }
}
