using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class BundleLoader : BaseLoader
    {
        public override void LoadAtPath<T>(string path, Action<AssetLoadResult<T>> result)
        {

            StartCoroutine(LoadCoroutine(path, result));
        }

        public override void Unload(string path)
        {
            
        }

        private IEnumerator LoadCoroutine<T>(string path, Action<AssetLoadResult<T>> result)
        {
            var request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            result?.Invoke(new AssetLoadResult<T>()
            {
                //asset = request.assetBundle,
            });
        }
    }
}

