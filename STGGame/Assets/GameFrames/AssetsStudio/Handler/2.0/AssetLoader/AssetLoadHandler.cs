using System;
using UnityEngine;

namespace ASGame
{
    public class AssetLoadHandler
    {
        public int id;
        public string assetPath;
        public BaseLoader loader;

        public AssetLoadCallback onCallback;

    }
}
