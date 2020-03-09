using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public class ResourcesLoader : BaseLoader
    {
        public override void LoadAtPath<T>(string path, Action<AssetLoadResult<T>> result)
        {
            AsyncOperation request = Resources.LoadAsync<T>(path);
            
        }

        public override void Unload(string path)
        {
            throw new NotImplementedException();
        }
    }
}

