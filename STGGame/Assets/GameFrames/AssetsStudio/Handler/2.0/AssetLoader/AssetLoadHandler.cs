using System;
using UnityEngine;

namespace ASGame
{
    public class AssetLoadHandler
    {
        public int id;
        public BaseLoader loader;

        public string assetPath;
        public AssetLoadCallback onCallback;

    }
}
