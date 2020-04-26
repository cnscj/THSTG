using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace ASGame
{
    public class NetworkLoader : BaseCoroutineLoader
    {
        protected override IEnumerator OnLoadAsset(AssetLoadHandler handler)
        {
            //TODO:
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle("");
            yield return request.SendWebRequest();


        }
    }
}

