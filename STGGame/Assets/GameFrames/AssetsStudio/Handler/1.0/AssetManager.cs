using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibrary.Package;

namespace ASGame
{
    public class AssetManager : MonoSingleton<AssetManager>
    {
        public ResourceLoadMode loadMode = ResourceLoadMode.AssetBundler;
        public string remoteUrl;    //远端资源路径
    }
}

