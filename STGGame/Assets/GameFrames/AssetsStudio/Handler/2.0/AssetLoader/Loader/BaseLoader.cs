using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASGame
{
    public abstract class BaseLoader : MonoBehaviour
    {
        public abstract AssetLoadHandler StartLoad(string path);
        public abstract void StopLoad(AssetLoadHandler handler);
        public abstract void Clear();
    }
}

