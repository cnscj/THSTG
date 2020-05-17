using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    //TODO:负责对下载好文件,或其他二进制文件进行解码,加载
    public class BinarylLoader : BaseCoroutineLoader
    {
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}

